using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLaserAttack : EnemyAttack
{
    [Header("Game Objects and transform")]
    public Transform laserGunSet;
    public Transform laserCannon;
    public Transform firePoint;
    [SerializeField] GameObject markerPrefab;
    [SerializeField] Transform markerSpawnPoint;

    [SerializeField] NewLaserAttackValues[] attackTypes;

    [Header("Events")]
    [SerializeField] public LaserEvents laserEvents;

    // attack stats
    //[Header("Turret Values")]
    float standbyTime = 2.0f;
    RotateDirection startDirection = RotateDirection.Clockwise;
    float rotateSpeed = 100;

    //[Header("Marker Movement ")]
    float verticalVelocity = 1;
    float horizontalVelocity = 1;
    float maxHorizontalDistance = 1;
    float maxVerticalDistance = 5;

    //[Header("For Debugging")]
    private float horizontalDistance = 0;
    private float verticalDistance = 0;

    private Transform marker;
    private Vector3 markerVelocity;
    private bool clockwiseRotation;
    private bool rotatingFromCenter;

    private Vector3 lastEndPoint;
    private AttackState state;
    private bool angleReached;

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

        laserLine = laserGunSet.GetComponent<LineRenderer>();
        marker = Instantiate(markerPrefab, markerSpawnPoint.position, Quaternion.identity).transform;

        iterations = 0;

        InitializeValues(0);

        state = AttackState.RotateTowardsPlayer;

        playerHit = false;
    }

    private void InitializeValues(int index)
    {
        // read values from scriptable object
        standbyTime = attackTypes[index].standbyTime;
        startDirection = attackTypes[index].startDirection;
        rotateSpeed = attackTypes[index].defaultRotateSpeed;

        verticalVelocity = attackTypes[index].verticalVelocity;
        horizontalVelocity = attackTypes[index].horizontalVelocity;
        maxHorizontalDistance = attackTypes[index].maxHorizontalDistance;
        maxVerticalDistance = attackTypes[index].maxVerticalDistance;

        // update other varaibles        
        clockwiseRotation = (startDirection == RotateDirection.Clockwise);
        lastEndPoint = markerSpawnPoint.position;
        marker.transform.position = markerSpawnPoint.position; // resets marker position

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

        verticalDistance = Mathf.Abs(marker.position.z - markerSpawnPoint.position.z);

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
        Vector3 aimVector = marker.position - firePoint.position;
        Vector3 aimVectorXZ = aimVector;
        aimVectorXZ.y = 0;

        // sets the horizontal rotation of the turret
        laserGunSet.rotation = Quaternion.LookRotation(aimVectorXZ.normalized, Vector3.up);

        //Debug.DrawRay(laserGunSet.position, laserGunSet.forward * 100, Color.yellow);

        // sets the vertical rotation
        Vector3 cannonPointDirection = new Vector3(0, aimVector.y, aimVectorXZ.magnitude).normalized;
        laserCannon.localRotation = Quaternion.LookRotation(cannonPointDirection, Vector3.up);

        //Debug.DrawRay(firePoint.position, -cannonPointDirection * 100, Color.yellow);
        //Debug.DrawRay(firePoint.position, laserCannon.forward * 100, Color.yellow);
    }

    private void UpdateLaserLine()
    {
        Vector3 fireDirection = laserCannon.forward;
                                //(marker.position - firePoint.position).normalized;
        Ray raycast = new Ray(firePoint.position, fireDirection);

        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, firePoint.position + fireDirection * 100);
        
        if (Physics.Raycast(raycast, out RaycastHit hitInfo))
        {
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
        float rotation = startDirection == RotateDirection.Clockwise ? rotateSpeed : -rotateSpeed;      
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
        float rotation = rotateSpeed;
        angleReached = false;

        if (clockwiseRotation)
        {
            rotation = rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1;
        }
        else
        {
            rotation = -rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1;
        }

        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if (angleReached)
        {
            laserCannon.rotation = Quaternion.Euler(0, 180, 0);
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
        float rotation = rotateSpeed;
        angleReached = false;

        if (startDirection == RotateDirection.Clockwise)
        {
            rotation = rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }
        else if (startDirection == RotateDirection.AntiClockwise)
        {
            rotation = -rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }

        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if (angleReached)
        {
            laserCannon.rotation = Quaternion.identity;
            state = AttackState.Standby;
            AttackFinished = true;

            GameObject.Destroy(marker);
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

}