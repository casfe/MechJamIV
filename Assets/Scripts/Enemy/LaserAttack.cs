using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : EnemyAttack
{
    [Header("Game Objects and transform")]
    [SerializeField] Transform laserGunSet;
    [SerializeField] Transform laserCannon;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform marker;
    [SerializeField] Transform markerOrigin;

    [Header("Default Turret Rotation")]
    [Tooltip("Time delay before the after the turret faces the player before firing the laser")]
    [SerializeField] float standbyTime = 2.0f;    
    [Tooltip("The speed in which the turret will rotate towards and away from the player when not firing")]
    [SerializeField] float horizontalRotateSpeed = 100;
    [Tooltip("The speed in which the will rotate back down after completing an attack")]
    [SerializeField] float verticalRotateSpeed = 100;

    [SerializeField] LaserAttackValues[] attackTypes;

    [Header("Events")]
    [SerializeField] public LaserEvents laserEvents;

    RotateDirection startDirection = RotateDirection.Clockwise;

    //[Header("Marker Movement ")]
    float verticalVelocity = 1;
    float horizontalVelocity = 1;
    float maxHorizontalDistance = 1;
    float maxVerticalDistance = 5;

    //[Header("For Debugging")]
    private float horizontalDistance = 0;
    private float verticalDistance = 0;

    private Vector3 markerVelocity;
    private bool clockwiseRotation;
    private bool rotatingFromCenter;

    private Vector3 lastEndPoint;
    private AttackState state;
    private bool horiztonalAngleReached;
    private bool verticalAngleReached;
    private float cannonStartAngle;

    private bool playerHit;
    private int iterations;

    LineRenderer laserLine;    

    enum AttackState
    {
        Standby,
        RotateTowardsPlayer,
        FireLaser,
        ReturnToCenter,
        ReturnToOrigin
    }

    new void OnEnable()
    {
        base.OnEnable();

        laserLine = laserCannon.GetComponent<LineRenderer>();

        iterations = 0;

        InitializeValues(0);

        state = AttackState.RotateTowardsPlayer;

        playerHit = false;

        cannonStartAngle = laserCannon.localRotation.eulerAngles.x;
    }

    private void InitializeValues(int index)
    {
        // read values from scriptable object
        startDirection = attackTypes[index].startDirection;
        verticalVelocity = attackTypes[index].verticalVelocity;
        horizontalVelocity = attackTypes[index].horizontalVelocity;
        maxHorizontalDistance = attackTypes[index].maxHorizontalDistance;
        maxVerticalDistance = attackTypes[index].maxVerticalDistance;

        // update other varaibles        
        clockwiseRotation = (startDirection == RotateDirection.Clockwise);
        lastEndPoint = markerOrigin.position;
        marker.position = markerOrigin.position; // resets marker position

        rotatingFromCenter = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case AttackState.RotateTowardsPlayer:
                RotateTowardsPlayer();
                break;

            case AttackState.Standby:
                break;

            case AttackState.FireLaser:
                UpdateMarkerPosition();
                UpdateTurretRotation();
                UpdateLaserLine();
                break;

            case AttackState.ReturnToCenter:
                RotateToCenter();
                break;

            case AttackState.ReturnToOrigin:
                RotateToOrigin();
                break;
        }

    }

    private void UpdateMarkerPosition()
    {
        // calculate the horizontal distanceW
        if (clockwiseRotation) //(startDirection == RotateDirection.Clockwise)
            horizontalDistance = Mathf.Abs(marker.position.x - lastEndPoint.x);
        else
        {
            if (lastEndPoint.x < 0)
                horizontalDistance = Mathf.Abs(lastEndPoint.x) + marker.position.x;
            else
                horizontalDistance = marker.position.x - lastEndPoint.x;
        }

        verticalDistance = Mathf.Abs(marker.position.z - markerOrigin.position.z);

        marker.Translate(markerVelocity * Time.deltaTime);

        // finish attack when reaching the maxmimum distance down the road
        if (verticalDistance > maxVerticalDistance)
        {
            state = AttackState.ReturnToCenter;
            laserLine.enabled = false;
            clockwiseRotation = !clockwiseRotation;
        }
        // check if the edge of the road has been reached
        else if (horizontalDistance > maxHorizontalDistance || 
            (rotatingFromCenter && horizontalDistance > maxHorizontalDistance / 2))
        {
            clockwiseRotation = !clockwiseRotation;
            lastEndPoint = marker.position;
            rotatingFromCenter = false;

            UpdateDirection();
        }
        
    }

    private void UpdateTurretRotation()
    {
        Vector3 aimVector = marker.position - laserCannon.position;
        Vector3 aimVectorXZ = aimVector;
        aimVectorXZ.y = 0;

        // sets the horizontal rotation of the turret
        laserGunSet.rotation = Quaternion.LookRotation(aimVectorXZ.normalized, Vector3.up);

        //Debug.DrawRay(laserGunSet.position, laserGunSet.forward * 100, Color.yellow);

        // sets the vertical rotation
        Vector3 cannonPointDirection = new Vector3(0, aimVector.y, aimVectorXZ.magnitude).normalized;
        laserCannon.localRotation = Quaternion.LookRotation(cannonPointDirection, Vector3.up);


        //aimVector.Normalize();

        //Debug.DrawRay(firePoint.position, -cannonPointDirection * 100, Color.yellow);
        Debug.DrawRay(firePoint.position, aimVector * 1000, Color.green);
        Debug.DrawRay(firePoint.position, laserCannon.forward * 100, Color.yellow);
    }

    private void UpdateLaserLine()
    {
        Vector3 fireDirection = laserCannon.forward;
                                //(marker.position - firePoint.position).normalized;
        Ray raycast = new Ray(firePoint.position, fireDirection);

        laserLine.SetPosition(0, firePoint.position);        
        
        if (Physics.Raycast(raycast, out RaycastHit hitInfo))
        {
            laserLine.SetPosition(1, hitInfo.point);

            if (hitInfo.transform.tag == "Player")
            {
                if (playerHit) return;

                laserEvents.OnLaserPause?.Invoke();
                //OnPlayerHit?.Invoke();
                playerHit = true;

                hitInfo.transform.GetComponent<PlayerController>().Die();
                laserLine.enabled = false;
            }
        }
        else
        {
            Debug.Log("Laser raycast not hitting anything");
        }
    }

    private void UpdateDirection()
    {
        if(clockwiseRotation)
            markerVelocity = new Vector3(-horizontalVelocity, 0, -verticalVelocity);
        else
            markerVelocity = new Vector3(horizontalVelocity, 0, -verticalVelocity);
    }

    private void RotateTowardsPlayer()
    {
        float rotation = startDirection == RotateDirection.Clockwise ? horizontalRotateSpeed : -horizontalRotateSpeed;      
        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        float angleFromCenter = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0));

        if (angleFromCenter < 1)
        {
            laserGunSet.rotation = Quaternion.Euler(0, 180, 0);

            state = AttackState.Standby;
            laserEvents.OnLaserLock?.Invoke();
            Invoke("FireLaser", standbyTime);
        }

        if (!laserEvents.startSoundPlayed)
        {
            laserEvents.OnLaserWindup?.Invoke();
            laserEvents.startSoundPlayed = true;
        }
    }

    private void RotateToCenter()
    {
        float rotation = horizontalRotateSpeed;

        if (clockwiseRotation)
        {
            rotation = horizontalRotateSpeed;
            horiztonalAngleReached = Quaternion.Angle(laserGunSet.localRotation, Quaternion.Euler(0, 180, 0)) < 1;
        }
        else
        {
            rotation = -horizontalRotateSpeed;
            horiztonalAngleReached = Quaternion.Angle(laserGunSet.localRotation, Quaternion.Euler(0, 180, 0)) < 1;
        }

        verticalAngleReached = Quaternion.Angle(laserCannon.localRotation, Quaternion.Euler(cannonStartAngle, 0, 0)) < 1;

        // rotate the gun parts
        if (!horiztonalAngleReached)
            laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if(!verticalAngleReached)
            laserCannon.Rotate(verticalRotateSpeed * Time.deltaTime, 0, 0);

        if (horiztonalAngleReached && verticalAngleReached)
        {
            laserGunSet.localRotation = Quaternion.Euler(0, 180, 0);
            laserCannon.localRotation = Quaternion.Euler(cannonStartAngle, 0, 0);
            state = AttackState.Standby;

            iterations++;

            // begin next laser attack if there are more available
            if(iterations >= attackTypes.Length)
            {
                state = AttackState.ReturnToOrigin;
            }   
            else
            {
                InitializeValues(iterations);
                Invoke("FireLaser", standbyTime);
            }
        }

        if (!laserEvents.endSoundPlayed)
        {
            laserEvents.OnLaserEnd?.Invoke();
            laserEvents.endSoundPlayed = true;
        }
    }

    private void RotateToOrigin()
    {
        float rotation = horizontalRotateSpeed;
        horiztonalAngleReached = false;

        if (startDirection == RotateDirection.Clockwise)
        {
            rotation = horizontalRotateSpeed;
            horiztonalAngleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }
        else if (startDirection == RotateDirection.AntiClockwise)
        {
            rotation = -horizontalRotateSpeed;
            horiztonalAngleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }

        // rotate the gun parts
        if (!horiztonalAngleReached)
            laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        //if (!verticalAngleReached)
            //laserCannon.Rotate(rotateSpeed * Time.deltaTime, 0, 0);

        if (horiztonalAngleReached && verticalAngleReached)
        {
            laserGunSet.localRotation = Quaternion.identity;
            laserCannon.localRotation = Quaternion.Euler(cannonStartAngle, 0, 0);

            state = AttackState.Standby;
            AttackFinished = true;
        }

        if (!laserEvents.endSoundPlayed)
        {
            laserEvents.OnLaserEnd?.Invoke();
            laserEvents.endSoundPlayed = true;
        }
    }

    private void FireLaser()
    {
        state = AttackState.FireLaser;

        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, marker.position);
        laserLine.enabled = true;

        UpdateDirection();
        UpdateLaserLine();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(laserCannon.position, laserCannon.forward * 100);
    }

}