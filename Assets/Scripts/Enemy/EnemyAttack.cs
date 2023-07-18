using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    public bool AttackFinished { get; protected set; }

    protected virtual void OnEnable()
    {
        AttackFinished = false;
    }
}

public class LaneSwitchAttack : EnemyAttack
{
    [Header("Lane Switch attack")]
    [SerializeField] protected int sidewaysMovementSpeed = 10;
    [SerializeField] bool selectLanesRandomly = false;
    [SerializeField] protected Transform[] lanePositions;

    protected bool atLanePosition;
    private bool descending = false;
    private int spawnPointNumber = -1;

    protected void MoveToPosition(int laneNumber)
    {
        Vector3 targetPosition = lanePositions[laneNumber].position;

        if (transform.position.x < targetPosition.x)
        {
            transform.Translate(Vector3.right * sidewaysMovementSpeed * Time.deltaTime);

            if (transform.position.x >= targetPosition.x)
                atLanePosition = true;

        }
        else if (transform.position.x > targetPosition.x)
        {
            transform.Translate(Vector3.left * sidewaysMovementSpeed * Time.deltaTime);

            if (transform.position.x <= targetPosition.x)
                atLanePosition = true;
        }

    }

    protected int PickNextPoint()
    {
        if (selectLanesRandomly)
        {
            int randomPick;
            do
            {
                randomPick = Random.Range(0, 3);
            } while (randomPick == 3);

            return randomPick;
        }
        else
        {
            if (descending)
            {
                if (spawnPointNumber > 0)
                    spawnPointNumber--;
                else
                {
                    spawnPointNumber++;
                    descending = false;
                }
            }
            else
            {
                if (spawnPointNumber < 2)
                    spawnPointNumber++;
                else
                {
                    spawnPointNumber--;
                    descending = true;
                }
            }

            return spawnPointNumber;
        }
    }
}