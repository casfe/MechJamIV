using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MineAttack : LaneSwitchAttack
{
    public GameObject minePrefab;

    [SerializeField] uint minesToSpawn = 10;
    [SerializeField] float timeBetweenMines = 1;    
    [SerializeField] bool selectSpawnPointsRandomly;
    [SerializeField] bool dropMinesSimulatenously;
    [SerializeField] Transform spawnPoint;

    float timeSinceLastMine = 0;
    uint minesSpawned = 0;

    private int pointChosen;

    public UnityEvent OnMineDropped;

    protected override void OnEnable()
    {
        base.OnEnable();
        timeSinceLastMine = 0;
        minesSpawned = 0;
        pointChosen = PickNextPoint();
    }

    void Update()
    {
        timeSinceLastMine += Time.deltaTime;

        if (dropMinesSimulatenously)
        {
            if (timeSinceLastMine >= timeBetweenMines)
            {
                foreach (Transform dropPoint in lanePositions)
                {
                    Instantiate(minePrefab, dropPoint.position, Quaternion.identity);
                    minesSpawned++;
                }
                OnMineDropped?.Invoke();
            }
        }
        else
        {
            if (atLanePosition)
            {
                if (timeSinceLastMine >= timeBetweenMines)
                {
                    timeSinceLastMine = 0;
                    Instantiate(minePrefab, spawnPoint.position, Quaternion.identity);
                    atLanePosition = false;
                    minesSpawned++;
                    OnMineDropped?.Invoke();

                    pointChosen = PickNextPoint();
                }

                if (minesSpawned >= minesToSpawn)
                {
                    AttackFinished = true;
                }
            }
            else
            {
                MoveToPosition(pointChosen);
            }
        }
    }

}
