using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : EnemyAttack
{
    public Transform laserGunSet;
    public Transform laserCannon;
    public Transform firePoint;

    [SerializeField] int fireIterations = 3;
    [SerializeField] float standbyTime = 2.0f;

    [Header("Cannon Horizontal Rotation")]
    [SerializeField] float rotateSpeed = 0.5f;
    [SerializeField] float fireRotationSpeed = 1.0f;
    [Range(0, 180)]
    [SerializeField] float fireAngle = 30;

    [Header("Cannon Veritcal Rotation")]
    [SerializeField] float raiseSpeed = 0.2f;
    [SerializeField] float descendSpeed = 0.5f;
    [Range(0, 90)]
    [SerializeField] float raiseAngle = 10;

    LineRenderer laserLine;
   
    private int iteration = 0;
    private bool raiseGun = true;
    private RotationState state = RotationState.RotateTowardsPlayer;

    [Header("Debugging")]
    float cannonAngle;

    enum RotationState
    {
        Standby,
        RotateTowardsPlayer,
        RotateClockwise,
        RotateAntiClockwise,
        RotateToOrigin
    }

    private void Awake()
    {
        laserLine = laserGunSet.GetComponent<LineRenderer>();    
    }

    private void OnEnable()
    {
        laserLine = laserGunSet.GetComponent<LineRenderer>();
        //raiseGun = true;
    }

    void Update()
    {        
        switch(state)
        {
            case RotationState.RotateTowardsPlayer:
                laserGunSet.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);

                if (laserGunSet.transform.rotation.eulerAngles.y >= 180)
                {
                    state = RotationState.Standby;
                    Invoke("FireLaser", standbyTime);
                }
            break;

            case RotationState.RotateClockwise:
                RotateClockwise();
                UpdateLaserLine();
            break;

            case RotationState.RotateAntiClockwise:
                RotateAntiClockwise();
                UpdateLaserLine();
            break;

            case RotationState.RotateToOrigin:
                laserGunSet.Rotate(0, -rotateSpeed * Time.deltaTime, 0, Space.Self);

                if (laserGunSet.rotation.y <= 0)
                {
                    laserCannon.rotation = Quaternion.identity;
                    state = RotationState.Standby;
                    AttackFinished = true;
                }
           break;
        }

        UpdateVerticalRotation();

        cannonAngle = laserCannon.eulerAngles.x;
    }

    private void UpdateLaserLine()
    {
        Vector3 fireDirection = firePoint.forward;
        Ray raycast = new Ray(firePoint.position, firePoint.forward);

        laserLine.SetPosition(0, firePoint.position);

        if(Physics.Raycast(raycast, out RaycastHit hitInfo))
        {
            laserLine.SetPosition(1, hitInfo.point);
        }
    }

    private void RotateClockwise()
    {
        laserGunSet.Rotate(0, fireRotationSpeed * Time.deltaTime, 0, Space.Self);

        if (laserGunSet.transform.rotation.eulerAngles.y >= 180 + (fireAngle / 2))
        {
            iteration++;

            if (iteration > fireIterations)
            {
                state = RotationState.RotateToOrigin;
                laserLine.enabled = false;
            }
            else
                state = RotationState.RotateAntiClockwise;
        }
    }

    private void RotateAntiClockwise()
    {
        laserGunSet.Rotate(0, -fireRotationSpeed * Time.deltaTime, 0, Space.Self);

        if (laserGunSet.rotation.eulerAngles.y <= 180 - (fireAngle / 2))
        {
            iteration++;
            if (iteration > fireIterations)
            {
                state = RotationState.RotateToOrigin;
                laserLine.enabled = false;
            }
            else
                state = RotationState.RotateClockwise;
        }
    }

    private void UpdateVerticalRotation()
    {
        if (raiseGun && (state == RotationState.RotateClockwise || state == RotationState.RotateAntiClockwise))
        {
            float angleTravelled = Mathf.Abs(laserCannon.eulerAngles.x - 360);

            laserCannon.Rotate(Vector3.left * raiseSpeed * Time.deltaTime, Space.Self);

            if (angleTravelled >= raiseAngle && angleTravelled < raiseAngle + 1)
            {
                raiseGun = false;
            }
        }

        if (state == RotationState.RotateToOrigin)
        {
            if (laserCannon.eulerAngles.x < 360 && laserCannon.eulerAngles.x > 1)
                laserCannon.Rotate(Vector3.right * descendSpeed * Time.deltaTime, Space.Self);
            else if(laserCannon.eulerAngles.x < 1 || laserCannon.eulerAngles.x > 360)
                laserCannon.localRotation = Quaternion.identity;
        }
    }

    private void FireLaser()
    {
        iteration++;
        state = RotationState.RotateClockwise;
        laserLine.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, firePoint.forward * 50);
    }
}
