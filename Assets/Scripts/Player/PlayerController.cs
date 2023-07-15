using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int maxWeaponGauage = 100;
    [SerializeField] int gaugeIncreaseFromPickup = 20;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;    

    public UnityEvent OnCollideWithObstacle;

    Animator animator;
    GameManager gameManager;

    private int weaponGauge = 0;

    private void Start()
    {
        gameManager = GameManager.Instance;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FireWeapon();
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) // hazard layer
        {
            OnCollideWithObstacle.Invoke();
            animator.SetTrigger("Die");
            GetComponent<PlayerMovement>().enabled = false;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            weaponGauge += gaugeIncreaseFromPickup;

            if(weaponGauge > maxWeaponGauage)
                weaponGauge = maxWeaponGauage;

            gameManager.SetWeaponGauge(weaponGauge, maxWeaponGauage);

            GameObject.Destroy(other.gameObject);
        }
    }

    private void FireWeapon()
    {
        if (weaponGauge < maxWeaponGauage)
            Debug.Log("Gauge too low");
        else
        {
            weaponGauge = 0;
            gameManager.SetWeaponGauge(weaponGauge, maxWeaponGauage);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletPrefab.transform.rotation);            
        }
    }
}