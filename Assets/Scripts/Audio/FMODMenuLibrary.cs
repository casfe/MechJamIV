using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODMenuLibrary : MonoBehaviour
{
    EventInstance menuMusicInstance;
    string menuMusicPath = "event:/Music/MenuMusic";
    string acceptButtonPath = "event:/SFX/UI/Open";
    string backButtonPath = "event:/SFX/UI/Close";

    private void Start()
    {
        menuMusicInstance = RuntimeManager.CreateInstance(menuMusicPath);
        menuMusicInstance.start();
    }

    public void PlayUIButtonAccept()
    {
        FMODUtilities.PlayOneShotUsingString(acceptButtonPath);
    }

    public void PlayUIButtonBack()
    {
        FMODUtilities.PlayOneShotUsingString(backButtonPath);
    }

    private void OnDisable()
    {
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuMusicInstance.release();
    }
}
