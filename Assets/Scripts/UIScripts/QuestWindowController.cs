using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

//Class to control the display and change of information within the quest window
public class QuestWindowController : MonoBehaviour
{
    public QuestManager questManager;
    //Quest Window Display Elements for access to change display
    public Dropdown questObjectiveDropdown;
    public Dropdown questLocationDropdown;
    public InputField rewardInput;
    public QuestItemScript questHolder = null;

    int questLevel = 1;

    void Start()
    {
        UpdateDropdownText(questLocationDropdown, questManager.questLocationsList);
        UpdateDropdownText(questObjectiveDropdown, questManager.questObjectivesList);

        questLocationDropdown.value = -1;
        questLocationDropdown.captionText.text = "Select One...";

        questObjectiveDropdown.value = -1;
        questObjectiveDropdown.captionText.text = "Select One...";

    }

    void Update()
    {
        if(questLevel < questManager.currentLevel)
        {
            UpdateDropdownText(questLocationDropdown, questManager.questLocationsList);
            UpdateDropdownText(questObjectiveDropdown, questManager.questObjectivesList);
            questLevel = questManager.currentLevel;
        }
    }

    public void UpdateDropdownText(Dropdown d, List<string> list)
    {
        d.ClearOptions();
        d.AddOptions(list);
        d.RefreshShownValue();
    }

    public void CreateQuest()
    {

        if (questHolder == null)
        {
            Debug.Log("Quest Board Not present, something went very wrong...");
            return;
        }
        
        if (!questHolder.HasRoomForQuest())
        {
            GameMaster.Instance.SendGameMessage("Quest Board is full");
            return;
        }

        if (questObjectiveDropdown.value == -1 || questLocationDropdown.value == -1)
        {
            GameMaster.Instance.SendGameMessage("Fill in all options");
            return;
        }

        int x;
        if(Int32.TryParse(rewardInput.text, out x))
        {
            if (x <= 0)
                return;
        }

        ActiveQuestManager am = GameMaster.Instance.GetComponent<ActiveQuestManager>();

        Quest newQuest = new Quest(questLocationDropdown.value, questObjectiveDropdown.value, Int32.Parse(rewardInput.text.ToString()));

        if (am && !am.IsDuplicateQuest(newQuest))
        {
            am.AddQuest(newQuest);
            questHolder.AddQuest(newQuest);
        }

        else
        {
            GameMaster.Instance.SendGameMessage("Identical quest already exists");
            return;
        }
    }

    public void CloseWindow()
    {
        questHolder = null;
        this.gameObject.SetActive(false);
    }
}