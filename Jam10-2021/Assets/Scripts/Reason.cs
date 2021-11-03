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
        StartCoroutine(Glitch(eff));
    }

    void SetEffect(Reasoning eff, bool state)
    {
        switch (eff)
        {
            case Reasoning.JumpOnce:
                if (state) { playerBody.Midairjumps = 3; }
                else { playerBody.Midairjumps = 0; insanity += 15; FileManager.CreateFile(FileManager.ruleDatas[0]); }
                break;
            case Reasoning.GoThroughSubSet:
                break;
            case Reasoning.InfinteRun:
                if (state) { playerBody.unexhaustable = true; }
                else { playerBody.unexhaustable = false; insanity += 8; FileManager.CreateFile(FileManager.ruleDatas[2]); }
                break;
            default:
                break;
        }
    }

    public void CheckInsanity()
    {
        if (Random.Range(0, 100) > insanity) { return; }
        playerBody.doubleFootsteps =Random.Range(20, 50);
    }

    IEnumerator InsanityFix()
    {
        if (insanityLower) { yield break; }
        insanityLower = true;
        while (insanity > 0)
        {
            Debug.Log(string.Format("Hidden Variable Insanity: {0}", insanity));
            yield return new WaitForSeconds(insanity / 2f);
            CheckInsanity();
            insanity--;
        }
        insanity = 0;
        insanityLower = false;
    }

    IEnumerator Glitch(Reasoning eff)
    {
        glitches++;
        SetEffect(eff, true);
        StartCoroutine(playerEyes.Glitch());
        yield return new WaitForSeconds(20f);
        glitches--;
        SetEffect(eff, false);
        if (glitches == 0) { playerEyes.Glitching = false; }
        if (!insanityLower) { StartCoroutine(InsanityFix()); }
        if (insanity > 100) { insanity = 100; }
    }
}
