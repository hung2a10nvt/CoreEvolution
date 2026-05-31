using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [Header("GameOver Elements")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD Elements")] 
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Menu Panels")] 
    [SerializeField] private GameObject pausePanel;

    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float elapsedTime;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossUIContainer; 
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossNameText;

    public static bool IsPaused { get; private set; }
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Event for EXP and Level
        if (playerStats != null)
        {
            playerStats.OnDeath += ShowGameOverUI;

            playerStats.OnEXPChanged += UpdateExpBar;
            playerStats.OnLevelChanged += UpdateLevelText;

            UpdateExpBar(playerStats.CurrentEXP, playerStats.EXPToNextLevel);
            UpdateLevelText(playerStats.currentLevel);
        }

        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }

        if (!IsPaused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    // BOSS UI
    public void ShowBossBar(string name, float maxHealth)
    {
        bossUIContainer.SetActive(true);
        bossNameText.text = name;
        bossHealthSlider.maxValue = maxHealth;
        bossHealthSlider.value = maxHealth;
    }

    public void UpdateBossHealth(float currentHealth)
    {
        bossHealthSlider.value = currentHealth;
    }

    public void HideBossBar()
    {
        bossUIContainer.SetActive(false);
    }

    // TIMER 
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public float GetElapsedTime() => elapsedTime;

    // LOGIC PAUSE MENU

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; 
        IsPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateExpBar(int currentExp, int targetExp)
    {
        expSlider.maxValue = targetExp;
        expSlider.value = currentExp;
    }

    private void UpdateLevelText(int newLevel)
    {
        levelText.text = "LV " + newLevel.ToString();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnEXPChanged -= UpdateExpBar;
            playerStats.OnLevelChanged -= UpdateLevelText;
        }
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}