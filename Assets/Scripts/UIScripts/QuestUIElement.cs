using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIElement : MonoBehaviour {

    public Quest storedQuest { get; private set; }

    public void SetQuest(Quest q)
    {
        storedQuest = q;
    }

    public void DeleteQuest()
    {
        if (Player.Instance.playerGold < 5)
        {
            GameMaster.Instance.SendGameMessage("Requires 5 Gold");
            return;
        }

        ActiveQuestManager am = GameMaster.Instance.GetComponent<ActiveQuestManager>();
        am.RemoveTrackedQuest(storedQuest);
        foreach (GameObject questBoard in GameMaster.Instance.QuestObjects)
        {
            if (questBoard.GetComponent<QuestItemScript>().ContainsQuest(storedQuest))
            {
                questBoard.GetComponent<QuestItemScript>().RemoveQuest(storedQuest);
            }
        }
        Player.Instance.playerGold -= 5;
        Player.Instance.UpdateGoldDisplay();
        GameMaster.Instance.SendGameMessage("-5 Gold");
        Destroy(gameObject);
    }
}
