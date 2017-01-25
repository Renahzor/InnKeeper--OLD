using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowController : MonoBehaviour {

    [SerializeField]
    GameObject questWindow, inventoryWindow, activeAdventurerWindow, ledgerWindow, activeQuestWindow;

    // Use this for initialization
    void Start () {
        questWindow.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
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
                if (hit.transform.tag == "QuestGiver")
                {
                    questWindow.SetActive(true);
                    questWindow.GetComponent<QuestWindowController>().questHolder = hit.transform.GetComponent<QuestItemScript>();
                }
            }
        }

    }

    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);
    }

    public void ChangeWindowState(GameObject window)
    {
        window.SetActive(!window.activeSelf);
    }

    public void AddHeroToWindow(Adventurer a)
    {
        activeAdventurerWindow.GetComponent<ActiveHeroPanel>().AddHero(a);
    }

}
