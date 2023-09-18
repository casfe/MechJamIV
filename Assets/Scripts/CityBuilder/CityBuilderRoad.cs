using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilderRoad : MonoBehaviour
{
    [SerializeField] GameObject RoadPrefab;
    [SerializeField] GameObject FirstRoad;
    [SerializeField] Transform roadsParent;
    [SerializeField] int startRoadsCounter;
    [SerializeField] float timeToSpawnRoad = 4f;
    [SerializeField] float spawnOffset;

    GameObject lastRoad;
    Vector3 spawnedPosition;

    private void Start()
    {
        lastRoad = FirstRoad;

        // spawn starting roads
        for (int i = 0; i < startRoadsCounter; i++)
        {
            SpawnNew();
        }
    }

    public void UpdateRoadBuilder()
    {
        // if the road has moved far enough it is time to spawn another one
        if(spawnedPosition.z - lastRoad.transform.position.z >= spawnOffset)
        {
            SpawnNew();
        }
    }

    public void SpawnNew()
    {
        GameObject newRoad = Instantiate(RoadPrefab, new Vector3(lastRoad.transform.position.x, lastRoad.transform.position.y, lastRoad.transform.position.z + spawnOffset), Quaternion.identity);
        newRoad.transform.parent = roadsParent;
        spawnedPosition = newRoad.transform.position;

        lastRoad = newRoad;
    }

}
