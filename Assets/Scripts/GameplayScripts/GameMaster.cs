using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance { get; private set; }

    [SerializeField]
    GameObject adventurerPrefab, questWindow, inventoryWindow, activeAdventurerWindow, ledgerWindow, activeQuestWindow;
    List<Adventurer> deceasedAdventurers = new List<Adventurer>();

    //Generic Display Elements
    public Text npcNameDisplay;
    Adventurer selectedAdventurer = null;

    QuestManager questManager;

    public int questsCompleted = 0;
    public int innRating = 10;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i <= 5; i++)
        {
            var adv = Instantiate(adventurerPrefab);
            activeAdventurerWindow.GetComponent<ActiveHeroPanel>().AddHero(adv.GetComponent<Adventurer>());
            adv.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }

    void Start()
    {
        questWindow.SetActive(false);
        questManager = GetComponent<QuestManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown("q"))
            questWindow.SetActive(!questWindow.activeSelf);
        
        if (Input.GetKeyDown("i"))
            inventoryWindow.SetActive(!inventoryWindow.activeSelf);

        if (Input.GetKeyDown("h"))
            activeAdventurerWindow.SetActive(!activeAdventurerWindow.activeSelf);

        if (Input.GetKeyDown("l"))
            ledgerWindow.SetActive(!ledgerWindow.activeSelf);

        if (Input.GetKeyDown("a"))
            activeQuestWindow.SetActive(!activeQuestWindow.activeSelf);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "NPC")
                {
                    selectedAdventurer = hit.transform.GetComponent<Adventurer>();
                    npcNameDisplay.text = "Character: " + selectedAdventurer.gameObject.GetComponent<AdventurerStats>().advName;
                }
            }

            else
            {
                npcNameDisplay.text = "Character: None";
                selectedAdventurer = null;
            }
        }

        if (questsCompleted == 10)
        {
            questManager.IncreaseQuestLevel();
        }
    }

    public void KillAdventurer(Adventurer a)
    {
        deceasedAdventurers.Add(a);
        ActiveHeroPanel.Instance.RemoveHero(a);
    }
}