using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

    [SerializeField]
    CBTController CombatTextController;

    public void RecieveAttack(int attackDamage, Transform cbtParent, bool crit, bool weak)
    {
        string textToDisplay = "";
        string attackOutcomeType = "Hit";
        Color textColor;

        if (crit)
        {
            textToDisplay = "Crit ";
            textColor = Color.red;
            attackOutcomeType = "Crit";
        }
        else if (weak)
        {
            textToDisplay = "Weak ";
            textColor = Color.blue;
            attackOutcomeType = "Crit";
        }
        else
        {
            textColor = Color.yellow;
        }

        textToDisplay += attackDamage;

        CombatTextController.CreateCBT(textToDisplay, cbtParent, textColor, attackOutcomeType);
    }
}
