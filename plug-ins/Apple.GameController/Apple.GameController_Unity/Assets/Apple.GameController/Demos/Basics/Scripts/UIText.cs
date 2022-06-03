using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIText : MonoBehaviour
{
    public Text textObject;

    public void SetText(string value)
    {
        textObject.text = value;
    }
}
