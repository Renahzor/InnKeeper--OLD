using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

//AI behavior routines.
public class NPCBehaviors : MonoBehaviour
{

    public enum InteractableObjects { Bed, QuestMarker, Exit, Bar, Table, IdleActivity }

    //Store health bar so we dont have to "find" it multiple times
    public Image healthBar = null;
    public GameObject quickUIHero;
    public GameObject quickUIEnemy;

    StateTracker state;
    AdventurerNeeds needs;
    AdventurerStats stats;

    void Start()
    {
        state = GetComponent<StateTracker>();
        needs = GetComponent<AdventurerNeeds>();
        stats = GetComponent<AdventurerStats>();
    }

    //Routine to select and accept a quest
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
            StartCoroutine(MoveTo(InteractableObjects.QuestMarker, state.currentQuest));
        }

        return;
    }

    //routine to leave the Inn, travel to and run quests
    IEnumerator RunQuest(Quest q)
    {
        var myQuickUI = Instantiate(quickUIHero);
        myQuickUI.transform.SetParent(GameObject.Find("quicktrackui").transform, false);
        myQuickUI.GetComponentInChildren<Text>().text = stats.advName;

        state.hasActivity = true;

        List<Enemy> enemies = GameObject.Find("GameMaster").GetComponent<QuestManager>().GenerateEnemyList(q);
        Enemy myTarget = null;
        int enemyIndex = -1;

        int enemiesDefeated = 0;

        //travel to the location
        state.advActivity = "Traveling to " + GameObject.Find("GameMaster").GetComponent<QuestManager>().questLocationsList[q.locationIndex];
        float travelTime = (q.locationIndex + 1) * 12.0f;

        GameObject temp = GameObject.Find("ExitPath");

        while (state.objectCurrentlyTouching != temp)
        {
            this.GetComponent<Movement>().MoveTowardTarget(temp.transform);
            yield return null;
        }

        this.gameObject.GetComponent<Renderer>().enabled = false;
        needs.SetNeedRates(-1.0f, -0.15f, 0.75f, -0.25f);

        while (travelTime >= 0.0f)
        {
            travelTime -= Time.deltaTime;
            state.activityTime = travelTime;
            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
            yield return null;
        }

        List<GameObject> enemyUIList = new List<GameObject>();
        foreach (Enemy e in enemies)
        {
            var enemyQuickImage = Instantiate(quickUIEnemy);
            enemyQuickImage.transform.SetParent(myQuickUI.transform, false);
            enemyUIList.Add(enemyQuickImage);
            enemyQuickImage.GetComponent<Image>().sprite = e.mySprite;
        }

        needs.SetNeedRates(-1.0f, -0.15f, 2.0f, -0.25f);

        state.advActivity = "Fighting!";
        state.activityTime = 0.0f;

        //fight the monsters while there are monsters to fight
        while (enemies.Count > 0)
        {
            state.activityTime += Time.deltaTime;
            int tmpHP = stats.HP;

            foreach (Enemy e in enemies)
            {
                int HPReduction = e.Attack(10 + stats.agility + stats.toughness, false);
                stats.HP -= HPReduction;
                if (HPReduction > 0)
                {
                    if (e.critLastHit)
                        GameMaster.Instance.GetComponent<AttackHandler>().RecieveAttack(-HPReduction, myQuickUI.transform, true, false);
                    else
                        GameMaster.Instance.GetComponent<AttackHandler>().RecieveAttack(-HPReduction, myQuickUI.transform, false, false);
                }

            }

            if (tmpHP != stats.HP)
                ChangeHeroHealthDisplay();

            if (myTarget == null && enemies.Count > 0)
            {
                enemyIndex = Random.Range(0, enemies.Count);
                myTarget = enemies[enemyIndex];
                state.advActivity = "Fighting " + myTarget.name + "...";
            }

            state.attackTimer -= Time.deltaTime;
            if (Attack(myTarget, enemyUIList[enemyIndex].transform))
            {
                state.advActivity = "Target Killed: " + myTarget.name;
                stats.AddEXP(myTarget.level * 25);
                enemies.Remove(myTarget);
                myTarget = null;
                enemiesDefeated++;
                Destroy(enemyUIList[enemyIndex], 1.1f);
                enemyUIList.Remove(enemyUIList[enemyIndex]);
            }

            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());

            //This section is for ending a quest, either by completion or by fleeing
            if (enemies.Count == 0)
            {
                travelTime = (q.locationIndex + 1) * 12.0f;
                state.advActivity = "Returning...";
                while (travelTime >= 0.0f)
                {
                    travelTime -= Time.deltaTime;
                    state.activityTime = travelTime;
                    ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
                    yield return null;
                }
                CompleteQuest(q, enemiesDefeated, enemies);
                Destroy(myQuickUI, 3.0f);
            }

            else if (stats.HP < stats.maxHP * .3)
            {
                var tempEnemies = new List<Enemy>(enemies);
                enemies.Clear();
                Flee(q, enemiesDefeated, tempEnemies, myQuickUI);

                if (stats.HP <= 0)
                {
                    CompleteQuest(q, enemiesDefeated, tempEnemies);
                    Destroy(myQuickUI, 3.0f);
                }

                else
                {
                    travelTime = (q.locationIndex + 1) * 12.0f;
                    state.advActivity = "Fleeing...";
                    while (travelTime >= 0.0f)
                    {
                        travelTime -= Time.deltaTime;
                        state.activityTime = travelTime;
                        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
                        yield return null;
                    }
                    CompleteQuest(q, enemiesDefeated, tempEnemies);
                }
            }
            yield return null;
        }
        Destroy(myQuickUI, 3.0f);
        needs.ResetRates();
        state.ResetState();
    }

    //Helper method for attacking an enemy, math for damage and hit chance may change at a later time
    public bool Attack(Enemy target, Transform enemyQuickUITransform)
    {
        if (state.attackTimer > 0.0f)
        {
            return false;
        }

        state.attackTimer = 2.0f;

        int attackRoll = UnityEngine.Random.Range(1, 21) + stats.agility;

        if (attackRoll >= target.armor)
        {
            //Set attack damage multiplier if the enemy is weak to this type
            float attackMultiplier = 1.0f;
            if (target.weaknessType == stats.attackType)
                attackMultiplier = 1.5f;

            int damage = UnityEngine.Random.Range((int)(stats.minDamage * attackMultiplier), (int)(stats.maxDamage * attackMultiplier));

            bool critical = false;
            if (attackRoll - stats.agility == 20)
            {
                damage *= 2;
                critical = true;
            }

            GameMaster.Instance.GetComponent<AttackHandler>().RecieveAttack(-damage, enemyQuickUITransform.transform, critical, false);

            if (target.ReduceHP(damage))
            {
                return true;
            }
        }

        return false;
    }

    //when a hero decides to flee, all enemies get one extra attack
    void Flee(Quest q, int numberDefeated, List<Enemy> remainingEnemies, GameObject uiParent)
    {
        //each enemy gets a final attack as the hero flees
        foreach (Enemy e in remainingEnemies)
        {
            int HPReduction = e.Attack(10 + stats.agility + stats.toughness, true);
            stats.HP -= HPReduction;
            if (HPReduction > 0)
            {
                if (e.critLastHit)
                    GameMaster.Instance.GetComponent<AttackHandler>().RecieveAttack(-HPReduction, uiParent.transform, true, false);
                else
                    GameMaster.Instance.GetComponent<AttackHandler>().RecieveAttack(-HPReduction, uiParent.transform, false, false);
            }
        }
        GameMaster.Instance.questsCompleted--;
    }

    //moveto routine for different objects within the inn
    public IEnumerator MoveTo(InteractableObjects target, Quest q = null)
    {
        GameObject targetSelected = null;
        List<GameObject> possibleTargets = new List<GameObject>();
        state.hasActivity = true;

        //check what kind of target we're looking for and narrow down out targets
        switch (target)
        {
            case InteractableObjects.Bed:
                possibleTargets = new List<GameObject>(GameMaster.Instance.RestObjects);
                for (int i = possibleTargets.Count - 1; i >= 0; i--)
                {
                    BedScript bed = possibleTargets[i].GetComponent<BedScript>();
                    if (bed.occupied == true || bed.goldCost > stats.gold)
                    {
                        possibleTargets.Remove(possibleTargets[i]);
                    }
                }
                if (possibleTargets.Count == 0)
                {
                    Debug.Log("Out of Beds or available beds too expensive for Hero " + stats.advName);
                    state.ResetState();
                    state.hasActivity = false;
                    yield break;
                }
                break;
            case InteractableObjects.Exit:
                possibleTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag("ExitPath"));
                break;
            case InteractableObjects.QuestMarker:
                possibleTargets = new List<GameObject>(GameMaster.Instance.QuestObjects);
                for (int i = possibleTargets.Count - 1; i >= 0; i--)
                {
                    if (!possibleTargets[i].GetComponent<QuestItemScript>().ContainsQuest(q))
                        possibleTargets.Remove(possibleTargets[i]);
                }

                if (possibleTargets.Count == 0)
                {
                    state.ResetState();
                    state.advActivity = "None of these quests pay enough!";
                    state.hasActivity = false;
                    yield break;
                }
                break;
            case InteractableObjects.Bar:
                possibleTargets = new List<GameObject>(GameMaster.Instance.DrinkObjects);
                break;
            case InteractableObjects.IdleActivity:
                possibleTargets = new List<GameObject>(GameMaster.Instance.IdleObjects);
                break;
            case InteractableObjects.Table:
                possibleTargets = new List<GameObject>(GameMaster.Instance.TableObjects);
                break;
            default:
                break;
        }

        targetSelected = SelectTrarget(possibleTargets, target);

        if (targetSelected != null)
        {
            while (state.objectCurrentlyTouching != targetSelected)
            {
                this.GetComponent<Movement>().MoveTowardTarget(targetSelected.transform);
                yield return null;
            }
        }

        else { Debug.Log("No Target of type selected: MoveTo() failed"); }

        //after selecting a specific target and moving to it, start the proper coroutine for that type of object
        switch (target)
        {
            case InteractableObjects.Bed:
                StartCoroutine(Sleep(targetSelected));
                break;
            case InteractableObjects.Table:
                StartCoroutine(Eat());
                break;
            case InteractableObjects.Bar:
                StartCoroutine(Drink());
                break;
            case InteractableObjects.QuestMarker:
                if (q != null)
                    StartCoroutine(RunQuest(q));
                break;
            case InteractableObjects.IdleActivity:
                needs.SetNeedRates(-0.1f, -0.05f, -0.8f, 1.5f);
                StartCoroutine(IdleActivity(targetSelected.GetComponent<IdleActivityScript>()));
                break;
            default: break;
        }
        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
    }

    //Trims a list of targets based on specific target types
    GameObject SelectTrarget(List<GameObject> possibleTargets, InteractableObjects intendedTarget)
    {
        if (possibleTargets != null)
        {
            int randomIndex = Random.Range(0, possibleTargets.Count);
            if (possibleTargets[randomIndex].GetComponent<BedScript>())
            {
                possibleTargets[randomIndex].GetComponent<BedScript>().occupied = true;
            }
            return possibleTargets[randomIndex];
        }

        else return null;
    }

    void ChangeHeroHealthDisplay()
    {
        healthBar.rectTransform.localScale = new Vector3((float)stats.HP / (float)stats.maxHP, 1, 1);
    }

    //activity routines
    IEnumerator IdleActivity(IdleActivityScript i)
    {
        state.hasActivity = true;
        float timeRemaining = i.activityDuration;
        state.advActivity = i.activityName;
        while (timeRemaining > 0.0f)
        {
            timeRemaining -= Time.deltaTime;
            state.activityTime = timeRemaining;
            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
            yield return null;
        }
        state.hasActivity = false;
        needs.ResetRates();
        state.ResetState();
        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
        GetComponent<Adventurer>().CheckStateImmediate();
    }

    IEnumerator Sleep(GameObject bed)
    {
        state.advActivity = "Sleeping";
        state.activityTime = 0.0f;
        BedScript temp = bed.GetComponent<BedScript>();
        stats.gold -= temp.goldCost;
        Player.Instance.playerGold += temp.goldCost;
        Player.Instance.UpdateGoldDisplay();
        Debug.Log("Paid for bed");
        float healTimer = 60 / temp.sleepRate;
        while (stats.HP < stats.maxHP)
        {
            healTimer -= Time.deltaTime;
            if (healTimer <= 0.0f)
            {
                stats.HP = Mathf.Clamp(stats.HP += stats.toughness, 0, stats.maxHP);
                ChangeHeroHealthDisplay();
                healTimer = 60 / temp.sleepRate;
            }
            yield return null;
        }
        state.ResetState();
        state.hasActivity = false;
        temp.occupied = false;
        GetComponent<Adventurer>().CheckStateImmediate();
        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
    }

    IEnumerator Eat()
    {
        state.advActivity = "Eating";
        state.activityTime = 0.0f;
        needs.SetNeedRates(1.0f, 0.4f, -0.2f, -0.2f);
        while (needs.foodNeed < 100.0f)
        {
            yield return null;
        }

        needs.ResetRates();
        state.ResetState();
        state.hasActivity = false;
        GetComponent<Adventurer>().CheckStateImmediate();
        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
    }

    IEnumerator Drink()
    {
        state.advActivity = "Gettin' Tipsy";
        state.activityTime = 0.0f;
        needs.SetNeedRates(0.25f, 1.0f, -0.7f, 0.3f);
        while (needs.drinkNeed < 100.0f)
        {
            yield return null;
        }

        needs.ResetRates();
        state.ResetState();
        state.hasActivity = false;
        GetComponent<Adventurer>().CheckStateImmediate();
        ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
    }

    void CompleteQuest(Quest q, int numberDefeated, List<Enemy>enemiesRemaining)
    {
        if (stats.HP <= 0)
        {
            string enemyKilledBy;
            if (enemiesRemaining.Count > 0)
                enemyKilledBy = enemiesRemaining[Random.Range(0, enemiesRemaining.Count)].name;
            else
                enemyKilledBy = "hubris";
            GameMaster.Instance.SendGameMessage("Hero " + stats.advName + " has been killed by " + enemyKilledBy);
            KillNPC();
        }

        else
        {
            InventoryMaster.Instance.AddItem(q.objectiveIndex, numberDefeated);

            if (numberDefeated > 0)
            {
                Player.Instance.playerGold -= q.goldReward;
                stats.gold += q.goldReward;
            }
            this.gameObject.GetComponent<Renderer>().enabled = true;

            //spawn point
            gameObject.transform.position = new Vector3(-7, -4.4f, -0.05f);

            state.currentQuest = null;
            Player.Instance.UpdateGoldDisplay();
            state.advActivity = "Relaxing..";
            state.activityTime = 0.0f;
            state.hasActivity = false;
            GameMaster.Instance.questsCompleted++;
            ActiveHeroPanel.Instance.UpdateHeroStats(this.GetComponent<Adventurer>());
        }
    }

    //state modification for colliders, detects when NPCs are at specific interactable objects.
    void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.tag != "NPC")
            state.objectCurrentlyTouching = otherCollider.gameObject;
    }

    void OnCollisionExit2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.tag != "NPC")
            state.objectCurrentlyTouching = null;
    }

    void KillNPC()
    {
        GameMaster.Instance.KillAdventurer(gameObject.GetComponent<Adventurer>());
        GameObject.Destroy(this.gameObject);
    }
}
