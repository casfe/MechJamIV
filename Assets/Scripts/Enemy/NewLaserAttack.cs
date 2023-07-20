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

    [Header("Turret Values")] 
    [SerializeField] float standbyTime = 2.0f;
    [SerializeField] RotateDirection startDirection = RotateDirection.Clockwise;
    [SerializeField] float rotateSpeed = 100;

    [Header("Marker Movement ")]
    [SerializeField] float verticalVelocity = 1;
    [SerializeField] float horizontalVelocity = 1;
    [SerializeField] float maxHorizontalDistance = 1;
    [SerializeField] float maxVerticalDistance = 5;

    [Header("Events")]
    [SerializeField] public LaserEvents laserEvents;

    [SerializeField] private float horizontalDistance = 0;
    [SerializeField] private float verticalDistance = 0;

    private Transform marker;
    private Vector3 markerVelocity;
    private bool clockwiseRotation;

    private Vector3 lastEndPoint;
    private AttackState state;
    private bool angleReached;

    private bool playerHit;

    LineRenderer laserLine;

    // Enumerations
    public enum RotateDirection { Clockwise, AntiClockwise };

    enum AttackState
    {
        Standby,
        RotateTowardsPlayer,
        FireLaser,
        ReturnToCenter,
        ReturnToOrigin
    }

    void OnEnable()
    {
        laserLine = laserGunSet.GetComponent<LineRenderer>();

        marker = Instantiate(markerPrefab, markerSpawnPoint.position, Quaternion.identity).transform;

        lastEndPoint = markerSpawnPoint.position;
        lastEndPoint.x += (maxHorizontalDistance /2);

        clockwiseRotation = startDirection == RotateDirection.Clockwise;

        state = AttackState.RotateTowardsPlayer;

        playerHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AttackState.RotateTowardsPlayer:
                RotateTowardsPlayer();
                break;

            case AttackState.FireLaser:
                UpdateMarkerPosition();
                UpdateTurretRotation();
                UpdateLaserLine();
                break;

            case AttackState.ReturnToCenter:
                //ReturnToCenter();
                break;

            case AttackState.ReturnToOrigin:
                RotateToOrigin();
                break;
        }

    }

    private void UpdateMarkerPosition()
    {
        horizontalDistance = Mathf.Abs(marker.position.x - lastEndPoint.x);
        verticalDistance = Mathf.Abs(marker.position.z - markerSpawnPoint.position.z);

        marker.Translate(markerVelocity * Time.deltaTime);

        if (horizontalDistance > maxHorizontalDistance)
        {
            clockwiseRotation = !clockwiseRotation;
            lastEndPoint = marker.position;

            UpdateDirection();
        }

        if (verticalDistance > maxVerticalDistance)
        {
            state = AttackState.ReturnToOrigin;
            laserLine.enabled = false;
        }
    }

    private void UpdateTurretRotation()
    {
        Vector3 aimVector = marker.position - laserCannon.position;
        Vector3 aimVectorXZ = aimVector;
        aimVectorXZ.y = 0;

        laserGunSet.rotation = Quaternion.LookRotation(aimVectorXZ.normalized, Vector3.up);

        Vector3 cannonLookDir = new Vector3(0, aimVector.y, aimVectorXZ.magnitude);
        laserCannon.localRotation = Quaternion.LookRotation(cannonLookDir.normalized, Vector3.up);
    }

    private void UpdateLaserLine()
    {
        Vector3 fireDirection = (marker.position - firePoint.position).normalized; //firePoint.forward;
        Ray raycast = new Ray(firePoint.position, fireDirection);

        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, marker.position);
        
        if (Physics.Raycast(raycast, out RaycastHit hitInfo))
        {
            //laserLine.SetPosition(1, hitInfo.point);

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

        if (Quaternion.Angle(laserGunSet.rotation, Quaternion.Euler(0, 180, 0)) < 1)
        {
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

    private void RotateToOrigin()
    {
        float rotation = rotateSpeed;
        angleReached = false;

        if (startDirection == RotateDirection.Clockwise)
        {
            rotation = -rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }
        else if (startDirection == RotateDirection.AntiClockwise)
        {
            rotation = rotateSpeed;
            angleReached = Quaternion.Angle(laserGunSet.rotation, Quaternion.identity) < 1;
        }

        laserGunSet.Rotate(0, rotation * Time.deltaTime, 0, Space.Self);

        if (angleReached)
        {
            laserCannon.rotation = Quaternion.identity;
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
        UpdateDirection();
    }

}