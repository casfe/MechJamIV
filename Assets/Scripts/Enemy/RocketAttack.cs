using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class RocketAttack : EnemyAttack
{
    public GameObject rocketPrefab;
    public Transform rocketLauncher;
    public Transform spawnPoint;
    [SerializeField] int launcherRotateSpeed = 1;
    [SerializeField] int wavesToFire = 1;
    [SerializeField] float timeBetweenWaves = 1;

    [Header("Lane Positions")]
    [SerializeField] Transform leftLanePoint;
    [SerializeField] Transform middleLanePoint;
    [SerializeField] Transform rightLanePoint;

    private GameObject rocket1, rocket2, rocket3;
    private int wavesFired;

    public UnityEvent OnRocketFired;

    protected override void OnEnable()
    {
        base.OnEnable();
        wavesFired = 0;

        FireWave();
    }

    private void Update()
    {
        /*
        if (rocketLauncher.rotation.eulerAngles.y < 180)
            rocketLauncher.Rotate(0, launcherRotateSpeed * Time.deltaTime, 0);
        else if (wavesFired < 1)
            FireWave();*/

        if (wavesFired == wavesToFire)
        {
            if (rocket1 == null && rocket2 == null && rocket3 == null)
                AttackFinished = true;
        }
    }

    private void FireWave()
    {
        // fire first rocket
        rocket1 = Instantiate(rocketPrefab, spawnPoint.position, rocketPrefab.transform.rotation);
        rocket1.GetComponent<Rocket>().LanePosition = leftLanePoint;
        Vector3 direction = (leftLanePoint.position - spawnPoint.position).normalized;
        direction.y = 0;
        rocket1.GetComponent<Rocket>().InitialDirection = direction;        

        // fire second rocket
        rocket2 = Instantiate(rocketPrefab, spawnPoint.position, rocketPrefab.transform.rotation);
        rocket2.GetComponent<Rocket>().LanePosition = middleLanePoint;
        direction = (middleLanePoint.position - spawnPoint.position).normalized;
        direction.y = 0;
        rocket2.GetComponent<Rocket>().InitialDirection = direction;  

        // fire third rocket
        rocket3 = Instantiate(rocketPrefab, spawnPoint.position, rocketPrefab.transform.rotation);
        rocket3.GetComponent<Rocket>().LanePosition = rightLanePoint;
        direction = (rightLanePoint.position - spawnPoint.position).normalized;
        direction.y = 0;
        rocket3.GetComponent<Rocket>().InitialDirection = direction;      

        wavesFired++;

        OnRocketFired?.Invoke();

        if (wavesFired < wavesToFire)
            Invoke("FireWave", timeBetweenWaves);
    }

}
