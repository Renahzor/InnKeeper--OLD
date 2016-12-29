using UnityEngine;

public class AdventurerNeeds : MonoBehaviour {

    public float foodNeed, drinkNeed, questNeed, socialNeed;
    public float foodRate, drinkeRate, questRate, socialRate;
	
    void Start()
    {
        foodNeed = 100;
        drinkNeed = 100;
        questNeed = 100;
        socialNeed = 100;
    }

	// Update is called once per frame
	void Update () {
        foodNeed = Mathf.Clamp((foodNeed += (foodRate * Time.deltaTime)), 0.0f, 100.0f);
        drinkNeed = Mathf.Clamp((drinkNeed += (drinkeRate * Time.deltaTime)), 0.0f, 100.0f);
        questNeed = Mathf.Clamp((questNeed += (questRate * Time.deltaTime)), 0.0f, 100.0f);
        socialNeed = Mathf.Clamp((socialNeed += (socialRate * Time.deltaTime)), 0.0f, 100.0f);
    }
}
