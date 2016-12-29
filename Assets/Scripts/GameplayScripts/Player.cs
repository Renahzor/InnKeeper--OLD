using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public int playerGold = 10;
    public int innLevel = 1;

    public Text goldDisplay;

	// Use this for initialization
	void Start () {
        UpdateGoldDisplay();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateGoldDisplay()
    {
        goldDisplay.text = "Gold: " + playerGold.ToString();
    }
}
