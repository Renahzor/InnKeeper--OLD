using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//stats container for adventurer NPCs.
public class AdventurerStats : MonoBehaviour {

    public enum Profession { Fighter, Pickpocket, Acolyte, Apprentice };
    public enum Race { Human, Elf, Dwarf, Halfling }

    Profession profession;
    Race race;

    public int level, HP, maxHP;
    public int strength, agility, toughness, smarts, minDamage, maxDamage, gold;

    public int exp; //{ get; private set; }

    public int levelUpExp;

    public string advName;
    public List<string> statsList = new List<string>();

    public void UpdateStatList()
    {
        statsList.Clear();
        statsList.Add(profession.ToString());
        statsList.Add(race.ToString());

        statsList.Add("Level: " + level);
        statsList.Add("Strength: " + strength);
        statsList.Add("Agility: " + agility);
        statsList.Add("Toughness: " + toughness);
        statsList.Add("Smarts: " + smarts);
        statsList.Add("Gold: " + gold);
    }

    public void SetStats(int _level, string name)
    {
        profession = (Profession)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Profession)).Length);
        race = (Race)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Race)).Length);
        level = _level;
        strength = 1;
        agility = 1;
        toughness = 1;
        smarts = 1;
        HP = 10;
        maxHP = HP;
        minDamage = 1;
        maxDamage = 5;
        exp = 0;
        gold = 2;
        advName = name;
        UpdateStatList();

        levelUpExp = 100;
        //for (int i = 0; i < level; i++)
            //levelUpExp *= 3;        
    }

    public void AddEXP(int experience)
    {
        exp += experience;
        if (exp >= levelUpExp)
        {
            levelUp();
            levelUpExp *= 3;
        }

    }

    void levelUp()
    {
        switch (profession)
        {
            case Profession.Fighter:
                level++;
                strength++;
                toughness++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    smarts++;
                else
                    agility++;
                maxHP += toughness;
                break;
            case Profession.Acolyte:
                level++;
                smarts++;
                strength++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    toughness++;
                else
                    agility++;
                maxHP += toughness;
                break;
            case Profession.Apprentice:
                level++;
                smarts++;
                agility++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    strength++;
                else
                    toughness++;
                maxHP += toughness;
                break;
            case Profession.Pickpocket:
                level++;
                agility++;
                strength++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    smarts++;
                else
                    toughness++;
                maxHP += toughness;
                break;
            default:
                break;
        }

    }
}
