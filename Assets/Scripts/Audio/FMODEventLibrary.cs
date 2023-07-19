using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEventLibrary : MonoBehaviour
{

    string machineGunStartPath = "";
    string machineGunAttackPath = "event:/SFX/EnemyEvents/EnemyGunAttack";
    string rocketFirePath = "event:/SFX/EnemyEvents/EnemyRocketFire";
    string mineDroppedPath = "event:/SFX/EnemyEvents/EnemyMineDrop";

    FMOD.Studio.EventInstance laserInstance;
    FMOD.Studio.EventInstance laserStartInstance;
    string laserStartPath = "event:/SFX/EnemyEvents/EnemyLaserStart";
    string laserAttackPath = "event:/SFX/EnemyEvents/EnemyLaserAttack";
    string laserEndPath = "event:/SFX/EnemyEvents/EnemyLaserEnd";
    string laserLockPath = "event:/SFX/EnemyEvents/EnemyLaserLock";

    FMOD.Studio.EventInstance musicInstance;
    string gameplayMusicPath = "event:/Music/GameplayMusic";
    string winStingerPath = "event:/Music/WinStinger";
    string loseStingerPath = "event:/Music/LoseStinger";
    string enemyMechDamagePath = "event:/SFX/EnemyEvents/EnemyDamage";

    string explosionPath = "event:/SFX/EnemyEvents/EnemyAttackExplosion";
    
    FMOD.Studio.EventInstance snapshotInstance;
    string pauseSnapshotPath = "snapshot:/Pause";

    string acceptButtonPath = "event:/SFX/UI/Open";
    string backButtonPath = "event:/SFX/UI/Close";

    string playerMovePath = "event:/SFX/PlayerEvents/PlayerMove";

    private void Start()
    {
        StartMusic();
        snapshotInstance = RuntimeManager.CreateInstance(pauseSnapshotPath);
    }

    public void PlayMachineGunStartSound()
    {
        FMODUtilities.PlayOneShotUsingString(machineGunStartPath);
    }

    public void PlayMachineGunAttackSound()
    {
        FMODUtilities.PlayOneShotUsingString(machineGunAttackPath);
    }

    public void PlayRocketFireSound()
    {
        FMODUtilities.PlayOneShotUsingString(rocketFirePath);
    }

    public void PlayMineDroppedSound()
    {
        FMODUtilities.PlayOneShotUsingString(mineDroppedPath);
    }

    public void PlayLaserStartSound()
    {
        laserStartInstance = RuntimeManager.CreateInstance(laserStartPath);
        laserStartInstance.start();
        laserInstance = RuntimeManager.CreateInstance(laserAttackPath);
    }

    public void PlayLaserLockSound()
    {
        FMODUtilities.PlayOneShotUsingString(laserLockPath);
        laserStartInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        laserStartInstance.release();
    }

    public void PlayLaserAttackSound()
    {
        laserInstance.start();
    }

    public void PauseLaserAttackSound()
    {
        laserInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayLaserStopSound()
    {
        FMODUtilities.PlayOneShotUsingString(laserEndPath);
        laserInstance.release();
    }

    public void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(gameplayMusicPath);
        if (PlaybackState(musicInstance) != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            musicInstance.start();
            SetMusicState(100);
        }
    }
    public void SetMusicState(float bossHealth)
    {
        musicInstance.setParameterByName("BossHealth", bossHealth);
    }
    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    public void PlayWinStinger()
    {
        FMODUtilities.PlayOneShotUsingString(winStingerPath);
    }

    public void PlayLoseStinger()
    {
        FMODUtilities.PlayOneShotUsingString(loseStingerPath);
    }

    public void PlayMechDamageSound()
    {
        FMODUtilities.PlayOneShotUsingString(enemyMechDamagePath);
    }

    FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        Debug.Log(pS);
        return pS;
    }

    public void PlayExplosionSound()
    {
        FMODUtilities.PlayOneShotUsingString(explosionPath);
    }

    public void PauseSnapshot()
    {
        snapshotInstance.start();
    }

    public void ResumeSnapshot()
    {
        snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayUIButtonAccept()
    {
        FMODUtilities.PlayOneShotUsingString(acceptButtonPath);
    }

    public void PlayUIButtonBack()
    {
        FMODUtilities.PlayOneShotUsingString(backButtonPath);
    }

    public void PlayPlayerMove()
    {
        FMODUtilities.PlayOneShotUsingString(playerMovePath);
    }

    private void OnDisable()
    {
        StopMusic();
        ResumeSnapshot();
    }
}
