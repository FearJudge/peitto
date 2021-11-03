using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    Color[] cols = new Color[] { new Color(0, 0.6f, 0.2f), new Color(0, 0, 0.7f), new Color(0.8f, 0.4f, 0.4f), new Color(0.2f, 0, 0.5f), new Color(0.2f, 0.9f, 0.3f), new Color(0.4f, 0.1f, 0.9f) };

    public Slider staminaSlider;
    public Button quitButton;
    public Button returnToGame;
    public Image insanity;
    public RectTransform pingBase;
    public Animator[] insanityPingAnimations;
    public AudioSource insanityPingSound;
    int inValue = 0;
    bool filling = false;
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

    public void Insanity()
    {
        StartCoroutine(FillInsanity());
    }

    IEnumerator FillInsanity()
    {
        if (filling) { yield break; }
        filling = true;
        while (inValue != Reason.insanity * 4)
        {
            if (Reason.insanity * 4 > inValue) { inValue++; }
            else if (Reason.insanity * 4 < inValue) { inValue--; }
            insanity.fillAmount = inValue / 400f;
            yield return new WaitForSeconds(0.1f);
        }
        filling = false;
    }

    public void QuitToDesk()
    {
        Debug.Log("GONE");
        Application.Quit();
    }

    public IEnumerator Glitch()
    { 
        Glitching = true;
        insanityPingSound.Play();
        for (int a = 0; a < insanityPingAnimations.Length; a++)
        {
            yield return new WaitForSeconds(Random.Range(0.01f, 0.06f));
            insanityPingAnimations[a].SetTrigger("InPing");
        }
        while (Glitching)
        {
            yield return new WaitForSeconds(Random.Range(0.15f,0.9f));
            pingBase.anchoredPosition = new Vector2(150, Random.Range(-250, -151));
            insanityPingAnimations[Random.Range(0, insanityPingAnimations.Length)].SetTrigger("InPing");
        }
    }

    public Color GetGlitchColor()
    {
        int val = Random.Range(0, cols.Length);
        return cols[val];
    }
}
