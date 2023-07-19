using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int maxWeaponGauage = 100;
    [SerializeField] int startingWeaponGauge = 0;
    [SerializeField] int gaugeIncreaseFromPickup = 20;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;   

    [Header("FMOD Event paths")]
    [SerializeField] string weaponShoot;
    [SerializeField] string weaponPickup;
    [SerializeField] string weaponReady;

    [Header("Events")]
    public UnityEvent onCollisionWithObstacle;
    public UnityEvent onShootWeapon;
    public UnityEvent onPickupWeapon;
    public UnityEvent onWeaponReady;

    Animator animator;
    GameManager gameManager;

    private int weaponGauge = 0;

    private void Start()
    {
        gameManager = GameManager.Instance;
        animator = GetComponentInChildren<Animator>();

        weaponGauge = startingWeaponGauge;
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
            GameObject.Destroy(collision.gameObject);

            onCollisionWithObstacle.Invoke();
            animator.SetTrigger("Die");
            GetComponent<PlayerMovement>().enabled = false;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // hazard layer
        {
            GameObject.Destroy(other.gameObject);

            onCollisionWithObstacle.Invoke();
            animator.SetTrigger("Die");
            GetComponent<PlayerMovement>().enabled = false;
            this.enabled = false;
        }
        else if (other.gameObject.tag == "Weapon")
        {
            GameObject.Destroy(other.gameObject);
            weaponGauge += gaugeIncreaseFromPickup;

            if(weaponGauge > maxWeaponGauage) 
            {
                weaponGauge = maxWeaponGauage;
                onWeaponReady.Invoke();

                FMODUtilities.PlayOneShotUsingString(weaponReady);
            }
            else
            {
                FMODUtilities.PlayOneShotUsingString(weaponPickup);
                onPickupWeapon.Invoke();
            }

            gameManager.SetWeaponGauge(weaponGauge, maxWeaponGauage);            
        }
    }

    private void FireWeapon()
    {
        if (weaponGauge < maxWeaponGauage)
            Debug.Log("Gauge too low");
        //this should play a sound
        else
        {
            weaponGauge = 0;
            gameManager.SetWeaponGauge(weaponGauge, maxWeaponGauage);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletPrefab.transform.rotation);

            onShootWeapon.Invoke();
            FMODUtilities.PlayOneShotUsingString(weaponShoot);
        }
    }
}