using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunAttack : EnemyAttack
{
    public Transform machineGun;
    [SerializeField] float machineGunRotateSpeed = 0.5f;
    [SerializeField] float machineGunDamage;

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        machineGun.Rotate(0, machineGunRotateSpeed, 0);
    }
}
