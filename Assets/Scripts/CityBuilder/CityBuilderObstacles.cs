using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilderObstacles : MonoBehaviour
{
    public GameObject obstacle;
    [SerializeField] Transform obstacleSpawnPoint;

    [SerializeField] float timeBetweenObstacles = 3;

    float timeSinceObstacle = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceObstacle += Time.deltaTime;

        if(timeSinceObstacle >= timeBetweenObstacles)
        {
            Instantiate(obstacle, obstacleSpawnPoint);
            timeSinceObstacle = 0;
        }
    }
}
