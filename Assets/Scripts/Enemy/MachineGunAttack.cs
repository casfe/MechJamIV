using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunAttack : EnemyAttack
{
    public Transform machineGun;
    public Transform firingPoint;
    [SerializeField] float machineGunRotateSpeed = 0.5f;

    public GameObject machineGunSparks;
    [SerializeField] int totalShots = 5;
    [SerializeField] float distanceBetweenSparks = 1;
    [SerializeField] float timeBetweenSparks = 0.2f;

    float sparkTimer = 0;
    int shotsFired;
    GameObject sparks;
    private Vector3 hitPosition;
    private Vector3 direction;

    [Header("FMOD event paths")]
    [SerializeField] string machineGunStart;
    [SerializeField] string machineGunAttack;



    private void OnEnable()
    {
        shotsFired = 0;
        hitPosition = new Vector3();
        FMODUtilities.PlayOneShotUsingString(machineGunAttack);

        if(Physics.Raycast(firingPoint.position, firingPoint.forward, out RaycastHit hit))
        {
            Vector3 spawnPos = new Vector3(hit.point.x, 0.53f, hit.point.z);
            sparks = Instantiate(machineGunSparks, spawnPos, Quaternion.identity);
            hitPosition = sparks.transform.position;
        }
        else
        {
            Debug.LogWarning("Raycast from machine gun did not hit anything");
        }
    }

    private void Update()
    {        
        machineGun.Rotate(0, machineGunRotateSpeed, 0);
        
        sparkTimer += Time.deltaTime;

        if(sparkTimer > timeBetweenSparks)
        {            
            hitPosition.z -= distanceBetweenSparks;
            FireShot();

            sparkTimer = 0;
            shotsFired++;

            if (shotsFired == totalShots)
            {
                GameObject.Destroy(sparks);
                AttackFinished = true;
                this.enabled = false;
            }
        }

    }

    private void FireShot()
    {
        direction = (hitPosition - firingPoint.position).normalized;

        if (Physics.Raycast(firingPoint.position, direction, out RaycastHit hit))
        {
            sparks.transform.Translate(Vector3.back * distanceBetweenSparks);

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
