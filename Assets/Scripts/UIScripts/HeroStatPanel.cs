using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatPanel : MonoBehaviour {

    Adventurer activeHero;
    [SerializeField]
    Text heroName, heroDeck;
    [SerializeField]
    Image heroSprite;
    [SerializeField]
    Image healthBar, happinessBar, questBar, foodbar, drinkBar, socialBar;
    [SerializeField]
    Text strDisplay, agiDisplay, tghDisplay, smrDisplay, atkDisplay, acDisplay, goldDisplay;

    AdventurerStats stats;
    AdventurerNeeds needs;

    void Start()
    {
        activeHero = null;
        stats = null;
        needs = null;
        SetActiveHero(GameObject.Find("ActiveHeroPanel").GetComponent<ActiveHeroPanel>().ReturnHeroByndex(0));
    }

    void Update()
    {
        if (activeHero != null)
        {
            healthBar.rectTransform.localScale = new Vector3((float)stats.HP / (float)stats.maxHP, 1, 1);

            questBar.rectTransform.localScale = new Vector3(needs.questNeed / 100.0f, 1, 1);
            foodbar.rectTransform.localScale = new Vector3(needs.foodNeed / 100.0f, 1, 1);
            drinkBar.rectTransform.localScale = new Vector3(needs.drinkNeed / 100.0f, 1, 1);
            socialBar.rectTransform.localScale = new Vector3(needs.socialNeed / 100.0f, 1, 1);
            happinessBar.rectTransform.localScale = new Vector3(stats.advHappiness / 100.0f, 1, 1);

            strDisplay.text = "Strength: " + stats.strength;
            agiDisplay.text = "Agility: " + stats.agility;
            tghDisplay.text = "Toughness: " + stats.toughness;
            smrDisplay.text = "Smarts: " + stats.smarts;

            atkDisplay.text = "Attack: (" + stats.attackType + ") " + (stats.minDamage) + " - " + (stats.maxDamage);
            acDisplay.text = "Armor: " + (10 + stats.toughness + stats.agility).ToString();

            goldDisplay.text = "Gold: " + stats.gold;
        }

    }

    public void SetActiveHero(Adventurer a)
    {
        activeHero = a;
        if (activeHero != null)
        {
            stats = a.gameObject.GetComponent<AdventurerStats>();
            needs = a.gameObject.GetComponent<AdventurerNeeds>();

            heroSprite.sprite = a.gameObject.GetComponent<SpriteRenderer>().sprite;
            heroSprite.color = a.gameObject.GetComponent<Renderer>().material.color;
            heroName.text = stats.advName;
            heroDeck.text = "Lvl " + stats.level + " " + stats.race + " " + stats.profession;
        }
    }
}
