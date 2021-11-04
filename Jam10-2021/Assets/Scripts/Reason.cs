using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reason : MonoBehaviour
{
    [SerializeField] private HUDManager playerEyes;
    public PlayerMovement playerBody;
    public GameObject subSet;
    public static int insanity = 0;
    int glitches = 0;
    bool insanityLower = false;
    List<Reasoning> activeReasonings = new List<Reasoning>();

    public enum Reasoning
    {
        JumpOnce,
        GoThroughSubSet,
        InfinteRun,
    }

    public void BackInGame()
    {
        playerEyes.ShowQuit(false);
    }

    public void StartEffect(Reasoning eff)
    {
        if (activeReasonings.Contains(eff)) { return; }
        StartCoroutine(Glitch(eff));
    }

    void SetEffect(Reasoning eff, bool state)
    {
        switch (eff)
        {
            case Reasoning.JumpOnce:
                if (state) { playerBody.Midairjumps = 1; insanity += 15; }
                else { playerBody.Midairjumps = 0;}
                break;
            case Reasoning.GoThroughSubSet:
                break;
            case Reasoning.InfinteRun:
                if (state) { playerBody.unexhaustable = true; insanity += 8; }
                else { playerBody.unexhaustable = false;}
                break;
            default:
                break;
        }
    }

    public void CheckInsanity()
    {
        if (Random.Range(0, 100) > 12) { return; }
        int effect = Random.Range(0, 4);
        switch (effect)
        {
            case 1:
                if (insanity < 30) { return; }
                playerBody.invertedControls = Random.Range(500, 800);
                break;
            case 2:
                string[] randomQuotes = new string[] { "Why you Little!", "Press [Alt + F4] for GOD MODE!", "Up is Down, Down is still Down. Always Down.", "1+1=2. 2+2=4. Quick Maths." };
                playerEyes.Think(randomQuotes[Random.Range(0, randomQuotes.Length)]);
                break;
            case 3:
                AudioScript.PlaySFX(Random.Range(2,4), 0.8f, 1f);
                break;
            default:
                playerBody.doubleFootsteps = Random.Range(4, 20);
                break;
        }
        
    }

    IEnumerator InsanityFix()
    {
        if (insanityLower) { yield break; }
        insanityLower = true;
        while (insanity > 0)
        {
            Debug.Log(string.Format("Hidden Variable Insanity: {0}", insanity));
            playerEyes.Insanity();
            yield return new WaitForSeconds(5f);
            CheckInsanity();
            insanity--;
        }
        insanity = 0;
        insanityLower = false;
    }

    IEnumerator Glitch(Reasoning eff)
    {
        SetEffect(eff, true);
        if (activeReasonings.Count == 0) { StartCoroutine(playerEyes.Glitch()); }
        activeReasonings.Add(eff);
        if (!insanityLower) { StartCoroutine(InsanityFix()); }
        if (insanity > 100) { insanity = 100; }
        playerEyes.Insanity();
        yield return new WaitForSeconds(20f);
        SetEffect(eff, false);
        activeReasonings.Remove(eff);
        if (activeReasonings.Count == 0) { playerEyes.Glitching = false; }
    }
}
