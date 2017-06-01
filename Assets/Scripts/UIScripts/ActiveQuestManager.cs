using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActiveQuestManager : MonoBehaviour {

    public Text questTextPrefab;
    public GameObject questTimerWindow;
    public Dictionary<Quest, Text> activeQuests = new Dictionary<Quest, Text>();
	
    public void AddQuest(Quest q)
    {
        QuestManager qm = gameObject.GetComponent<QuestManager>();

        Text t = Instantiate(questTextPrefab) as Text;
        t.transform.SetParent(questTimerWindow.transform, false);

        t.text = "Objective: " + qm.questObjectivesList[q.objectiveIndex].ToString() + "    Location: " + qm.questLocationsList[q.locationIndex].ToString() + "    Reward: " +
                  q.goldReward.ToString() + " ";

        activeQuests.Add(q, t);

        t.GetComponent<QuestUIElement>().SetQuest(q);
    }

    public bool IsDuplicateQuest(Quest q)
    {
        foreach (Quest temp in activeQuests.Keys)
            if (temp.locationIndex == q.locationIndex && temp.objectiveIndex == q.objectiveIndex && temp.goldReward == q.goldReward)
            {
                return true;
            }

        return false;
    }

    public void RemoveTrackedQuest(Quest q)
    {
        if (activeQuests.ContainsKey(q))
        {
            activeQuests.Remove(q);
        }

        else
            Debug.Log("Quest does not exist. This should never happen");
    }
}