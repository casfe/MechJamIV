using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumerations
public enum RotateDirection { Clockwise, AntiClockwise };

[CreateAssetMenu(fileName = "Laser Attack 1", menuName = "Enemy Attack/New Laser Attack", order = 1)]
public class LaserAttackValues : ScriptableObject
{
    [Header("Turret Rotation")]
    public RotateDirection startDirection = RotateDirection.Clockwise;

    [Header("Target Movement")]
    [Tooltip("The speed in which the laser will move down the road")]
    public float verticalVelocity = 10;
    [Tooltip("The speed in which the laser will move across the road")]
    public float horizontalVelocity = 10;
    [Tooltip("The distance in which the laser will move across the road before the turret changes direction")]
    public float maxHorizontalDistance = 20;
    [Tooltip("The distance in which the laser will down across the road before the attack finishes")]
    public float maxVerticalDistance = 50;
}
