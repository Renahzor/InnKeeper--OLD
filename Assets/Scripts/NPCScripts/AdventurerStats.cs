using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//stats container for adventurer NPCs.
public class AdventurerStats : MonoBehaviour {

    public enum Profession { Fighter, Pickpocket, Acolyte, Apprentice };
    public enum Race { Human, Elf, Dwarf, Halfling, Gnome }
    public enum AttackType { Unarmed, Melee, Stealth, Arcane }

    public Profession profession;
    public Race race;
    public AttackType attackType;

    public int level, HP, maxHP;
    public int strength, agility, toughness, smarts, minDamage, maxDamage, gold;

    public int exp; //{ get; private set; }

    public int levelUpExp;

    public string advName;
    public List<string> statsList = new List<string>();

    public int advHappiness;

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

        levelUpExp = 100;      

        advHappiness = UnityEngine.Random.Range(50, 100);

        switch (profession)
        {
            case Profession.Fighter:
            attackType = AttackType.Melee;
                break;
            case Profession.Acolyte:
                attackType = AttackType.Unarmed;
                break;
            case Profession.Pickpocket:
                attackType = AttackType.Stealth;
                break;
            case Profession.Apprentice:
                attackType = AttackType.Arcane;
                break;
            default:
                    break;
        }

        levelUp();
        HP = maxHP;
        UpdateStatList();
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

    public void ChangeHappiness(int change)
    {
        Mathf.Clamp(advHappiness += change, 0.0f, 100.0f);
    }

    void levelUp()
    {
        switch (profession)
        {
            case Profession.Fighter:
                level++;
                strength += UnityEngine.Random.Range(1, 3);
                toughness++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    smarts++;
                else
                    agility++;
                maxHP += toughness;
                minDamage = 1 + strength;
                maxDamage = 5 + strength;
                break;
            case Profession.Acolyte:
                level++;
                toughness += UnityEngine.Random.Range(1, 3);
                smarts++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    strength++;
                else
                    agility++;
                maxHP += toughness;
                minDamage = 1 + toughness;
                maxDamage = 5 + toughness;
                break;
            case Profession.Apprentice:
                level++;
                smarts += UnityEngine.Random.Range(1, 3);
                agility++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    strength++;
                else
                    toughness++;
                maxHP += toughness;
                minDamage = 1 + smarts;
                maxDamage = 5 + smarts;
                break;
            case Profession.Pickpocket:
                level++;
                agility += UnityEngine.Random.Range(1, 3);
                strength++;
                if (UnityEngine.Random.Range(0, 100) > 50)
                    smarts++;
                else
                    toughness++;
                maxHP += toughness;
                minDamage = 1 + agility;
                maxDamage = 5 + agility;
                break;
            default:
                break;
        }

    }
}
