using UnityEngine;

public class AdventurerNeeds : MonoBehaviour {

    public float foodNeed, drinkNeed, questNeed, socialNeed;
    float foodRate, drinkRate, questRate, socialRate;
	
    void Start()
    {
        foodNeed = Random.Range(70f, 100f);
        drinkNeed = Random.Range(70f, 100f);
        questNeed = Random.Range(70f, 100f);
        socialNeed = Random.Range(70f, 100f);

        foodRate = -0.05f;
        drinkRate = -0.05f;
        questRate = -0.05f;
        socialRate = -0.05f;
    }

	// Update is called once per frame
	void Update () {
        foodNeed = Mathf.Clamp((foodNeed += (foodRate * Time.deltaTime)), 0.0f, 100.0f);
        drinkNeed = Mathf.Clamp((drinkNeed += (drinkRate * Time.deltaTime)), 0.0f, 100.0f);
        questNeed = Mathf.Clamp((questNeed += (questRate * Time.deltaTime)), 0.0f, 100.0f);
        socialNeed = Mathf.Clamp((socialNeed += (socialRate * Time.deltaTime)), 0.0f, 100.0f);
    }

    public void SetNeedRates(float food, float drink, float quest, float social)
    {
        foodRate = food;
        drinkRate = drink;
        questRate = quest;
        socialRate = social;
    }

    public void ResetRates()
    {
        foodRate = -0.05f;
        drinkRate = -0.05f;
        questRate = -0.05f;
        socialRate = -0.05f;
    }
}
