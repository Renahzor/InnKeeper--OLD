
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    float attackTimer = 1.0f;
    public string advActivity = "Relaxing...";
    public float activityTime = 0.0f;

    Quest currentQuest = null;

    float stateCheckCooldown = 5.0f;

    //state tracking
    public bool atBed, atTable, atQuest, atExit, wantsQuest, hasActivity = false;

    //Store health bar so we dont have to "find" it multiple times
    public Image healthBar = null;

    StateTracker state;
    NPCBehaviors behaviors;
    AdventurerNeeds needs;
    AdventurerStats stats;

    //creates a new random adventurer
    void Awake()
    {
        NewAdventurer(1);
    }

    void NewAdventurer(int _level)
    {
        stats = GetComponent<AdventurerStats>();
        stats.SetStats(_level);
        state = new StateTracker();
        behaviors = GetComponent<NPCBehaviors>();
        needs = GetComponent<AdventurerNeeds>();
    }

    void Update()
    {
        stateCheckCooldown -= Time.deltaTime;

        if (stateCheckCooldown <= 0.0f)
        {
            state.UpdateState(GetComponent<AdventurerNeeds>());
            stateCheckCooldown = 5.0f;
        }

        //check for health fist
        if (stats.HP < stats.maxHP * 0.8 && !hasActivity)
        {
            hasActivity = true;
            StartCoroutine(MoveToBed());
        }

        //if no activity, find an activity that interests the npc
        if (!hasActivity)
        {
            switch(state.GetCurrentState())
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

    public void GetQuest()
    {
        List<Quest> desireableQuests = new List<Quest>();

        ActiveQuestManager aqm = GameObject.Find("GameMaster").GetComponent<ActiveQuestManager>();

        foreach (Quest q in aqm.activeQuests.Keys)
        {
            if (q.locationIndex + q.locationIndex <= stats.level && q.goldReward >= stats.level)
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

        List<Enemy> enemies = GameObject.Find("GameMaster").GetComponent<QuestManager>().GenerateEnemyList(q);
        Enemy myTarget = null;
        int enemiesDefeated = 0;

        //travel to the location
        advActivity = "Traveling...";
        float travelTime = (q.locationIndex + 1) * 12.0f;

        GameObject temp = GameObject.Find("ExitPath");

        while (!atExit)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }

        this.gameObject.GetComponent<Renderer>().enabled = false;

        while (travelTime >= 0.0f)
        {
            travelTime -= Time.deltaTime;
            activityTime = travelTime;
            ActiveHeroPanel.Instance.UpdateHeroStats(this);
            yield return null;
        }

        needs.SetNeedRates(-0.5f, -0.5f, 0.75f, -0.25f);

        advActivity = "Fighting!";
        activityTime = 0.0f;
        //fight the monsters
        while (enemies.Count > 0)
        {
            activityTime += Time.deltaTime;
            int tmpHP = stats.HP;

            foreach (Enemy e in enemies)
                stats.HP -= e.Attack(10 + stats.agility + stats.toughness);
                        
            if (tmpHP != stats.HP)
                ChangeHeroHealthDisplay();

            if (myTarget == null && enemies.Count > 0)
            {
                myTarget = enemies[Random.Range(0, enemies.Count)];
            }

            attackTimer -= Time.deltaTime;
            if (Attack(myTarget))
            {
                Debug.Log("Target Killed: " + myTarget.name);
                stats.AddEXP(myTarget.level * 25);
                enemies.Remove(myTarget);
                myTarget = null;
                enemiesDefeated++; 
            }

            if (enemies.Count == 0)
                CompleteQuest(q, enemiesDefeated);

            else if (stats.HP < stats.maxHP * .3)
            {
                Flee(q, enemiesDefeated, enemies);
            }

            ActiveHeroPanel.Instance.UpdateHeroStats(this);
            yield return null;
        }
        needs.ResetRates();
        state.ResetState();
    }

    public bool Attack(Enemy target)
    {
        if (attackTimer > 0.0f)
        {
            return false;
        }

        attackTimer = 2.0f;
        if (UnityEngine.Random.Range(1, 20) + stats.agility >= target.armor)
        {
            if (target.ReduceHP(UnityEngine.Random.Range(stats.minDamage + stats.strength, stats.maxDamage + stats.strength)))
                return true;
        }

        return false;
    }

    public void Flee(Quest q, int numberDefeated, List<Enemy> remainingEnemies)
    {
        //each enemy gets a final attack as the hero flees
        foreach (Enemy e in remainingEnemies)
        {
            stats.HP -= e.Attack(10 + stats.agility + stats.toughness);
        }
        remainingEnemies.Clear();

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
        while (stats.HP < stats.maxHP)
        {
            healTimer -= Time.deltaTime;
            if (healTimer <= 0.0f)
            {
                stats.HP += stats.toughness;
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
        healthBar.rectTransform.localScale = new Vector3((float)stats.HP / (float)stats.maxHP, 1, 1);
    }

    IEnumerator Eat()
    {
        needs.SetNeedRates(2.0f, 0.25f, -0.25f, -0.25f);
        while (needs.foodNeed < 100.0f)
        {
            yield return null;
        }

        needs.ResetRates();
        state.ResetState();
        hasActivity = false;
    }

    public void LevelUp()
    {

    }

    public void CompleteQuest(Quest q, int numberDefeated)
    {
        Player p = GameObject.Find("PlayerTest").GetComponent<Player>();
        InventoryMaster.Instance.AddItem(q.objectiveIndex, numberDefeated);

        p.playerGold -= q.goldReward;
        stats.gold += q.goldReward;
        this.gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.transform.position = new Vector3(-7, -4.4f, -0.1f);
        currentQuest = null;
        p.UpdateGoldDisplay();
        advActivity = "Relaxing..";
        activityTime = 0.0f;
        hasActivity = false;
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            atBed = true;
        else if (otherCollider.tag == "TableItems")
            atTable = true;
        else if (otherCollider.tag == "QuestGiver")
            atQuest = true;
        else if (otherCollider.tag == "Exit")
            atExit = true;

    }

    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            atBed = false;
        else if (otherCollider.tag == "TableItems")
            atTable = false;
        else if (otherCollider.tag == "QuestGiver")
            atQuest = false;
        else if (otherCollider.tag == "Exit")
            atExit = false;
    }
}