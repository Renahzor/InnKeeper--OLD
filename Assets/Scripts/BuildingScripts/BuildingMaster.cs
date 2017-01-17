using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaster : MonoBehaviour {

    public List<GameObject> panelList = new List<GameObject>();
    public List<GameObject> itemPrefabs = new List<GameObject>();
    public GameObject buttonPrefab;

    // Use this for initialization
    void Start () {
        ChangePanelView(0);		
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

        while (building)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Player.Instance.playerGold -= item.GetComponent<BuildableObject>().buildCost;
                Player.Instance.UpdateGoldDisplay();

                if (item.GetComponent<BedScript>())
                    GameMaster.Instance.restObjectsInScene.Add(item);
                building = false;
            }

            else
            {
                Vector3 previousPosition = item.transform.position;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    item.transform.position = new Vector3(hit.point.x, hit.point.y, 0);
                }
                else
                    item.transform.position = previousPosition;   
            }
            yield return null;
        }
    }
}
