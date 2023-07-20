using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class LaserAttack2 : EnemyAttack
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
    private bool angleReached = false;
    private RotateDirection rotateDirection;

    private float degreesRemaining;

    public enum RotateDirection { Clockwise, AntiClockwise };

    [SerializeField] public LaserEvents laserEvents;

    enum RotationState
    {
        Standby,
        RotateTowardsPlayer,
        RotateAndFire,        
        ReturnToCenter,
        ReturnToOrigin
    }

    protected override void OnEnable()
    {        
        base.OnEnable();
        laserLine = laserGunSet.GetComponent<LineRenderer>();
        attacksPerformed = 0;
        iteration = 0;

        InitializeValues(0);

        //laserGunSet.localRotation = Quaternion.identity;
        //laserCannon.localRotation = Quaternion.Euler(0, -20.716f, 0);
        state = RotationState.RotateTowardsPlayer;
    }

    private void InitializeValues(int index)
    {
        fireIterations = attackTypes[index].fireIterations;
        startDirection = (RotateDirection)attackTypes[index].startDirection;
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

            case RotationState.RotateAndFire:
                RotateAndFire();
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

            if(Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1)
            {
                state = RotationState.Standby;
                laserEvents.OnLaserLock?.Invoke();
                Invoke("FireLaser", standbyTime);
            }
        }
        else if(startDirection == RotateDirection.AntiClockwise)
        {
            laserGunSet.Rotate(0, -rotateSpeed * Time.deltaTime, 0, Space.Self);

            if (Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1)
            {
                state = RotationState.Standby;
                laserEvents.OnLaserLock?.Invoke();
                Invoke("FireLaser", standbyTime);
            }

        }

        if (!laserEvents.startSoundPlayed)
        {
            laserEvents.OnLaserWindup?.Invoke();
            laserEvents.startSoundPlayed = true;
        }
    }

    private void RotateAndFire()
    {
        // if an end limit angle has been set on the scriptable object
        if (setLimit && iteration == fireIterations)
        {
            if (Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, limitAngle, 0)) < 1)
            {
                angleReached = true;
            }
        }
        // clockwise rotation
        else if (rotateDirection == RotateDirection.Clockwise)
        {
            angleReached = false;
            laserGunSet.Rotate(0, fireRotationSpeed * Time.deltaTime, 0, Space.Self);

            degreesRemaining = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180 + (fireAngle / 2), 0));
            
            if (Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180 + (fireAngle / 2), 0)) < 1)
            {
                angleReached = true;
            }
        }
        // anti-clockwise rotation
        else if (rotateDirection == RotateDirection.AntiClockwise)
        {
            angleReached = false;
            float desiredAngle;

            laserGunSet.Rotate(0, -fireRotationSpeed * Time.deltaTime, 0, Space.Self);
            //Debug.Log("Current Angle: " + transform.rotation.eulerAngles.y + ", Desired Angle: " + desiredAngle + ", Degrees Renaining: " + degreesRemaining);

            desiredAngle = 180 - (fireAngle / 2);

            if (Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, desiredAngle, 0)) < 1)
                angleReached = true;            
        }
        
        if (angleReached)
        {
            iteration++;

            if (iteration > fireIterations)
            {
                state = RotationState.ReturnToCenter;
                laserLine.enabled = false;
            }
            else if(rotateDirection == RotateDirection.Clockwise)
            {                
                rotateDirection = RotateDirection.AntiClockwise;
            }
            else if(rotateDirection == RotateDirection.AntiClockwise)
            {
                rotateDirection = RotateDirection.Clockwise;
            }
        }
    }

    private void RotateToOrigin()
    {
        float rotation = rotateSpeed;
        angleReached = false;

        if (startDirection == RotateDirection.Clockwise)
        {
            rotation = -rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }
        else if(startDirection == RotateDirection.AntiClockwise)
        {
            rotation = rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }

        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if (angleReached)
        {
            laserCannon.rotation = Quaternion.identity;
            state = RotationState.Standby;
            AttackFinished = true;
        }

        if (!laserEvents.endSoundPlayed)
        {
            laserEvents.OnLaserEnd?.Invoke();
            laserEvents.endSoundPlayed = true;
        }
    }

    private void ReturnToCenter()
    {
        angleReached = false;

        if (laserGunSet.rotation.eulerAngles.y < 180)
        {
            laserGunSet.Rotate(0, fireRotationSpeed * Time.deltaTime, 0, Space.Self);
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1;            

        }
        else if(laserGunSet.rotation.eulerAngles.y > 180)
        {
            laserGunSet.Rotate(0, -fireRotationSpeed * Time.deltaTime, 0, Space.Self);
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1;             
        }
        else
        {
            angleReached = true;
        }

        if (angleReached)
        {
            attacksPerformed++;
            if (attacksPerformed >= attackTypes.Length)
            {
                laserEvents.OnLaserPause?.Invoke();
                state = RotationState.ReturnToOrigin;
            }
            else
            {                
                iteration = 0;
                raiseGun = true;
                state = RotationState.Standby;

                InitializeValues(attacksPerformed);

                laserEvents.shootSoundPlayed = false;
                laserEvents.OnLaserPause?.Invoke();

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
                laserEvents.OnLaserPause?.Invoke();
                GameManager.Instance.EndGame();
            }
        }
        else
        {
            Debug.LogWarning("Laser raycast not hitting anything");
        }
    }

    private void UpdateVerticalRotation()
    {
        if (raiseGun && state == RotationState.RotateAndFire)
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

        rotateDirection = startDirection;
        state = RotationState.RotateAndFire;

        if (!laserEvents.shootSoundPlayed)
        {
            laserEvents.OnLaserShoot?.Invoke();
            laserEvents.shootSoundPlayed = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, firePoint.forward * 50);
    }
}