using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaster : MonoBehaviour {

    public List<GameObject> panelList = new List<GameObject>();
    public List<GameObject> bedPrefabs = new List<GameObject>();
    public List<GameObject> wallPrefabs = new List<GameObject>();
    public List<GameObject> questPrefabs = new List<GameObject>();

    // Use this for initialization
    void Start () {
        ChangePanelView(0);		
	}
	
    //Changes which panel is active
    public void ChangePanelView(int indexOfPanel)
    {
        foreach (GameObject go in panelList)
        {
            go.SetActive(false);
        }

        panelList[indexOfPanel].SetActive(true);
    }
}
