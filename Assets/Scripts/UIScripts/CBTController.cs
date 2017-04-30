using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBTController : MonoBehaviour {

    public GameObject CBTPrefab;

    public void CreateCBT(string textInput, Transform parent, Color textColor, string animationName)
    {
        var newCBT = Instantiate(CBTPrefab) as GameObject;

        newCBT.transform.SetParent(parent, false);
        //newCBT.GetComponent<RectTransform>().anchoredPosition = parent.GetComponent<RectTransform>().anchoredPosition;
        newCBT.GetComponent<Text>().text = textInput;
        newCBT.GetComponent<Text>().transform.SetAsLastSibling();
        newCBT.GetComponent<Text>().color = textColor;
        newCBT.GetComponent<Animator>().SetTrigger(animationName);

        Destroy(newCBT, 1.1f);       
    }
}
