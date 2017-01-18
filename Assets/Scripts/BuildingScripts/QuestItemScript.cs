using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemScript : MonoBehaviour {
    [SerializeField]
    int numberOfQuests;
    [SerializeField]
    List<Quest> questsStored = new List<Quest>();

    public bool HasRoomForQuest()
    {
        return (questsStored.Count < numberOfQuests);
    }

    public bool ContainsQuest(Quest q)
    {
        return questsStored.Contains(q);
    }

    public void AddQuest(Quest q)
    {
        if (questsStored.Count < numberOfQuests)
            questsStored.Add(q);

        else Debug.Log("Cannot Hold More Quests");
    }

    public void RemoveQuest(Quest q)
    {
        if (questsStored.Contains(q))
            questsStored.Remove(q);
    }
    
}
