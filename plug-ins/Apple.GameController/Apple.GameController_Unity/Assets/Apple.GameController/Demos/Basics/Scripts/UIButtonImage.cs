using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonImage : MonoBehaviour
{
    public Image buttonImage;

    //
    public void SetColor(Color color)
    {
        buttonImage.color = color;
    }

    //
    public void SetTexture(Texture2D tex)
    {
        Rect r = new Rect(0f, 0f, tex.width, tex.height);
        Vector2 v = new Vector2(.5f, .5f);
        buttonImage.sprite = Sprite.Create(tex, r, v);
    }
}
