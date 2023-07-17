using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAttack : EnemyAttack
{
    public GameObject missilePrefab;
    public Transform rocketLauncher;
    [SerializeField] int launcherRotateSpeed = 1;
    [SerializeField] int wavesToFire = 1;
    [SerializeField] float timeBetweenWaves = 1;

    [Header("Spawn Positions")]
    [SerializeField] Transform leftSpawnPoint;
    [SerializeField] Transform middleSpawnPoint;
    [SerializeField] Transform rightSpawnPoint;

    private GameObject missile1, missile2, missile3;
    private int wavesFired;

    private void OnEnable()
    {
        wavesFired = 0;
    }

    private void Update()
    {
        if (rocketLauncher.rotation.eulerAngles.y < 180)
            rocketLauncher.Rotate(0, launcherRotateSpeed * Time.deltaTime, 0);
        else if (wavesFired < 1)
            FireWave();

        if (wavesFired == wavesToFire)
        {
            if (missile1 == null && missile2 == null && missile3 == null)
                AttackFinished = true;
        }
    }

    private void FireWave()
    {
        missile1 = Instantiate(missilePrefab, leftSpawnPoint.position, missilePrefab.transform.rotation);
        missile2 = Instantiate(missilePrefab, middleSpawnPoint.position, missilePrefab.transform.rotation);
        missile3 = Instantiate(missilePrefab, rightSpawnPoint.position, missilePrefab.transform.rotation);
        wavesFired++;

        if (wavesFired < wavesToFire)
            Invoke("FireWave", timeBetweenWaves);
    }

}
