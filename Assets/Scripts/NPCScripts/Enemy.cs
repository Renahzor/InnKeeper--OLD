using UnityEngine;

public class Enemy{

    public string name;
    int HP, attackMin, attackMax, attackBonusChance;
    public int level, armor;
    float attackTimer, attackSwingReset;

    public Enemy(string _name, int _hpMax, int _level)
    {
        name = _name;
        HP = _hpMax;
        level = _level;
        attackMin = Random.Range(1, _level * 2);
        attackMax = Random.Range(attackMax, _level * 4);
        attackTimer = Random.Range(1.0f, 5.0f);
        attackSwingReset = Random.Range(2.0f, 6.0f);
        attackBonusChance = _level;
        armor = Random.Range(8, 12) + _level;
    }


    public int Attack(int armorValue)
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0.0)
        {
            return 0;
        }

        else
        {
            attackTimer = attackSwingReset;

            if (Random.Range(1, 20) + attackBonusChance >= armorValue)
            {
                return Random.Range(attackMin, attackMax);
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
