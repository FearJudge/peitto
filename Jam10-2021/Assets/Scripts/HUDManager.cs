using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    Color[] cols = new Color[] { new Color(0, 0.6f, 0.2f), new Color(0, 0, 0.7f), new Color(0.8f, 0.4f, 0.4f), new Color(0.2f, 0, 0.5f), new Color(0.2f, 0.9f, 0.3f), new Color(0.4f, 0.1f, 0.9f) };

    public Slider staminaSlider;
    public Slider[] SetOfGlitches;
    public Button quitButton;
    public Button returnToGame;
    public bool Glitching = false;

    public void SetStamina(int value)
    {
        staminaSlider.value = (value / 1000f);
    }

    public void ShowQuit(bool state)
    {
        Cursor.lockState = CursorLockMode.None;
        if (state) { state = !quitButton.gameObject.activeSelf; }
        if (!state) { ReFocus(); }
        else { quitButton.gameObject.SetActive(true); returnToGame.gameObject.SetActive(true); }
    }

    public void ReFocus()
    {
        Cursor.lockState = CursorLockMode.Locked;
        quitButton.gameObject.SetActive(false);
        returnToGame.gameObject.SetActive(false);
    }

    public void QuitToDesk()
    {
        Debug.Log("GONE");
        Application.Quit();
    }

    public IEnumerator Glitch()
    {
        Glitching = true;
        while (Glitching)
        {
            for (int a = 0; a < SetOfGlitches.Length; a++)
            {
                SetOfGlitches[a].value = Random.Range(0f, 1f);
                ColorBlock g = SetOfGlitches[a].colors;
                g.disabledColor = GetGlitchColor();
                SetOfGlitches[a].colors = g;
            }
            yield return new WaitForSeconds(0.1f);
        }
        for (int b = 0; b < SetOfGlitches.Length; b++)
        {
            SetOfGlitches[b].value = 0;
        }
    }

    public Color GetGlitchColor()
    {
        int val = Random.Range(0, cols.Length);
        return cols[val];
    }
}
