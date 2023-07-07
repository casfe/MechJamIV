using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineAttack : EnemyAttack
{
    public GameObject minePrefab;

    [SerializeField] float minesToSpawn;
    [SerializeField] float timeBetweenMines;
    [SerializeField] Vector3[] spawnPoints;

    float timeSinceLastMine = 0;

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastMine += Time.deltaTime;

        if(timeSinceLastMine >= timeBetweenMines)
        {
            timeSinceLastMine = 0;
        }
    }
}
