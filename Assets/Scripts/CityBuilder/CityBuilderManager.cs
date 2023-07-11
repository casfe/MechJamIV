using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilderManager : MonoBehaviour
{
    [Header("- - - - [Managers] - - - -")]
    [SerializeField] CityBuilderDeco DecorationManager;
    [SerializeField] CityBuilderRoad RoadManager;
    [SerializeField] CityBuilderBuildings BuildingsManager;

    float counter;

    [Header("- - - - [Overall Config] - - - -")]
    [SerializeField] float TimeToSpawnRoad = 4f;



    private void Update()
    {
        RoadManager.UpdateRoadBuilder();
    }


}
