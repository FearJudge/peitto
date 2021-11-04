using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneControls
{
    public static int currentScene;
    public static bool allowTransition = false;

    public static void ChangeScene(string toLoad)
    {
        currentScene = SceneManager.GetSceneByName(toLoad).buildIndex;
        SceneManager.LoadScene(toLoad);
    }

    public static IEnumerator LoadSceneInBG(string scene, int bgm = 0)
    {
        AsyncOperation b = SceneManager.LoadSceneAsync(scene);
        b.allowSceneActivation = false;
        while (b.progress < 0.9f)
        {
            yield return null;
        }
        while (!allowTransition)
        {
            yield return null;
        }
        if (bgm != -1) { AudioScript.PlayBGM(bgm, 0.7f, 1f); }
        b.allowSceneActivation = true;
        allowTransition = false;
    }
    public static IEnumerator waitAllowC(float t)
    {
        yield return new WaitForSeconds(t);
        allowTransition = true;
    }
}
