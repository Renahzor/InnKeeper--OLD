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

    int questLevel = 1;

    void Start()
    {
        UpdateDropdownText(questLocationDropdown, questManager.questLocationsList);
        UpdateDropdownText(questObjectiveDropdown, questManager.questObjectivesList);
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
        d.captionText.text = "Select One...";
    }

    public void CreateQuest()
    {
        if (questObjectiveDropdown.value == -1 || questLocationDropdown.value == -1)
        {
            Debug.Log("SELECT OPTIONS");
            return;
        }

        int x;
        if(Int32.TryParse(rewardInput.text, out x))
        {
            if (x <= 0)
                return;
        }

        ActiveQuestManager am = GameObject.Find("GameMaster").GetComponent<ActiveQuestManager>();

        if (am)
        {
            am.AddQuest(new Quest(questLocationDropdown.value, questObjectiveDropdown.value, Int32.Parse(rewardInput.text.ToString())));
        }
    }
}