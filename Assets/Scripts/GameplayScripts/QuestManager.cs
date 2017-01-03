using UnityEngine;
using System.Collections.Generic;

//Class for storage of quest options, generation of stats, etc
public class QuestManager : MonoBehaviour {

    public List<string> questLocationsList = new List<string>();
    public List<string> questObjectivesList = new List<string>();

    void Awake()
    {
        questLocationsList.Add("Inn Basement");
        //questLocationsList.Add("Old Mill");

        questObjectivesList.Add("Rat Tails");
        //questObjectivesList.Add("Ogre Knuckles");
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
            tmp.Add(new Enemy("Rat " + i, (q.objectiveIndex + 1 * Random.Range(2,5)), q.locationIndex + 1));

        return tmp;
    }
}
