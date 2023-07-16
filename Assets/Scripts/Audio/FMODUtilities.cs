using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public static class FMODUtilities
{
    public static void PlayOneShotUsingString(string eventPath)
    {
        RuntimeManager.PlayOneShot(eventPath);
    }
}
