using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Supports two indicators
public class UITouchpadIndicator : MonoBehaviour
{
    public RectTransform rectTx;

    private RectTransform[] _indicatorRectTransforms;
    private float _halfWidth;
    private float _halfHeight;

    void Start()
    {
        _halfWidth  = rectTx.rect.width / 2f;
        _halfHeight = rectTx.rect.height / 2f;

        _indicatorRectTransforms = new RectTransform[2];

        _indicatorRectTransforms[0] = rectTx.Find("InputIndicator0").gameObject.GetComponent<RectTransform>();
        _indicatorRectTransforms[1] = rectTx.Find("InputIndicator1").gameObject.GetComponent<RectTransform>();
    }

    //
    public void SetLocalPosition(uint index, float xPos, float yPos)
    {
        if (index < 2)
        {
            _indicatorRectTransforms[index].localPosition = new Vector3(xPos * _halfWidth, yPos * _halfHeight, 0f);
        }
    }
}
