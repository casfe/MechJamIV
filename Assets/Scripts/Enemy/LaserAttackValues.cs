using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser Attack 1", menuName = "Enemy Attack/Laser Attack", order = 1)]
public class LaserAttackValues : ScriptableObject
{
    [Tooltip("The number of times the laser will change direction before the gun stops firing")]
    public int fireIterations = 3;

    [Header("Cannon Horizontal Rotation")]
    [Tooltip("The gun's initial direction of rotation")]
    public LaserAttack.RotateDirection startDirection = LaserAttack.RotateDirection.Clockwise;
    [Tooltip("The speed in which the gun rotates to face the player")]
    public float rotateSpeed = 100;
    [Tooltip("The speed in which the gun rotates horizontally while firing the laser")]
    public float fireRotationSpeed = 20;
    [Tooltip("The total range in which the rotates horizontally while firing the laser")]
    [Range(0, 180)] public float fireAngle = 20;

    [Header("Cannon Veritcal Rotation")]
    [Tooltip("The speed in which the gun rotates vertically as the laser gets close to the player")]
    public float raiseSpeed = 6;
    [Tooltip("The speed in which the gun rotates back down once it has finished firing the laser")]
    public float descendSpeed = 10;
    [Tooltip("The Maximum angle that the laser gun can rotate upwards")]
    [Range(0, 90)]
    public float raiseAngle = 7.5f;

    [Header("Limit on final iteration")]
    public bool setLimit;
    public float limitAngle;
}
