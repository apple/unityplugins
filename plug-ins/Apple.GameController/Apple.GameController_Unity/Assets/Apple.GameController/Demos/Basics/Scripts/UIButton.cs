using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public RectTransform rectTx;

    //
    public void SetLocalPosition(float xPos, float yPos)
    {
        rectTx.localPosition = new Vector3(xPos, yPos, 0f);
    }
}
