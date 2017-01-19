using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActiveHeroPanel : MonoBehaviour {

    public static ActiveHeroPanel Instance { get; private set; }

    Dictionary<Adventurer, GameObject> activeHeroes = new Dictionary<Adventurer, GameObject>();
    public GameObject activeHeroInfo;

    void Awake()
    {
        Instance = this;
    }

	public void AddHero(Adventurer a)
    {
        if (activeHeroes.ContainsKey(a))
            return;
        else
        {
            GameObject temp = Instantiate(activeHeroInfo);
            temp.transform.SetParent(this.transform, false);
            activeHeroes.Add(a, temp);
            UpdateHeroStats(a);
            a.GetComponent<NPCBehaviors>().healthBar = temp.transform.FindChild("HealthBar").GetComponent<Image>();
        }    
    }

    public void UpdateHeroStats(Adventurer a)
    {
        List<Text> textComponents = new List<Text>();
        activeHeroes[a].GetComponentsInChildren<Text>(textComponents);

        foreach(Text t in textComponents)
        {
            if (t.name == "HeroName")
                t.text = a.gameObject.GetComponent<AdventurerStats>().advName;
            else if (t.name == "Activity")
                t.text = a.GetComponent<StateTracker>().advActivity;
            else if (t.name == "Time")
                t.text = a.GetComponent<StateTracker>().activityTime.ToString("F2");
        }
    }

    public void RemoveHero(Adventurer a)
    {
        if (activeHeroes.ContainsKey(a))
        {
            GameObject.Destroy(activeHeroes[a]);
            activeHeroes.Remove(a);
        }
    }

    public int NumberOfHeroesActive()
    {
        return activeHeroes.Count;
    }
}
