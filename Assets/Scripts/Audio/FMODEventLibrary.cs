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
    string laserStartPath = "event:/SFX/EnemyEvents/EnemyLaserStart";
    string laserAttackPath = "event:/SFX/EnemyEvents/EnemyLaserAttack";
    string laserEndPath = "event:/SFX/EnemyEvents/EnemyLaserEnd";

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
        FMODUtilities.PlayOneShotUsingString(laserStartPath);
        laserInstance = RuntimeManager.CreateInstance(laserAttackPath);
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
}
