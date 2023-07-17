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
    [SerializeField] Transform[] spawnPoints;

    float timeSinceLastMine = 0;
    uint minesSpawned = 0;

    private int spawnPointNumber = 0;
    private bool descending = false;

    [Header("FMOD Event Paths")]
    [SerializeField] string mineDrop;

    private void OnEnable()
    {
        DropMine();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastMine += Time.deltaTime;

        if(timeSinceLastMine >= timeBetweenMines)
        {
            timeSinceLastMine = 0;
            DropMine();
        }

        if(minesSpawned >= minesToSpawn)
        {
            AttackFinished = true;
            this.enabled = false;
        }
    }

    private void DropMine()
    {
        if (dropMinesSimulatenously)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(minePrefab, spawnPoint.position, Quaternion.identity);
                minesSpawned++;

                FMODUtilities.PlayOneShotUsingString(mineDrop);
            }
        }
        else if(selectSpawnPointsRandomly)
        {
            int randomPick;
            do {
                randomPick = Random.Range(0, 3);
            } while (randomPick == 3);

            Instantiate(minePrefab, spawnPoints[randomPick].position, Quaternion.identity);

            minesSpawned++;

            FMODUtilities.PlayOneShotUsingString(mineDrop);
        }
        else
        {
            Instantiate(minePrefab, spawnPoints[spawnPointNumber].position, Quaternion.identity);

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

            minesSpawned++;

            FMODUtilities.PlayOneShotUsingString(mineDrop);
        }
    }
}
