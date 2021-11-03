using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider staminaSlider;

    public void SetStamina(int value)
    {
        staminaSlider.value = (value / 1000f);
    }
}
