using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIndicator2D : MonoBehaviour
{
    public RectTransform rectTx;
    private RectTransform _indicatorRectTx;
    private float _halfWidth;
    private float _halfHeight;

    // --
    void Start()
    {
        _halfWidth  = rectTx.rect.width / 2f;
        _halfHeight = rectTx.rect.height / 2f;

        _indicatorRectTx = rectTx.Find("InputIndicator").gameObject.GetComponent<RectTransform>();
    }

    //
    public void SetLocalPosition(float xPos, float yPos)
    {
        _indicatorRectTx.localPosition = new Vector3(xPos * _halfWidth, yPos * _halfHeight, 0f);
    }
}
