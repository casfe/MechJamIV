using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region public and exposed variables
    [Header("--- Object Spawn Points ---")]
    [SerializeField] Transform leftSpawnPoint;
    [SerializeField] Transform rightSpawnPoint;
    [SerializeField] Transform centerSpawnPoint;

    [Header("--- Road Spawning ---")]
    [SerializeField] bool spawnRoads = true;
    [SerializeField] GameObject roadPrefab;
    [SerializeField] Transform roadsParent;
    [SerializeField] int startRoadsCounter = 5;
    [SerializeField] float timeToSpawnRoad = 1.5f;
    [SerializeField] float roadSpawnOffset = 48;

    [Header("--- Building Spawning ---")]
    [SerializeField] bool spawnBuildings = true;

    [Header("Building Types")]
    [SerializeField] bool selectBuildingsRandomly = true;
    [SerializeField] GameObject[] leftSideBuildings;
    [SerializeField] GameObject[] rightSideBuildings;

    [Header("Building timers and offsets")]
    [SerializeField] uint startingBuildingsPerSide = 12;
    [SerializeField] uint buildingSpawnOffset = 18;
    [Range(0, 1)]
    [SerializeField] float timeBetweenBuildings = 0.5f;

    [Header("--- Intersecting Objects ---")]
    [SerializeField] bool spawnIntersections;
    [SerializeField] GameObject intersection;    
    [SerializeField] bool spawnBridges;
    [SerializeField] GameObject bridgePrefab;
    [SerializeField] int minSpawnFrequency = 5;
    [SerializeField] int maxSpawnFrequency = 10;
    #endregion

    #region private variables
    // roads
    GameObject lastRoad;
    Vector3 spawnedRoadPosition;

    // buildings
    float timeSinceLastSpawn = 0;
    uint leftBuildingIndex = 0;
    uint rightBuildingIndex = 0;
    uint objectIndex = 0;

    private int buildingsSinceIntersection = 0;
    private int buildingsTillNextIntersection;
    #endregion

    #region start and update
    // Start is called before the first frame update
    void Start()
    {        
        // spawn starting roads
        if(spawnRoads)
        {
            // spawn starting roads
            for (int i = 0; i < startRoadsCounter; i++)
            {
                SpawnNewRoad();
            }
        }

        // spawn starting buildings
        if(spawnBuildings)
        {
            for (int i = 0; i < startingBuildingsPerSide; i++)
            {
                SpawnBuildingOnLeft(i * buildingSpawnOffset);
                SpawnBuildingOnRight(i * buildingSpawnOffset);
                buildingsSinceIntersection++;
            }

            buildingsTillNextIntersection = Random.Range(minSpawnFrequency, maxSpawnFrequency);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(spawnRoads)
        {
            // if the road has moved far enough it is time to spawn another one
            if (spawnedRoadPosition.z - lastRoad.transform.position.z >= roadSpawnOffset)
            {
                SpawnNewRoad();
            }
        }

        if(spawnBuildings)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn > timeBetweenBuildings)
            {
                if (buildingsSinceIntersection >= buildingsTillNextIntersection)
                {
                    SpawnIntersectingObject();
                    buildingsTillNextIntersection = Random.Range(minSpawnFrequency, maxSpawnFrequency);
                }
                else
                {
                    SpawnBuildingOnLeft(0);
                    SpawnBuildingOnRight(0);
                    buildingsSinceIntersection++;
                }

                timeSinceLastSpawn = 0;
            }
        }
    }
    #endregion

    #region functions
    
    // Road spawning
    private void SpawnNewRoad()
    {
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(lastRoad.transform.position.x, lastRoad.transform.position.y, lastRoad.transform.position.z + roadSpawnOffset), Quaternion.identity);
        newRoad.transform.parent = roadsParent;
        spawnedRoadPosition = newRoad.transform.position;

        lastRoad = newRoad;
    }

    // building spawning //

    // Spawns a building on the left-hand side of the road
    private void SpawnBuildingOnLeft(float offset)
    {
        GameObject buildingToSpawn;

        if (selectBuildingsRandomly)
        {
            int randomPick = Random.Range(0, leftSideBuildings.Length);
            buildingToSpawn = leftSideBuildings[randomPick];
        }
        else
        {
            buildingToSpawn = leftSideBuildings[leftBuildingIndex];

            if (!selectBuildingsRandomly)
            {
                leftBuildingIndex++;

                if (leftBuildingIndex == leftSideBuildings.Length)
                    leftBuildingIndex = 0;
            }
        }

        Vector3 spawnPosition = new Vector3(leftSpawnPoint.transform.position.x, buildingToSpawn.transform.position.y,
                                            leftSpawnPoint.position.z);

        Instantiate(buildingToSpawn, spawnPosition + Vector3.back * offset, buildingToSpawn.transform.rotation, rightSpawnPoint);
    }

    // Spawns a building on the right-hand side of the road
    private void SpawnBuildingOnRight(float offset)
    {
        GameObject buildingToSpawn;
        GameObject newBuilding;

        if (selectBuildingsRandomly)
        {
            int randomPick = Random.Range(0, rightSideBuildings.Length);
            buildingToSpawn = rightSideBuildings[randomPick];
        }
        else
        {
            buildingToSpawn = rightSideBuildings[rightBuildingIndex];

            if (!selectBuildingsRandomly)
            {
                rightBuildingIndex++;

                if (rightBuildingIndex == leftSideBuildings.Length)
                    rightBuildingIndex = 0;
            }
        }

        Vector3 spawnPosition = new Vector3(rightSpawnPoint.transform.position.x, buildingToSpawn.transform.position.y,
                                            rightSpawnPoint.position.z);

        newBuilding = Instantiate(buildingToSpawn, spawnPosition + Vector3.back * offset,
                                  buildingToSpawn.transform.rotation, rightSpawnPoint);

        // Rotate building 180 degrees to face the right-hand side of the road
        newBuilding.transform.Rotate(Vector3.up, 180);
    }


    // intersecting object spawning

    // places either a cross road or a bridge in the level
    private void SpawnIntersectingObject()
    {
        if(spawnIntersections && spawnBridges)
        {
            if(objectIndex == 0)
                Instantiate(intersection, centerSpawnPoint);
            else if(objectIndex == 1)
                Instantiate(bridgePrefab, centerSpawnPoint);

            objectIndex++;
            if (objectIndex == 2)
                objectIndex = 0;

        }
        else if(spawnIntersections)
        {
            Instantiate(intersection, centerSpawnPoint);
        }
        else if(spawnBridges)
        {
            Instantiate(bridgePrefab, centerSpawnPoint);
        }

        buildingsSinceIntersection = 0;
    }
    #endregion
}
