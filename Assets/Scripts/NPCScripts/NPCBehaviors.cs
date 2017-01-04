using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

//move AI behaviors here
public class NPCBehaviors : MonoBehaviour {

    //Store health bar so we dont have to "find" it multiple times
    public Image healthBar = null;

    StateTracker state;
    AdventurerNeeds needs;
    AdventurerStats stats;

    void Start()
    {
        state = GetComponent<StateTracker>();
        needs = GetComponent<AdventurerNeeds>();
        stats = GetComponent<AdventurerStats>();
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
            state.currentQuest = desireableQuests[UnityEngine.Random.Range(0, desireableQuests.Count)];
            state.hasActivity = true;
            StartCoroutine(MoveToQuest(state.currentQuest));
        }

        return;
    }

    IEnumerator RunQuest(Quest q)
    {
        state.hasActivity = true;

        List<Enemy> enemies = GameObject.Find("GameMaster").GetComponent<QuestManager>().GenerateEnemyList(q);
        Enemy myTarget = null;
        int enemiesDefeated = 0;

        //travel to the location
        state.advActivity = "Traveling...";
        float travelTime = (q.locationIndex + 1) * 12.0f;

        GameObject temp = GameObject.Find("ExitPath");

        while (!state.atExit)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }

        this.gameObject.GetComponent<Renderer>().enabled = false;

        while (travelTime >= 0.0f)
        {
            travelTime -= Time.deltaTime;
            state.activityTime = travelTime;
            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
            yield return null;
        }

        needs.SetNeedRates(-0.5f, -0.5f, 0.75f, -0.25f);

        state.advActivity = "Fighting!";
        state.activityTime = 0.0f;
        //fight the monsters
        while (enemies.Count > 0)
        {
            state.activityTime += Time.deltaTime;
            int tmpHP = stats.HP;

            foreach (Enemy e in enemies)
                stats.HP -= e.Attack(10 + stats.agility + stats.toughness);

            if (tmpHP != stats.HP)
                ChangeHeroHealthDisplay();

            if (myTarget == null && enemies.Count > 0)
            {
                myTarget = enemies[Random.Range(0, enemies.Count)];
            }

            state.attackTimer -= Time.deltaTime;
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

            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
            yield return null;
        }
        needs.ResetRates();
        state.ResetState();
    }

    public bool Attack(Enemy target)
    {
        if (state.attackTimer > 0.0f)
        {
            return false;
        }

        state.attackTimer = 2.0f;
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

    public IEnumerator MoveToBed()
    {
        GameObject temp = GameObject.Find("Bed");

        while (!state.atBed)
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
                stats.HP = Mathf.Clamp(stats.HP += stats.toughness, 0, stats.maxHP);
                ChangeHeroHealthDisplay();
                healTimer = 3.0f;
            }
            yield return null;
        }

        state.hasActivity = false;
    }

    IEnumerator MoveToQuest(Quest q)
    {
        GameObject temp = GameObject.Find("PlayerTest");
        while (!state.atQuest)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }
        StartCoroutine(RunQuest(q));
    }

    public IEnumerator MoveToTable()
    {
        state.hasActivity = true;
        GameObject temp = GameObject.Find("Table");
        while (!state.atTable)
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
        state.hasActivity = false;
    }

    public void CompleteQuest(Quest q, int numberDefeated)
    {
        Player p = GameObject.Find("PlayerTest").GetComponent<Player>();
        InventoryMaster.Instance.AddItem(q.objectiveIndex, numberDefeated);

        p.playerGold -= q.goldReward;
        stats.gold += q.goldReward;
        this.gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.transform.position = new Vector3(-7, -4.4f, -0.1f);
        state.currentQuest = null;
        p.UpdateGoldDisplay();
        state.advActivity = "Relaxing..";
        state.activityTime = 0.0f;
        state.hasActivity = false;
        GameMaster.Instance.questsCompleted++;
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            state.atBed = true;
        else if (otherCollider.tag == "TableItems")
            state.atTable = true;
        else if (otherCollider.tag == "QuestGiver")
            state.atQuest = true;
        else if (otherCollider.tag == "Exit")
            state.atExit = true;
    }

    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "RestItems")
            state.atBed = false;
        else if (otherCollider.tag == "TableItems")
            state.atTable = false;
        else if (otherCollider.tag == "QuestGiver")
            state.atQuest = false;
        else if (otherCollider.tag == "Exit")
            state.atExit = false;
    }

}
