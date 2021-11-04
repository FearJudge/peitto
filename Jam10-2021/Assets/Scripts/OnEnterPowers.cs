using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnEnterPowers : MonoBehaviour
{
    public bool addPower = true;
    public bool removePowerInstead = false;
    public int powerIndex = 0;
    public bool activateHud = false;
    public bool saySomething = false;
    public string textToSay = "";
    public float waitToSay = 0f;
    private bool triggered = false;
    public bool loadScene = false;
    public string sceneToLoad = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7 || triggered) { return; }
        triggered = true;
        if (activateHud || saySomething)
        {
            PlayerMovement pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
            if (activateHud) { pm.hud.ShowBrain(true); }
            if (saySomething) { StartCoroutine(SayAfter(waitToSay, pm)); }
        }
        if (addPower) { PlayerMovement.GainPower(powerIndex, removePowerInstead); }
        if (loadScene) { StartCoroutine(SceneControls.LoadSceneInBG(sceneToLoad)); SceneControls.allowTransition = true; }
    }

    IEnumerator SayAfter(float delay, PlayerMovement pm)
    {
        yield return new WaitForSeconds(delay);
        pm.hud.Think(textToSay);
        yield return null;
    }
}
