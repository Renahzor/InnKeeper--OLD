using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleActivityScript : MonoBehaviour {

    public float activityDuration;
    public string activityName;
    public int activityCost;

    [SerializeField]
    List<string> activityMessages = new List<string>();

    public string getActivityMessage()
    {
        return activityMessages[Random.Range(0, activityMessages.Count)];
    }
}
