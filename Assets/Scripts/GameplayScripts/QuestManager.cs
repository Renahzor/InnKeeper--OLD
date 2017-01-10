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
        int numberOfEnemies = Random.Range(2, 6);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            string enemyName = "Ogre ";
            if (q.objectiveIndex == 0)
                enemyName = "Rat ";
            tmp.Add(new Enemy(enemyName + i, (q.objectiveIndex + 1 * Random.Range(2, 5)), q.locationIndex + 1));
        }

        return tmp;
    }

    public void IncreaseQuestLevel()
    {
        currentLevel++;

        switch (currentLevel)
        {
            case 2:
                questLocationsList.Add("Old Mill");
                break;
            case 3:
                questObjectivesList.Add("Ogre Knuckles");
                GameObject.Find("GameMaster").GetComponent<OrderLedger>().AddNewOrder(new Order("Ogre Knuckles Wanted", "Bring us some Ogre Knuckles", 8, 23, 1));
                break;
            default:
                break;
        }
    }
}
