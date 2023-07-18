using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CityBuilderRoad : MonoBehaviour
{
    [Header("- - - - [Overall Config] - - - -")]
    [SerializeField] GameObject RoadPrefab;
    [SerializeField] GameObject FirstRoad;
    [SerializeField] int startRoadsCounter;
    [SerializeField] float timeToSpawnRoad = 4f;
    //TODO: Offset Inside road to be able to make different roads
    [SerializeField] float spawnOffset;
    GameObject lastRoad;
    float counter;

    private void Start()
    {
        lastRoad = FirstRoad;
        //We're gonna spawn 4 new roads
        for (int i = 0; i < startRoadsCounter; i++)
        {
            SpawnNew();
        }
    }

    public void UpdateRoadBuilder()
    {
        counter += Time.deltaTime;
        //If its time spawn a road
        if (counter >= timeToSpawnRoad)
        {
            SpawnNew();
            counter = 0;
        }
    }

    public void SpawnNew()
    {
        GameObject newRoad = Instantiate(RoadPrefab, new Vector3(lastRoad.transform.position.x, lastRoad.transform.position.y, lastRoad.transform.position.z + spawnOffset), Quaternion.identity);
        lastRoad = newRoad;
    }

}
