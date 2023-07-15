using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cityBuilder;

    [Header("UI")]
    [SerializeField] GameObject gameOverMessage;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EndGame()
    {
        gameOverMessage.SetActive(true);
        Time.timeScale = 0;        
    }

    public void HaltAllObjects()
    {
        cityBuilder.SetActive(false);

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach(GameObject obstacle in obstacles)
        {
            obstacle.GetComponent<MoveTowardsPlayer>().enabled = false;
        }

        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");

        foreach(GameObject road in roads)
        {
            road.GetComponent<MoveTowardsPlayer>().enabled = false;
        }

    }

}
