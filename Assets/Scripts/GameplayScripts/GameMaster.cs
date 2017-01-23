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
    public List<GameObject> RestObjects;
    public List<GameObject> QuestObjects;
    public List<GameObject> IdleObjects;
    public List<GameObject> TableObjects;

    //Generic Display Elements
    public Text npcNameDisplay;
    Adventurer selectedAdventurer = null;

    QuestManager questManager;
    public NPCNames names = new NPCNames();

    public int questsCompleted = 0;
    int questsForNextLevel = 10;
    public int innRating = 20;

    float npcSpawnDelay;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i <= 3; i++)
        {
            SpawnNewHero();
        }
    }

    void Start()
    {
        npcSpawnDelay = UnityEngine.Random.Range(60.0f, 300.0f);

        questWindow.SetActive(false);
        questManager = GetComponent<QuestManager>();
        RestObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("RestItems"));
        QuestObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("QuestGiver"));
        IdleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("IdleActivity"));
        TableObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("TableItems"));
    }

    void Update()
    {        
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
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.transform.tag == "NPC")
                {
                    selectedAdventurer = hit.transform.GetComponent<Adventurer>();
                    npcNameDisplay.text = "Character: " + selectedAdventurer.gameObject.GetComponent<AdventurerStats>().advName;
                }

                else if (hit.transform.tag == "QuestGiver")
                {
                    questWindow.SetActive(true);
                    questWindow.GetComponent<QuestWindowController>().questHolder = hit.transform.GetComponent<QuestItemScript>();
                }
            }
            
            else
            {
                npcNameDisplay.text = "Character: None";
                selectedAdventurer = null;
            }
        }

        if (questsCompleted == questsForNextLevel)
        {
            questManager.IncreaseQuestLevel();
            questsForNextLevel *= 2;
        }

        //spawn more NPCs based on total inn rating.
        npcSpawnDelay -= Time.deltaTime;
        if (npcSpawnDelay <= 0.0f)
        {
            if (ActiveHeroPanel.Instance.NumberOfHeroesActive() < innRating / 3)
                SpawnNewHero();

            npcSpawnDelay = UnityEngine.Random.Range(60.0f, 300.0f);
        }
    }

    public void KillAdventurer(Adventurer a)
    {
        deceasedAdventurers.Add(a);
        ActiveHeroPanel.Instance.RemoveHero(a);
    }

    public void SpawnNewHero()
    {
        var adv = Instantiate(adventurerPrefab);
        activeAdventurerWindow.GetComponent<ActiveHeroPanel>().AddHero(adv.GetComponent<Adventurer>());
        adv.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, .2f, .5f, 0.5f, 1f);
    }
}

public class NPCNames
{
    string[] firstNamesMale = new string[] { "Malestrom", "Lassie", "Mattisen", "Arkonios", "Dojima", "Denae", "Sturm","Gerald",
                                            "Pyra", "Guntar", "Renahzor", "Snarky", "Kortice", "Ingvar","Vimak","Georg","Breunor","Ruben", "Aigis", "Wulfgar",
                                            "Jonathan","Theodin", "Brukhar"};

    public string GetName()
    {
        return firstNamesMale[UnityEngine.Random.Range(0, firstNamesMale.Length)];
    }
}