using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageTextScript : MonoBehaviour {

    public float duration = 4.0f;

    Vector3 targetPosition;

    // Use this for initialization
    void Start () {
		
        targetPosition = GetComponent<RectTransform>().localPosition;
        targetPosition.y += 100.0f;

    }
	
	// Update is called once per frame
	void Update () {

        duration -= Time.deltaTime;

        if (duration >= 0.0f)
        {
            GetComponent<RectTransform>().localPosition = Vector3.Lerp(GetComponent<RectTransform>().localPosition, targetPosition, Time.deltaTime);
            Color tmp = GetComponent<Text>().color;
            tmp.a -= Time.deltaTime / 3;
            GetComponent<Text>().color = tmp;
        }

        else Destroy(gameObject);

	}
}
