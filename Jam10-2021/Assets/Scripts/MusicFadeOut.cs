using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeOut : MonoBehaviour
{
    public AudioSource toPlay;
    public float fadeTime = 8f;
    float startvol = 0f;
    float volStep = 0f;

    // Start is called before the first frame update
    void Start()
    {
        toPlay.loop = true;
        startvol = toPlay.volume;
        toPlay.Play();
        volStep = startvol / (fadeTime * 10f);
        StartCoroutine(FadeVol());
    }

    IEnumerator FadeVol()
    {
        while (toPlay.volume > 0)
        {
            yield return new WaitForSeconds(0.1f);
            toPlay.volume -= volStep;
        }
        toPlay.Stop();
    }
}
