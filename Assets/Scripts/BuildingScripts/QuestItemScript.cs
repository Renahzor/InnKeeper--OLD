using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemScript : MonoBehaviour {
    [SerializeField]
    int numberOfQuests;
    [SerializeField]
    List<Quest> questsStored = new List<Quest>();
    [SerializeField]
    Text questCountText;

    void Awake()
    {
        questCountText = GetComponentInChildren<Text>();
    }

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

        else GameMaster.Instance.SendGameMessage("Quest Board is full, build a new one");
        questCountText.text = questsStored.Count + "/" + numberOfQuests;
    }

    public void RemoveQuest(Quest q)
    {
        if (questsStored.Contains(q))
            questsStored.Remove(q);
        questCountText.text = questsStored.Count + "/" + numberOfQuests;
    }
    
}
