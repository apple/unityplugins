using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public Slider slider;

    // On the range [0, 1]
    public void SetValue(float value)
    {
        slider.value = value;
    }
}
