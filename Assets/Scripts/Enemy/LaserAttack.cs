using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : EnemyAttack
{
    public Transform laserGun;
    [SerializeField] float rotateSpeed = 0.5f;
    [SerializeField] float fireRotationSpeed = 1.0f;
    [SerializeField] float minAngle = 135f;
    [SerializeField] float maxAnagle = 225f;
    [SerializeField] int fireIterations = 3;
    [SerializeField] float standbyTime = 2.0f;

    private RotationState state = RotationState.RotateTowardsPlayer;

    [SerializeField]
    private int iteration = 0;

    enum RotationState
    {
        Standby,
        RotateTowardsPlayer,
        RotateClockwise,
        RotateAntiClockwise,
        RotateToOrigin
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        switch(state)
        {
            case RotationState.RotateTowardsPlayer:
                laserGun.Rotate(0, rotateSpeed, 0);

                if (laserGun.transform.rotation.eulerAngles.y >= 180)
                {
                    state = RotationState.Standby;
                    Invoke("FireLaser", standbyTime);
                }
            break;

            case RotationState.RotateClockwise:
                laserGun.Rotate(0, fireRotationSpeed, 0);

                if (laserGun.transform.rotation.eulerAngles.y >= maxAnagle)
                {
                    iteration++;

                    if (iteration > fireIterations)
                        state = RotationState.RotateToOrigin;
                    else
                        state = RotationState.RotateAntiClockwise;
                }
            break;

            case RotationState.RotateAntiClockwise:
                laserGun.Rotate(0, -fireRotationSpeed, 0);

                if (laserGun.transform.rotation.eulerAngles.y <= minAngle)
                {
                    iteration++;
                    if (iteration > fireIterations)
                        state = RotationState.RotateToOrigin;
                    else
                        state = RotationState.RotateClockwise;
                }
            break;

            case RotationState.RotateToOrigin:
                laserGun.Rotate(0, -rotateSpeed, 0);

                if (laserGun.transform.rotation.y <= 0)
                    state = RotationState.Standby;
           break;
        }
    }

    private void FireLaser()
    {
        iteration++;
        state = RotationState.RotateClockwise;
    }
}
