using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    static int phase = 0;
    public bool isManager = false;
    public Animator[] managersAnimators;

    private void Update()
    {
        if (!isManager) { return; }
        if (Input.inputString != "") { phase++; }
        for (int c = 0; c < managersAnimators.Length; c++)
        {
            managersAnimators[c].SetInteger("Phase", phase);
        }
    }

    public void LoadScene(string toLoad)
    {
        SceneControls.ChangeScene(toLoad);
    }

    public void TransitionTryNow()
    {
        SceneControls.allowTransition = true;
    }

    public void BeginLoading(string toLoad)
    {
        StartCoroutine(SceneControls.LoadSceneInBG(toLoad));
    }

    public void advancePhase()
    {
        phase++;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
