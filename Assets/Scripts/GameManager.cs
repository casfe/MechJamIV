using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cityBuilder;

    [Header("UI")]
    [SerializeField] GameObject gameOverMessage;
    [SerializeField] GameObject winMessage;
    [SerializeField] Image weaponGaugeBar;
    [SerializeField] RectTransform enemyHealthBar;

    public static GameManager Instance { get; private set; }

    private float maxGaugeWidth;
    private Color defaultGaugeColor;
    private float maxHealthbarWidth;

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
        gameOverMessage.SetActive(false);
        winMessage.SetActive(false);

        maxGaugeWidth = weaponGaugeBar.rectTransform.sizeDelta.x;
        weaponGaugeBar.rectTransform.sizeDelta = new Vector2(0, weaponGaugeBar.rectTransform.sizeDelta.y);

        defaultGaugeColor = weaponGaugeBar.color;

        maxHealthbarWidth = enemyHealthBar.sizeDelta.x;
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

    public void WinGame()
    {
        winMessage.SetActive(true);
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

    public void SetWeaponGauge(int value, int maxValue)
    {
        float unitWidth = maxGaugeWidth / maxValue;
        weaponGaugeBar.rectTransform.sizeDelta = new Vector2(unitWidth * value, weaponGaugeBar.rectTransform.sizeDelta.y);

        if (value == maxValue)
            weaponGaugeBar.color = Color.green;
        else
            weaponGaugeBar.color = defaultGaugeColor;
    }

    public void SetEnemyHealthBar(int health, int maxHealth)
    {
        float unitWidth = maxHealthbarWidth / maxHealth;
        enemyHealthBar.sizeDelta = new Vector2(unitWidth * health, enemyHealthBar.sizeDelta.y);
    }

}
