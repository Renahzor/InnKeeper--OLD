using UnityEngine;
using System;

public class Enemy{

    public string name;
    int HP, attackMin, attackMax, attackBonusChance;
    public int level, armor;
    float attackTimer, attackSwingReset;
    public AdventurerStats.AttackType weaknessType;
    public bool critLastHit = false;
    public Sprite mySprite;

    public Enemy(string _name, int _hpMax, int _level, Sprite enemySprite)
    {
        name = _name;
        HP = UnityEngine.Random.Range(_hpMax, _hpMax * 2);
        level = _level;
        attackMin = UnityEngine.Random.Range(1, _level * 3);
        attackMax = UnityEngine.Random.Range(attackMin, attackMin + _level * 4);
        attackTimer = UnityEngine.Random.Range(1.0f, 5.0f);
        attackSwingReset = UnityEngine.Random.Range(2.0f, 6.0f);
        attackBonusChance = _level;
        armor = UnityEngine.Random.Range(8, 13) + _level;
        var values = Enum.GetValues(typeof(AdventurerStats.AttackType));
        weaknessType = (AdventurerStats.AttackType)UnityEngine.Random.Range(0, values.Length);
        mySprite = enemySprite;
    }


    public int Attack(int armorValue, bool opportunityAttack)
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0.0 && !opportunityAttack)
        {
            return 0;
        }

        else
        {
            critLastHit = false;
            attackTimer = attackSwingReset;

            int attackRoll = UnityEngine.Random.Range(1, 21);

            if (attackRoll == 20)
            {
                critLastHit = true;
                return UnityEngine.Random.Range(attackMin, attackMax) * 2;
            }

            if (attackRoll + attackBonusChance >= armorValue)
            {
                return UnityEngine.Random.Range(attackMin, attackMax);
            }

            else return 0;
        }
    }

    //returns true if enemy is killed
    public bool ReduceHP(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            return true;
        }

        return false;
    }
}
