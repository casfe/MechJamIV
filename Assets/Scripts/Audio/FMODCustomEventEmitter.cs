using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODCustomEventEmitter : MonoBehaviour
{
    public void PlayOneShotSoundUsingString(string eventName)
    {
        RuntimeManager.PlayOneShot(eventName);
    }
}
