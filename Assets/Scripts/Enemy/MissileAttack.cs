using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAttack : EnemyAttack
{
    public GameObject missilePrefab;

    [Header("Spawn Positions")]
    [SerializeField] Vector3 missle1Offset;
    [SerializeField] Vector3 missle2Offset;
    [SerializeField] Vector3 missle3Offset;

    private GameObject missile1, missile2, missile3;

    private void OnEnable()
    {
        missile1 = Instantiate(missilePrefab, transform.position + missle1Offset, Quaternion.identity);
        missile2 = Instantiate(missilePrefab, transform.position + missle2Offset, Quaternion.identity);
        missile3 = Instantiate(missilePrefab, transform.position + missle3Offset, Quaternion.identity);
    }

    private void Update()
    {
        if (missile1 == null && missile2 == null && missile3 == null)
            AttackFinished = true;
    }

}
