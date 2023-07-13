using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    uint maxHealth = 100;
    [SerializeField]
    float initialTimeBetweenAttacks = 5.0f;
    
    private List<EnemyAttack> enemyAttackList = new List<EnemyAttack>();

    private int health;
    private float timeBetweenAttacks;
    private float timeSinceLastAttack;

    private EnemyAttack currentAttack = null;
    private EnemyAttack[] attackSequence;
    private uint attackNumber;

    // Start is called before the first frame update
    void Start()
    {
        health = (int)maxHealth;
        timeBetweenAttacks = initialTimeBetweenAttacks;

        enemyAttackList.Add(GetComponent<RocketAttack>());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(currentAttack == null)
            timeSinceLastAttack += Time.deltaTime;
        else if (currentAttack.AttackFinished)
        {
            currentAttack.enabled = false;

            if(attackNumber < attackSequence.Length -1)
            {
                attackNumber++;                
            }
            else
            {
                attackNumber = 0;
                CreateAttackSequence();
            }

            timeSinceLastAttack = 0;
            currentAttack = null;
        }

        if (timeSinceLastAttack >= timeBetweenAttacks)
        {
            attackSequence[attackNumber].enabled = true;
        }*/
    }

    private void CreateAttackSequence()
    {
        List<EnemyAttack> attacksAvailable = enemyAttackList;

        for (int i=0; i < 4; i++)
        {
            int randomPick = UnityEngine.Random.Range(0, attacksAvailable.Count-1);
            attackSequence[i] = attacksAvailable[randomPick];

            attacksAvailable.RemoveAt(randomPick);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }

}

public abstract class EnemyAttack : MonoBehaviour
{
    public bool AttackFinished { get; protected set; } = false;
}