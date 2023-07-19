using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cityBuilder;

    [Header("UI")]
    [SerializeField] GameObject gameOverMessage;
    [SerializeField] GameObject winMessage;
    [SerializeField] GameObject pauseMessage;
    [SerializeField] GameObject menuButtons;
    [SerializeField] Button resumeButton;
    [SerializeField] Image weaponGaugeBar;
    [SerializeField] RectTransform enemyHealthBar;

    public static GameManager Instance { get; private set; }

    private float maxGaugeWidth;
    private Color defaultGaugeColor;
    private float maxHealthbarWidth;

    private bool gamePaused = false;

    [Header("FMOD Events")]
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;

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

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void TogglePause()
    {
        gamePaused = !gamePaused;

        pauseMessage.SetActive(gamePaused);
        menuButtons.SetActive(gamePaused);
        resumeButton.gameObject.SetActive(gamePaused);

        Time.timeScale = gamePaused ? 0: 1;

        if (gamePaused)
        {
            OnGamePaused?.Invoke();
        }
        else
        {
            OnGameResumed?.Invoke();
        }
    }

    public void EndGame()
    {
        gameOverMessage.SetActive(true);
        menuButtons.SetActive(true);

        OnGameLost?.Invoke();

        Time.timeScale = 0;        
    }

    public void WinGame()
    {
        winMessage.SetActive(true);

        OnGameWon?.Invoke();

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
