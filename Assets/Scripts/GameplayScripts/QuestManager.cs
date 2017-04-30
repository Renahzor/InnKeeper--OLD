using UnityEngine;
using System.Collections.Generic;

//Class for storage of quest options, generation of stats, etc
public class QuestManager : MonoBehaviour {

    public List<string> questLocationsList = new List<string>();
    public List<string> questObjectivesList = new List<string>();

    public int currentLevel = 1;

    void Awake()
    {
        questLocationsList.Add("Inn Basement");
        questObjectivesList.Add("Rat Tails");        
    }

    public string GetQuestDescription(int locationIndex, int objectiveIndex)
    {
        if (locationIndex == -1 || objectiveIndex == -1)
            return "Select Options Above";

        return "Send heroes to the " + questLocationsList[locationIndex] + " to retrieve some " + questObjectivesList[objectiveIndex];
    }

    public List<Enemy> GenerateEnemyList(Quest q)
    {
        List<Enemy> tmp = new List<Enemy>();
        int numberOfEnemies = Random.Range(2, (5 + (q.locationIndex + 1) / (q.objectiveIndex + 1) ));
        for (int i = 0; i < numberOfEnemies; i++)
        {
            string enemyName = "Goblin ";
            if (q.objectiveIndex == 0)
                enemyName = "Rat ";
            if (q.objectiveIndex == 2)
                enemyName = "Spider ";
            tmp.Add(new Enemy(enemyName + (i + 1), (q.objectiveIndex + 1 * Random.Range(2, 6)), q.locationIndex + 1, GameMaster.Instance.enemySpriteList[q.objectiveIndex]));
        }

        return tmp;
    }

    public void IncreaseQuestLevel()
    {
        currentLevel++;

        switch (currentLevel)
        {
            case 2:
                questLocationsList.Add("The Old Mill");
                break;
            case 3:
                questObjectivesList.Add("Goblin Ears");
                GameObject.Find("GameMaster").GetComponent<OrderLedger>().AddNewOrder(new Order("Goblin Ears", "The local magistrate is paying well for goblin ears.", 8, 23, 1));
                break;
            case 4:
                questLocationsList.Add("The Maple Thicket");
                break;
            case 5:
                questObjectivesList.Add("Spider Venom");
                GameObject.Find("GameMaster").GetComponent<OrderLedger>().AddNewOrder(new Order("Spider Venom", "A 'weaponsmith' in town needs spider venom for his craft", 10, 55, 2));
                break;
            default:
                break;
        }
    }
}
