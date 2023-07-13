using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class LaserAttack : EnemyAttack
{
    public Transform laserGunSet;
    public Transform laserCannon;
    public Transform firePoint;

    [SerializeField] float standbyTime = 2.0f;

    [SerializeField] LaserAttackValues[] attackTypes;

    private int fireIterations;

    // Cannon Horizontal Rotation
    private RotateDirection startDirection;
    private float rotateSpeed;
    private float fireRotationSpeed;
    private float fireAngle;

    // Cannon Veritcal Rotation
    private float raiseSpeed;
    private float descendSpeed;
    private float raiseAngle;

    private bool setLimit;
    private float limitAngle;

    private LineRenderer laserLine;
   
    private int iteration = 0;
    private int attacksPerformed = 0;
    private bool raiseGun = true;
    private RotationState state = RotationState.RotateTowardsPlayer;

    public enum RotateDirection { Clockwise, AntiClockwise };

    enum RotationState
    {
        Standby,
        RotateTowardsPlayer,
        RotateClockwise,
        RotateAntiClockwise,
        ReturnToCenter,
        ReturnToOrigin
    }

    private void Awake()
    {
        laserLine = laserGunSet.GetComponent<LineRenderer>();
        InitializeValues(0);
    }

    private void OnEnable()
    {
        laserLine = laserGunSet.GetComponent<LineRenderer>();
        InitializeValues(0);
    }

    private void InitializeValues(int index)
    {
        fireIterations = attackTypes[index].fireIterations;
        startDirection = attackTypes[index].startDirection;
        rotateSpeed = attackTypes[index].rotateSpeed;
        fireRotationSpeed = attackTypes[index].fireRotationSpeed;
        fireAngle = attackTypes[index].fireAngle;
        raiseSpeed = attackTypes[index].raiseSpeed;
        descendSpeed = attackTypes[index].descendSpeed;
        raiseAngle = attackTypes[index].raiseAngle;
        setLimit = attackTypes[index].setLimit;
        limitAngle = attackTypes[index].limitAngle;
    }

    void Update()
    {        
        switch(state)
        {
            case RotationState.RotateTowardsPlayer:
                RotateTowardsPlayer();
            break;

            case RotationState.RotateClockwise:
                RotateClockwise();
                UpdateLaserLine();
            break;

            case RotationState.RotateAntiClockwise:
                RotateAntiClockwise();
                UpdateLaserLine();
            break;

            case RotationState.ReturnToCenter:
                ReturnToCenter();
            break;

            case RotationState.ReturnToOrigin:
                RotateToOrigin();
           break;
        }

        UpdateVerticalRotation();
    }

    private void RotateTowardsPlayer()
    {
        if(startDirection == RotateDirection.Clockwise)
        {
            laserGunSet.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);

            if (laserGunSet.rotation.eulerAngles.y >= 180)
            {
                state = RotationState.Standby;
                Invoke("FireLaser", standbyTime);
            }
        }
        else if(startDirection == RotateDirection.AntiClockwise)
        {
            laserGunSet.Rotate(0, -rotateSpeed * Time.deltaTime, 0, Space.Self);

            if (laserGunSet.transform.rotation.eulerAngles.y <= 180)
            {
                state = RotationState.Standby;
                Invoke("FireLaser", standbyTime);
            }


        }
    }

    private void RotateToOrigin()
    {
        float rotation = rotateSpeed;
        bool limitReached = false;

        if (startDirection == RotateDirection.Clockwise)
        {
            rotation = -rotateSpeed;
            limitReached = laserGunSet.rotation.y <= 0;
        }
        else if(startDirection == RotateDirection.AntiClockwise)
        {
            rotation = rotateSpeed;
            limitReached = laserGunSet.rotation.y >= 0;
        }

        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if (limitReached)
        {
            laserCannon.rotation = Quaternion.identity;
            state = RotationState.Standby;
            AttackFinished = true;            
        }

    }

    private void ReturnToCenter()
    {
        bool limitReached;

        if (laserGunSet.rotation.eulerAngles.y < 180)
        {
            laserGunSet.Rotate(0, fireRotationSpeed * Time.deltaTime, 0, Space.Self);
            limitReached = laserGunSet.rotation.eulerAngles.y >= 180;

        }
        else if(laserGunSet.rotation.eulerAngles.y > 180)
        {
            laserGunSet.Rotate(0, -fireRotationSpeed * Time.deltaTime, 0, Space.Self);
            limitReached = laserGunSet.rotation.eulerAngles.y <= 180;
        }
        else
        {
            limitReached = true;
        }

        if (limitReached)
        {
            attacksPerformed++;
            if (attacksPerformed >= attackTypes.Length)
            {
                state = RotationState.ReturnToOrigin;
            }
            else
            {                
                iteration = 0;
                raiseGun = true;
                state = RotationState.Standby;

                InitializeValues(attacksPerformed);
                Invoke("FireLaser", standbyTime);
            }
        }

    }

    private void UpdateLaserLine()
    {
        Vector3 fireDirection = firePoint.forward;
        Ray raycast = new Ray(firePoint.position, firePoint.forward);

        laserLine.SetPosition(0, firePoint.position);

        if(Physics.Raycast(raycast, out RaycastHit hitInfo))
        {
            laserLine.SetPosition(1, hitInfo.point);

            if(hitInfo.transform.tag == "Player")
            {
                GameManager.Instance.EndGame();
            }
        }
    }

    private void RotateAndFire()
    {

    }

    private void RotateClockwise()
    {
        bool limitReached = false;
        laserGunSet.Rotate(0, fireRotationSpeed * Time.deltaTime, 0, Space.Self);

        if(setLimit && iteration == fireIterations)
        {
            if(laserGunSet.transform.rotation.eulerAngles.y >= limitAngle)
            {
                limitReached = true;
            }

        }
        else if (laserGunSet.transform.rotation.eulerAngles.y >= 180 + (fireAngle / 2))
        {
            limitReached = true;
        }

        if(limitReached)
        {
            iteration++;

            if (iteration > fireIterations)
            {
                state = RotationState.ReturnToCenter;
                laserLine.enabled = false;
            }
            else
                state = RotationState.RotateAntiClockwise;
        }

    }

    private void RotateAntiClockwise()
    {
        bool limitReached = false;
        laserGunSet.Rotate(0, -fireRotationSpeed * Time.deltaTime, 0, Space.Self);

        if (setLimit && iteration == fireIterations)
        {
            if (laserGunSet.transform.rotation.eulerAngles.y <= limitAngle)
            {
                limitReached = true;
            }

        }
        else if (laserGunSet.rotation.eulerAngles.y <= 180 - (fireAngle / 2))
        {
            limitReached = true;
        }

        if (limitReached)
        {
            iteration++;

            if (iteration > fireIterations)
            {
                state = RotationState.ReturnToCenter;
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

        if (state == RotationState.ReturnToOrigin || state == RotationState.ReturnToCenter)
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
        laserLine.enabled = true;

        if (startDirection == RotateDirection.Clockwise)
            state = RotationState.RotateClockwise;
        else
            state = RotationState.RotateAntiClockwise;        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, firePoint.forward * 50);
    }
}