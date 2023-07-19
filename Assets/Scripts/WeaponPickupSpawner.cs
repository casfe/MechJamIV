using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupSpawner : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform[] spawnPoints;  
    [SerializeField] bool selectPointsRandomly = true;
    [Header("Time Delay Between Spawns")]
    [Tooltip("Selects a random delay minimum and maximum values. If unchecked the minumum value will always be used.")]
    [SerializeField] bool randomTimeDelay = false;
    [SerializeField] float minTimeDelay = 3;
    [SerializeField] float maxTimeDelay = 10;

    private float timeSinceSpawn = 0;
    private Transform spawnPoint;
    private int spawnPointIndex = 0;
    private float nextTimeDelay;

    // Start is called before the first frame update
    void Start()
    {
        SelectNextSpawnPoint();

        if (randomTimeDelay)
        {
            nextTimeDelay = Random.Range(minTimeDelay, maxTimeDelay);
            //Debug.Log("Next time delay: " + nextTimeDelay);
        }
        else
            nextTimeDelay = minTimeDelay;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if(timeSinceSpawn > nextTimeDelay)
        {
            Instantiate(weaponPrefab, spawnPoint.position, Quaternion.identity);
            timeSinceSpawn = 0;
            SelectNextSpawnPoint();

            if (randomTimeDelay)
            {
                nextTimeDelay = Random.Range(minTimeDelay, maxTimeDelay);
                //Debug.Log("Next time delay: " + nextTimeDelay);
            }
            else
                nextTimeDelay = minTimeDelay;
        }

    }

    private void SelectNextSpawnPoint()
    {
        if(selectPointsRandomly)
        {
            float pick = Random.Range(0, spawnPoints.Length);
            //Debug.Log("Pick value: " + pick + ", Lane Chosen: " + (int)pick);
            spawnPoint = spawnPoints[(int)pick];
        }
        else
        {
            spawnPoint = spawnPoints[spawnPointIndex];

            spawnPointIndex++;

            if (spawnPointIndex >= spawnPoints.Length)
                spawnPointIndex = 0;
        }
    }

}
