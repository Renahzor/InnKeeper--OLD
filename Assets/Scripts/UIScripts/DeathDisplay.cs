using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathDisplay : MonoBehaviour {

    [SerializeField]
    GameObject deadHeroDisaplayPanel;
    [SerializeField]
    GameObject deadHeroElementPrefab;

    //Simple death tracking. TODO Randomize sayings
    public void RecordDeath(string hName, string enemyName)
    {
        var newDeadHero = Instantiate(deadHeroElementPrefab);
        newDeadHero.gameObject.transform.SetParent(deadHeroDisaplayPanel.transform, false);
        Text[] textElements = newDeadHero.GetComponentsInChildren<Text>();
        textElements[0].text = "R.I.P. " + hName;
        textElements[1].text = "Sadly slain by " + enemyName;
    }
}
