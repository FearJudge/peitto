using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSounds : MonoBehaviour
{
    public void PlaySoundWithID(int id)
    {
        AudioScript.PlaySFX(id, 0.7f, 1f);
    }
}
