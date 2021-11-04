using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public List<AudioClip> SFX;
    public List<AudioClip> BGM;
    private static List<GameObject> currentSounds = new List<GameObject>();
    private static GameObject currentBGM;
    private static AudioScript instance;
    private float bgmvolume = 0.7f;
    private bool bgmAudioFade = false;

    public void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(this); }
        if (instance.BGM.Count >= 2) { PlayBGM(1, bgmvolume, 1f); }
    }

    public static void PlaySFX(int soundid, float volume, float pitch)
    {
        if (instance == null) { return; }
        GameObject audio = new GameObject("SoundEffect", typeof(AudioSource));
        var source = audio.GetComponent<AudioSource>();
        currentSounds.Add(audio);
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(instance.SFX[soundid]);
        instance.StartCoroutine(PlayMe(source, audio));
    }

    public static void PlaySFX(AudioClip clip, float volume, float pitch)
    {
        if (instance == null) { return; }
        GameObject audio = new GameObject("SoundEffect", typeof(AudioSource));
        var source = audio.GetComponent<AudioSource>();
        currentSounds.Add(audio);
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(clip);
        instance.StartCoroutine(PlayMe(source, audio));
    }

    public static void PlayBGM(float volume, float pitch)
    {
        if (instance == null) { return; }
        PlayBGM(Random.Range(0, instance.BGM.Count), volume, pitch);
    }

    public static void PlayBGM(int soundid, float volume, float pitch)
    {
        if (instance == null) { return; }
        AudioSource player = BGMControl();
        instance.ControlBGM(player, 1f, 1f, instance.BGM[soundid], volume);
    }

    public static void PlayBGM(AudioClip bgm, float volume, float pitch, bool fakeSFX=false)
    {
        if (instance == null) { return; }
        AudioSource player = BGMControl();
        if (fakeSFX) { instance.ControlFakeBGM(player, 0.6f, 0.2f, bgm, volume); }
        else { instance.ControlBGM(player, 1f, 1f, bgm, volume); }
    }

    public static void StopBGM()
    {
        if (instance == null) { return; }
        AudioSource player = BGMControl();
        instance.ControlBGM(player);
    }

    private void ControlBGM(AudioSource player, float outVol, float inVol, AudioClip clip, float volume)
    {
        bgmAudioFade = true;
        StartCoroutine(FadeSwap(player, outVol, inVol, clip, volume));
    }

    private void ControlFakeBGM(AudioSource player, float outVol, float inVol, AudioClip clip, float volume)
    {
        bgmAudioFade = true;
        StartCoroutine(FadeSwap(player, outVol, inVol, clip, volume));
    }

    private void ControlBGM(AudioSource player)
    {
        bgmAudioFade = true;
        StartCoroutine(FadeSwap(player, 1.5f, 0f));
    }

    private static AudioSource BGMControl()
    {
        if (currentBGM == null)
        {
            GameObject audio = new GameObject("SoundTrack", typeof(AudioSource));
            audio.transform.parent = instance.gameObject.transform;
            currentBGM = audio;
        }
        var source = currentBGM.GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        return source;
    }

    static IEnumerator PlayMe(AudioSource src, GameObject obj)
    {
        bool trapped = true;
        while (trapped)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (src == null) { trapped = false; }
            else { trapped = src.isPlaying; }
        }
        if (src == null) { yield break; }
        currentSounds.Remove(obj);
        Destroy(obj);
        yield return null;
    }

    IEnumerator FadeSwap(AudioSource src, float timeOut, float timeIn, AudioClip newClip = null, float newVolumeIn = 0f)
    {
        float volumeStep = src.volume / 20f;
        if (src.isPlaying && timeOut > 0f)
        {
            for (int a = 0; a < 20; a++)
            {
                src.volume -= volumeStep;
                yield return new WaitForSeconds(timeOut / 20f);
            }
            src.Stop();
        }
        float volumeStepIn = newVolumeIn / 20f;
        if (volumeStepIn < 0 || volumeStepIn > 0.05f) { volumeStepIn = volumeStep; if (volumeStepIn < 0 || volumeStepIn > 0.05f) { volumeStep = 0.05f; } }
        if (newClip == null || timeIn == 0f) { yield break; }
        src.clip = newClip;
        src.Play();
        src.volume = 0f;
        for (int a = 0; a < 20; a++)
        {
            src.volume += volumeStepIn;
            yield return new WaitForSeconds(timeIn/20f);
        }
        if (bgmAudioFade == true) { bgmAudioFade = false; }
    }

}
