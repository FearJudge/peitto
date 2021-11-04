using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedAudio : MonoBehaviour
{
    public AudioSource toPlay;
    public float minDelay = 5f;
    public float maxDelay = 12f;
    public AudioClip[] randomClips;

    private void Start()
    {
        StartCoroutine(RDelay());
    }

    IEnumerator RDelay()
    {
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        while (this.gameObject != null)
        {
            if (randomClips.Length > 0) { toPlay.PlayOneShot(randomClips[Random.Range(0, randomClips.Length)]); }
            else { toPlay.Play(); }
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }
}
