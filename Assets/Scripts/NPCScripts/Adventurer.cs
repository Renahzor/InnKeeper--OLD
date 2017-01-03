
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    public enum Profession { Fighter, Pickpocket, Acolyte, Apprentice };
    public enum Race { Human, Elf, Dwarf, Halfling }

    Profession profession;
    Race race;

    public int level, HP, maxHP;
    int strength, agility, toughness, smarts, minDamage, maxDamage, exp, gold;

    public string advName;

    float attackTimer = 1.0f;
    public string advActivity = "Relaxing...";
    public float activityTime = 0.0f;

    public List<string> statsList = new List<string>();

    Quest currentQuest = null;

    float stateCheckCooldown = 5.0f;

    //state tracking
    bool atBed, atTable, atQuest, wantsQuest, hasActivity = false;

    //Store health bar so we dont have to "find" it multiple times
    public Image healthBar = null;

    StateTracker myState;
    NPCBehaviors behaviors;
    AdventurerNeeds myNeeds;

    //creates a new random adventurer
    void Awake()
    {
        NewAdventurer(1);
    }

    void NewAdventurer(int _level)
    {
        profession = (Profession)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Profession)).Length);
        race = (Race)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Race)).Length);
        level = _level;
        strength = 1;
        agility = 1;
        toughness = 1;
        smarts = 1;
        HP = 10;
        maxHP = HP;
        minDamage = 1;
        maxDamage = 5;

        exp = 0;

        gold = 2;
        advName = "Random Dude";
        UpdateStatList();
        myState = new StateTracker();
        behaviors = GetComponent<NPCBehaviors>();
        myNeeds = GetComponent<AdventurerNeeds>();
    }

    void Update()
    {
        stateCheckCooldown -= Time.deltaTime;

        if (stateCheckCooldown <= 0.0f)
        {
            myState.UpdateState(GetComponent<AdventurerNeeds>());
            stateCheckCooldown = 5.0f;
        }

        //check for health fist
        if (HP < maxHP * 0.8 && !hasActivity)
        {
            hasActivity = true;
            StartCoroutine(MoveToBed());
        }

        //if no activity, find an activity that interests the npc
        if (!hasActivity)
        {
            switch(myState.GetCurrentState())
            {
                case StateTracker.States.WantsQuest:
                    GetQuest();
                    break;                   
                case StateTracker.States.WantsFood:
                    StartCoroutine(MoveToTable());
                    break;
                default: break;   
            }
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            atBed = true;
        else if (otherCollider.tag == "TableItems")
            atTable = true;
        else if (otherCollider.tag == "QuestGiver")
            atQuest = true;
    }

    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            atBed = false;
        else if (otherCollider.tag == "TableItems")
            atTable = false;
        else if (otherCollider.tag == "QuestGiver")
            atQuest = false;
    }

    public void UpdateStatList()
    {
        statsList.Clear();
        statsList.Add(profession.ToString());
        statsList.Add(race.ToString());

        statsList.Add("Level: " + level);
        statsList.Add("Strength: " + strength);
        statsList.Add("Agility: " + agility);
        statsList.Add("Toughness: " + toughness);
        statsList.Add("Smarts: " + smarts);
        statsList.Add("Gold: " + gold);
    }

    public void GetQuest()
    {
        List<Quest> desireableQuests = new List<Quest>();

        ActiveQuestManager aqm = GameObject.Find("GameMaster").GetComponent<ActiveQuestManager>();

        foreach (Quest q in aqm.activeQuests.Keys)
        {
            if (q.locationIndex + q.locationIndex <= level && q.goldReward >= level)
            {
                desireableQuests.Add(q);
            }
        }

        if (desireableQuests.Count > 0)
        {
            currentQuest = desireableQuests[UnityEngine.Random.Range(0, desireableQuests.Count)];
            hasActivity = true;
            StartCoroutine(MoveToQuest(currentQuest));
        }

        return;
    }

    IEnumerator RunQuest(Quest q)
    {
        hasActivity = true;

        this.gameObject.GetComponent<Renderer>().enabled = false;
        List<Enemy> enemies = GameObject.Find("GameMaster").GetComponent<QuestManager>().GenerateEnemyList(q);
        Enemy myTarget = null;
        int enemiesDefeated = 0;

        //travel to the location
        advActivity = "Traveling...";
        float travelTime = (q.locationIndex + 1) * 12.0f;
        while (travelTime >= 0.0f)
        {
            travelTime -= Time.deltaTime;
            activityTime = travelTime;
            ActiveHeroPanel.Instance.UpdateHeroStats(this);
            yield return null;
        }

        myNeeds.SetNeedRates(-0.5f, -0.5f, 0.75f, -0.25f);

        advActivity = "Fighting!";
        activityTime = 0.0f;
        //fight the monsters
        while (currentQuest != null)
        {
            activityTime += Time.deltaTime;
            int tmpHP = HP;

            foreach (Enemy e in enemies)
                HP -= e.Attack(10 + agility + toughness);
                        
            if (tmpHP != HP)
                ChangeHeroHealthDisplay();
            
            if (myTarget == null)
            {
                myTarget = enemies[UnityEngine.Random.Range(0, enemies.Count)];
            }
            attackTimer -= Time.deltaTime;
            if (Attack(myTarget))
            {
                Debug.Log("Target Killed: " + myTarget.name);
                enemies.Remove(myTarget);
                exp += myTarget.level * 25;
                myTarget = null;
                enemiesDefeated++; 
            }

            if (enemies.Count == 0)
                CompleteQuest(q, enemiesDefeated);

            else if (HP < maxHP * .3)
            {
                Flee(q, enemiesDefeated, enemies);
            }
            ActiveHeroPanel.Instance.UpdateHeroStats(this);
            yield return null;
        }
        myNeeds.ResetRates();
        myState.ResetState();
    }

    public bool Attack(Enemy target)
    {
        if (attackTimer > 0.0f)
        {
            return false;
        }

        attackTimer = 2.0f;
        if (UnityEngine.Random.Range(1, 20) + agility >= target.armor)
        {
            if (target.ReduceHP(UnityEngine.Random.Range(minDamage, maxDamage)))
                return true;
        }

        return false;
    }

    public void CompleteQuest(Quest q, int numberDefeated)
    {
        Player p = GameObject.Find("PlayerTest").GetComponent<Player>();
        InventoryMaster.Instance.AddItem(q.objectiveIndex, numberDefeated);
        
        p.playerGold -= q.goldReward;
        gold += q.goldReward;
        this.gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.transform.position = new Vector3(-7, -4.4f, -0.1f);
        currentQuest = null;
        p.UpdateGoldDisplay();
        advActivity = "Relaxing..";
        activityTime = 0.0f;
        hasActivity = false;
    }

    public void Flee(Quest q, int numberDefeated, List<Enemy> remainingEnemies)
    {
        //each enemy gets a final attack as the hero flees
        foreach (Enemy e in remainingEnemies)
        {
            HP -= e.Attack(10 + agility + toughness);
        }

        CompleteQuest(q, numberDefeated);
    }

    IEnumerator MoveToBed()
    {
        GameObject temp = GameObject.Find("Bed");

        while (!atBed)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }

        StartCoroutine(Sleep());
    }

    IEnumerator Sleep()
    {
        float healTimer = 3.0f;
        while (HP < maxHP)
        {
            healTimer -= Time.deltaTime;
            if (healTimer <= 0.0f)
            {
                HP += toughness;
                ChangeHeroHealthDisplay();
                healTimer = 3.0f;
            }
            yield return null;
        }

        hasActivity = false;
    }

    IEnumerator MoveToQuest(Quest q)
    {
        GameObject temp = GameObject.Find("PlayerTest");
        while (!atQuest)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }
        StartCoroutine(RunQuest(q));
    }

    IEnumerator MoveToTable()
    {
        hasActivity = true;
        GameObject temp = GameObject.Find("Table");
        while (!atTable)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }

        StartCoroutine(Eat());
    }

    public void ChangeHeroHealthDisplay()
    {
        healthBar.rectTransform.localScale = new Vector3((float)HP / (float)maxHP, 1, 1);
    }

    IEnumerator Eat()
    {
        myNeeds.SetNeedRates(2.0f, 0.25f, -0.25f, -0.25f);
        while (myNeeds.foodNeed < 100.0f)
        {
            yield return null;
        }

        myNeeds.ResetRates();
        myState.ResetState();
        hasActivity = false;
    }

    public void LevelUp()
    {

    }
}