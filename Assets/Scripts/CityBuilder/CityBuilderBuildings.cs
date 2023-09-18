using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilderBuildings : MonoBehaviour
{

    [SerializeField] bool selectBuildingsRandomly;

    // public varables
    [Header("Spawners")]
    [SerializeField] Transform leftSpawnPoint;
    [SerializeField] Transform rightSpawnPoint;

    [Header("Building Types")]
    [SerializeField] GameObject[] leftSideBuildings;
    [SerializeField] GameObject[] rightSideBuildings;

    [Header("Initial Spawning")]
    [SerializeField] uint startingBuildingsPerSide = 12;
    [SerializeField] uint spawnOffset = 10;

    [Header("Continued Spawning")]
    [Range(0, 1)]
    [SerializeField] float timeBetweenBuldings = 0.5f;

    // private variables
    float timeSinceLastSpawn = 0;
    uint leftBuildingIndex = 0;
    uint rightBuildingIndex = 0;

    void Start()
    {
        for (int i = 0; i < startingBuildingsPerSide; i++)
        {
            SpawnBuildingOnLeft(i * spawnOffset);
            SpawnBuildingOnRight(i * spawnOffset);
        }
    }

    void Update()
    {   
        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn > timeBetweenBuldings) 
        {
            SpawnBuildingOnLeft(0);
            SpawnBuildingOnRight(0);

            timeSinceLastSpawn = 0;
        }
        
    }

    // Spawns a building on the left-hand side of the road
    private void SpawnBuildingOnLeft(float offset)
    {
        if(selectBuildingsRandomly)
        {
            int randomPick = Random.Range(0, leftSideBuildings.Length);
            Instantiate(leftSideBuildings[randomPick], leftSpawnPoint.position + Vector3.back * offset,
                        leftSideBuildings[randomPick].transform.rotation, leftSpawnPoint);
        }
        else
        {
            Instantiate(leftSideBuildings[leftBuildingIndex], leftSpawnPoint.position + Vector3.back * offset, 
                        leftSideBuildings[leftBuildingIndex].transform.rotation, leftSpawnPoint);

            if (!selectBuildingsRandomly)
            {
                leftBuildingIndex++;

                if (leftBuildingIndex == leftSideBuildings.Length)
                    leftBuildingIndex = 0;
            }
        }
        
    }

    // Spawns a building on the right-hand side of the road
    private void SpawnBuildingOnRight(float offset)
    {
        if (selectBuildingsRandomly)
        {
            int randomPick = Random.Range(0, rightSideBuildings.Length);
            Instantiate(rightSideBuildings[randomPick], rightSpawnPoint.position + Vector3.back * offset,
                        rightSideBuildings[randomPick].transform.rotation, rightSpawnPoint);
        }
        else
        {
            Instantiate(rightSideBuildings[rightBuildingIndex], rightSpawnPoint.position + Vector3.back * offset,
                        rightSideBuildings[rightBuildingIndex].transform.rotation, rightSpawnPoint);

            if (!selectBuildingsRandomly)
            {
                rightBuildingIndex++;

                if (rightBuildingIndex == leftSideBuildings.Length)
                    rightBuildingIndex = 0;
            }
        }

    }

}
