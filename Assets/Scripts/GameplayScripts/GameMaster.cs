using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance { get; private set; }

    [SerializeField]
    GameObject adventurerPrefab;
    List<Adventurer> deceasedAdventurers = new List<Adventurer>();
    List<Adventurer> inactiveAdventurers = new List<Adventurer>();
    public List<GameObject> RestObjects;
    public List<GameObject> QuestObjects;
    public List<GameObject> IdleObjects;
    public List<GameObject> TableObjects;
    public List<GameObject> DrinkObjects;
    public List<Sprite> enemySpriteList;

    public GameObject messagingTextPrefab;

    //Generic Display Elements
    public Text npcNameDisplay;
    Adventurer selectedAdventurer = null;
    [SerializeField]
    UIWindowController guiWindow;

    QuestManager questManager;
    public NPCNames names = new NPCNames();

    public int questsCompleted = 0;
    [SerializeField]
    int questsForNextLevel = 2;
    public int innRating = 7;

    float npcSpawnDelay;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i <= 2; i++)
        {
            SpawnNewHero();
        }
    }

    void Start()
    {
        npcSpawnDelay = UnityEngine.Random.Range(60.0f, 300.0f);

        questManager = GetComponent<QuestManager>();
        RestObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("RestItems"));
        QuestObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("QuestGiver"));
        IdleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("IdleActivity"));
        TableObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("TableItems"));
        DrinkObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("DrinkItem"));
    }

    void Update()
    {        
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
            }
            
            else
            {
                npcNameDisplay.text = "Character: None";
                selectedAdventurer = null;
            }
        }

        if (questsCompleted == questsForNextLevel)
        {
            innRating++;
            if (innRating % 2 == 0)
            {
                questManager.IncreaseQuestLevel();
                SendGameMessage("New Quest Options Available");
            }
            questsForNextLevel += (int)(questsForNextLevel * 1.1f);
        }

        //spawn more NPCs based on total inn rating.
        npcSpawnDelay -= Time.deltaTime;
        if (npcSpawnDelay <= 0.0f)
        {
            if (ActiveHeroPanel.Instance.NumberOfHeroesActive() < innRating / 2)
                SpawnNewHero();

            npcSpawnDelay = UnityEngine.Random.Range(60.0f, 180.0f);
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
        Vector3 position = adv.transform.localPosition;
        position.x += UnityEngine.Random.Range(-1.0f, 1.0f);
        position.y += UnityEngine.Random.Range(-1.0f, 1.0f);
        adv.transform.position = position;
        guiWindow.AddHeroToWindow(adv.GetComponent<Adventurer>());
        adv.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, .2f, .5f, 0.5f, 1f);
        SendGameMessage("New hero " + adv.GetComponent<AdventurerStats>().advName + " has arrived");
    }

    public void SendGameMessage(string message)
    {
        var temp = Instantiate(messagingTextPrefab);
        temp.transform.SetParent(GameObject.Find("PrimaryUI").transform, false);
        temp.GetComponent<Text>().text = message;
    }
}

public class NPCNames
{
    List<string> maleNames = new List<string> { "Malestrom", "Lassie", "Don MF Johnson", "Mattisen", "Arkonios", "Dojima", "Denae", "Sturm","Gerald",
                                            "Pyra", "Guntar", "Renahzor", "Snarky", "Kortice", "Ingvar","Vimak","Georg","Breunor","Ruben", "Aigis", "Wulfgar",
                                            "Jonathan","Theodin", "Brukhar", "Subs", "Jester", "Jeremu" };

    public string GetName()
    {
        int randomIndex = UnityEngine.Random.Range(0, maleNames.Count);
        if (maleNames.Count == 0)
            return "John Doe";

        string returnValue = maleNames[randomIndex];
        maleNames.Remove(maleNames[randomIndex]);
        return returnValue;
    }
}