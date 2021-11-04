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

    public static IEnumerator LoadSceneInBG(string scene)
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
        AudioScript.PlayBGM(0, 0.7f, 1f);
        b.allowSceneActivation = true;
        allowTransition = false;
    }
}
