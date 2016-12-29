﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    public GameObject adventurerPrefab;
    public GameObject questWindow;
    public GameObject inventoryWindow;
    public GameObject activeAdventurerWindow;
    public GameObject ledgerWindow;
    public GameObject activeQuestWindow;

    //Generic Display Elements
    public Text npcNameDisplay;
    Adventurer selectedAdventurer = null;

    public int questsCompleted = 0;

    void Awake()
    {
        var adv = Instantiate(adventurerPrefab);
        activeAdventurerWindow.GetComponent<ActiveHeroPanel>().AddHero(adv.GetComponent<Adventurer>());
    }

    void Start()
    {
        questWindow.SetActive(false);
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
                    npcNameDisplay.text = "Character: " + selectedAdventurer.advName;
                }
            }

            else
            {
                npcNameDisplay.text = "Character: None";
                selectedAdventurer = null;
            }
        }
    }
}
