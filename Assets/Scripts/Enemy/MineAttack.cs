using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineAttack : EnemyAttack
{
    public GameObject minePrefab;

    [SerializeField] uint minesToSpawn = 10;
    [SerializeField] float timeBetweenMines = 1;    
    [SerializeField] bool selectSpawnPointsRandomly;
    [SerializeField] bool dropMinesSimulatenously;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform[] dropPoints;
    [SerializeField] float sidewaysMovement;

    float timeSinceLastMine = 0;
    uint minesSpawned = 0;

    private int spawnPointNumber = -1;
    private bool descending = false;
    private bool atDropPoint = false;
    private int pointChosen;

    private void OnEnable()
    {
        pointChosen = PickNextPoint();
    }

    void Update()
    {
        timeSinceLastMine += Time.deltaTime;

        if (dropMinesSimulatenously)
        {
            if (timeSinceLastMine >= timeBetweenMines)
            {
                foreach (Transform dropPoint in dropPoints)
                {
                    Instantiate(minePrefab, dropPoint.position, Quaternion.identity);
                    minesSpawned++;
                }
            }
        }
        else
        {
            if (atDropPoint)
            {
                if (timeSinceLastMine >= timeBetweenMines)
                {
                    timeSinceLastMine = 0;
                    Instantiate(minePrefab, spawnPoint.position, Quaternion.identity);
                    atDropPoint = false;
                    minesSpawned++;

                    pointChosen = PickNextPoint();
                }

                if (minesSpawned >= minesToSpawn)
                {
                    AttackFinished = true;
                    this.enabled = false;
                }
            }
            else
            {
                MoveToPosition(dropPoints[pointChosen]);
            }
        }
    }

    private void MoveToPosition(Transform dropPoint)
    {
        if(transform.position.x < dropPoint.position.x)
        {
            transform.Translate(Vector3.right * sidewaysMovement * Time.deltaTime);

            if (transform.position.x >= dropPoint.position.x)
                atDropPoint = true;

        }
        else if(transform.position.x > dropPoint.position.x)
        {
            transform.Translate(Vector3.left * sidewaysMovement * Time.deltaTime);

            if (transform.position.x <= dropPoint.position.x)
                atDropPoint = true;
        }
        
    }

    private int PickNextPoint()
    {        
        if(selectSpawnPointsRandomly)
        {
            int randomPick;
            do {
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
