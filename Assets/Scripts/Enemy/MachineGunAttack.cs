using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MachineGunAttack : LaneSwitchAttack
{
    public Transform machineGun;
    public Transform firingPoint;
    [SerializeField] float machineGunRotateSpeed = 0.5f;
    [SerializeField] float laneIterations = 3;

    public GameObject machineGunSparks;
    [SerializeField] int shotsPerIteration = 5;
    [SerializeField] float distanceBetweenShots = 1;
    [SerializeField] float timeBetweenSparks = 0.2f;

    float sparkTimer = 0;
    int shotsFired;
    GameObject sparks;
    private Vector3 hitPosition;
    private Vector3 direction;
    private int lanePicked;
    private int iteration = 0;
    private bool initialShotFired = false;

    public UnityEvent OnMachineGunAttack;

    private void OnEnable()
    {
        shotsFired = 0;
        hitPosition = new Vector3();
        
        lanePicked = PickNextPoint();        
    }

    private void Update()
    {        
        if(!atLanePosition)
        {
            MoveToPosition(lanePicked);
        }
        else if(!initialShotFired)
        {
            FireInitialShot();
            OnMachineGunAttack.Invoke();
        }
        else
        {
            UpdateMachineGunAttack();
        }

    }

    private void UpdateMachineGunAttack()
    {
        machineGun.Rotate(0, machineGunRotateSpeed, 0);

        sparkTimer += Time.deltaTime;

        if (sparkTimer > timeBetweenSparks)
        {
            hitPosition.z -= distanceBetweenShots;
            FireShot();

            sparkTimer = 0;
            shotsFired++;

            if (shotsFired == shotsPerIteration)
            {
                GameObject.Destroy(sparks);
                shotsFired = 0;
                iteration++;
                initialShotFired = false;                

                if (iteration == laneIterations)
                {
                    AttackFinished = true;
                    this.enabled = false;
                }
                else
                {
                    lanePicked = PickNextPoint();
                    atLanePosition = false;
                }
            }
        }
    }

    private void FireInitialShot()
    {
        if (Physics.Raycast(firingPoint.position, firingPoint.forward, out RaycastHit hit))
        {
            Vector3 spawnPos = new Vector3(hit.point.x, 0.53f, hit.point.z);
            sparks = Instantiate(machineGunSparks, spawnPos, Quaternion.identity);
            hitPosition = sparks.transform.position;

            initialShotFired = true;
        }
        else
        {
            Debug.LogWarning("Raycast from machine gun did not hit anything");
        }
    }

    private void FireShot()
    {
        direction = (hitPosition - firingPoint.position).normalized;

        if (Physics.Raycast(firingPoint.position, direction, out RaycastHit hit))
        {
            sparks.transform.Translate(Vector3.back * distanceBetweenShots);

            if(hit.transform.tag == "Player")
            {
                GameManager.Instance.EndGame();
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(firingPoint.position, direction * 50);
        Gizmos.DrawRay(firingPoint.position, firingPoint.forward * 100);
    }

}
