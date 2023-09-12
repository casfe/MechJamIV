using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 100;
    [Tooltip("The amount of damage to take when hit my the player's weapon")]
    [SerializeField] int damagePerBlast = 10;
    [SerializeField] float initialTimeBetweenAttacks = 5.0f;
    [Tooltip("Used when the mech returns to the center after completing an attack")]
    [SerializeField] float sidewaysWalkSpeed = 10;
    [SerializeField] bool selectAttacksRandomly = true;
    
    private EnemyAttack[] enemyAttackList = new EnemyAttack[4];

    private int health;
    private float timeBetweenAttacks;
    private float timeSinceLastAttack;
    private bool returningToCenter = false;

    private EnemyAttack currentAttack = null;
    private EnemyAttack[] attackSequence;
    private uint attackNumber;

    GameManager gameManager;

    [Header("FMOD Events")]
    public UnityEvent<float> OnDamageTaken;

    void Start()
    {
        health = (int)maxHealth;
        timeBetweenAttacks = initialTimeBetweenAttacks;

        enemyAttackList[0] = GetComponent<MineAttack>();
        enemyAttackList[1] = GetComponent<RocketAttack>();
        enemyAttackList[2] = GetComponent<MachineGunAttack>();
        enemyAttackList[3] = GetComponent<LaserAttack>();
        
        gameManager = GameManager.Instance;

        if (selectAttacksRandomly)
            CreateAttackSequence();
        else
            attackSequence = enemyAttackList;
    }

    void Update()
    {
        if(returningToCenter)
        {
            ReturnToCenter();
        }
        else if (currentAttack == null)
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                currentAttack = attackSequence[attackNumber];
                currentAttack.enabled = true;
            }

        }
        else if (currentAttack.AttackFinished)
        {
            currentAttack.enabled = false;
            returningToCenter = true;
            timeSinceLastAttack = 0;
            currentAttack = null;

            if (attackNumber < attackSequence.Length - 1)
            {
                attackNumber++;
            }
            else
            {
                attackNumber = 0;

                if (selectAttacksRandomly)
                    CreateAttackSequence();
                else
                    attackSequence = enemyAttackList;
                
            }
            
        }        

    }

    private void CreateAttackSequence()
    {
        List<EnemyAttack> attacksAvailable = enemyAttackList.ToList<EnemyAttack>();
        attackSequence = new EnemyAttack[4];

        for (int i=0; i < 4; i++)
        {
            int randomPick = UnityEngine.Random.Range(0, attacksAvailable.Count-1);
            attackSequence[i] = attacksAvailable[randomPick];

            attacksAvailable.RemoveAt(randomPick);
        }
    }

    private void ReturnToCenter()
    {
        if(transform.position.x > 0)
        {
            transform.Translate(Vector3.left * sidewaysWalkSpeed * Time.deltaTime);

            if(transform.position.x <= 0)
            {
                transform.position.Set(0, transform.position.x, transform.position.y);
                returningToCenter = false;
            }
        }
        else if(transform.position.x < 0)
        {
            transform.Translate(Vector3.right * sidewaysWalkSpeed * Time.deltaTime);

            if (transform.position.x >= 0)
            {
                transform.position.Set(0, transform.position.x, transform.position.y);
                returningToCenter = false;
            }
        }
        else
        {
            returningToCenter = false;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        gameManager.SetEnemyHealthBar(health, maxHealth);

        OnDamageTaken?.Invoke((float)health);

        if (health <= 0)
            gameManager.WinGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8) // projectile layer
        {
            Debug.Log("Bullet hit mech");
            GameObject.Destroy(other.gameObject);
            TakeDamage(damagePerBlast);
        }
    }

}