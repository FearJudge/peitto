using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reason : MonoBehaviour
{
    [SerializeField] private HUDManager playerEyes;
    public PlayerMovement playerBody;
    public GameObject subSet;

    public enum Reasoning
    {
        JumpOnce,
        GoThroughSubSet,
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
                else { playerBody.Midairjumps = 0; FileManager.CreateFile(FileManager.ruleDatas[0]); }
                break;
            case Reasoning.GoThroughSubSet:
                break;
            default:
                break;
        }
    }

    IEnumerator Glitch(Reasoning eff)
    {
        SetEffect(eff, true);
        StartCoroutine(playerEyes.Glitch());
        Debug.Log("GLITCHES!");
        yield return new WaitForSeconds(20f);
        SetEffect(eff, false);
        playerEyes.Glitching = false;
    }
}
