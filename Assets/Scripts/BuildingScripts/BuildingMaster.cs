using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaster : MonoBehaviour {

    public List<GameObject> panelList = new List<GameObject>();
    public List<GameObject> itemPrefabs = new List<GameObject>();

    public GameObject restItemParent;
    public GameObject questItemParent;
    public GameObject wallItemParent;
    public GameObject idleItemParent;

    public GameObject buttonPrefab;

    // Use this for initialization
    void Start () {
        ChangePanelView(0);

        int i = 0;
        foreach (GameObject item in itemPrefabs)
        {
            //TODO Refactor this to avoid repetitive actions
            if (item.GetComponent<BedScript>() != null)
            {
                bool panelState = restItemParent.activeSelf;
                restItemParent.SetActive(true);

                var button = Instantiate(buttonPrefab);
                button.transform.SetParent(restItemParent.transform, false);
                button.GetComponent<BuildMenuButton>().SetupButton(i);
                restItemParent.SetActive(panelState);
            }

            else if (item.GetComponent<QuestItemScript>() != null)
            {
                bool panelState = questItemParent.activeSelf;
                questItemParent.SetActive(true);

                var button = Instantiate(buttonPrefab);
                button.transform.SetParent(questItemParent.transform, false);
                button.GetComponent<BuildMenuButton>().SetupButton(i);
                questItemParent.SetActive(panelState);
            }

            else if (item.GetComponent<IdleActivityScript>() != null)
            {
                bool panelState = questItemParent.activeSelf;
                questItemParent.SetActive(true);

                var button = Instantiate(buttonPrefab);
                button.transform.SetParent(idleItemParent.transform, false);
                button.GetComponent<BuildMenuButton>().SetupButton(i);
                questItemParent.SetActive(panelState);
            }
            i++;
        }
	}
	
    //Changes which panel is active, called by buttons on the UI
    public void ChangePanelView(int indexOfPanel)
    {
        foreach (GameObject go in panelList)
        {
            go.SetActive(false);
        }
        panelList[indexOfPanel].SetActive(true);
    }

    public void BuildItem(int index)
    {
        var item = Instantiate(itemPrefabs[index]);
        StartCoroutine(BuildingItem(item));
    }

    IEnumerator BuildingItem(GameObject item)
    {
        bool building = true;

        //if the mouse button is held over from clicking the button, wait for it to be released.
        while (Input.GetMouseButton(0))
            yield return null;

        while (building)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Player.Instance.playerGold < item.GetComponent<BuildableObject>().buildCost)
                {
                    Destroy(item);
                    Debug.Log("Not enough gold to build");
                    building = false;
                    yield break;
                }

                else
                {
                    Player.Instance.playerGold -= item.GetComponent<BuildableObject>().buildCost;
                    Player.Instance.UpdateGoldDisplay();

                    if (item.GetComponent<BedScript>())
                        GameMaster.Instance.RestObjects.Add(item);

                    if (item.GetComponent<QuestItemScript>())
                        GameMaster.Instance.QuestObjects.Add(item);

                    if (item.GetComponent<IdleActivityScript>())
                        GameMaster.Instance.IdleObjects.Add(item);

                    building = false;
                }
            }

            else
            {
                Vector3 previousPosition = item.transform.position;

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                item.transform.position = new Vector3(mousePos.x, mousePos.y, -1f);
            }
            yield return null;
        }
    }
}
