using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mazeGeneratorPrefab;

    [Header("Level / Maze settings")]
    public int startingMazeSize = 5;    // size at level 1 (square)
    public int maxMazeSize = 50;
    public int level = 10;               // current level (1-based)

    [Header("Scoring")]
    public int pointsPerCoin = 100;
    public int score = 0;

    // runtime
    private int collected = 0;
    private int requiredToWin = 3;
    private float timeAccumulator = 0f;

    [Header("UI (GameScene)")]
    public TextMeshProUGUI scoreText; // assign in GameScene
    public TextMeshProUGUI coinsText; // optional: show collected/required

    [Header("Scenes (names)")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    public string winSceneName = "WinScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure new game state when first created
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // nothing here; StartLevel will be called by menu or scene load
    }

    // Called when a scene loads so we can hook UI references automatically if needed
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            scoreText = GameObject.Find("scoreText")?.GetComponent<TextMeshProUGUI>();
            coinsText = GameObject.Find("coinText")?.GetComponent<TextMeshProUGUI>();
            StartLevel();
            Debug.Log("LV: " + level);
            int size = GetCurrentMazeSize();
            GameObject mazeObj = Instantiate(mazeGeneratorPrefab);
            MazeGenerator generator = mazeObj.GetComponent<MazeGenerator>();
            generator.Generate(size, size);
            generator.PlaceCollectibles(requiredToWin);

            collected = 0;
            // score = 0;

            UpdateUI();
        }
        else if (scene.name == mainMenuSceneName)
        {
            // optionally reset everything if returning to menu
        }
        else if (scene.name == winSceneName)
        {
            // Find the score text in the WinScene and update it
            TextMeshProUGUI finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<TextMeshProUGUI>();
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + GetFinalScoreForDisplay();
            }
        }
    }

    public void StartLevel()
    {
        // Reset runtime counters
        collected = 0;
        // score = 0;
        timeAccumulator = 0f;

        // Make sure maze size is within bounds
        int size = GetCurrentMazeSize();

        // compute coin count based on size
        requiredToWin = CalculateCoinCountForSize(size);

        // Find UI elements in scene (if not assigned)

        UpdateUI();
    }

    public int GetCurrentMazeSize()
    {
        // Maze size grows by 1 each level (you can adjust growth formula here)
        int size = startingMazeSize + (level - 1);
        if (size > maxMazeSize) size = maxMazeSize;
        return size;
    }

    // coin count formula: increases with maze area; minimum 3 coins
    int CalculateCoinCountForSize(int size)
    {
        // coins = clamp(area / 50, 3, area-1) — adjusts nicely for small & large mazes
        int area = size * size;
        int coins = Mathf.CeilToInt(area / 50f);
        if (coins < 3) coins = 3;
        if (coins > area - 1) coins = area - 1;
        return coins;
    }

    // public for MazeGenerator to query
    public int GetCoinCount()
    {
        return CalculateCoinCountForSize(GetCurrentMazeSize());
    }

    public void CollectCoin()
    {
        collected++;
        score += pointsPerCoin;
        UpdateUI();

        if (collected >= requiredToWin)
        {
            Win();
        }
    }

    void Update()
    {
        // only apply time penalty in GameScene (basic check)

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == gameSceneName)
        {
            // decrement 1 point per second
            timeAccumulator += Time.deltaTime;
            if (timeAccumulator >= 1f)
            {
                int sub = Mathf.FloorToInt(timeAccumulator);
                score -= sub;
                timeAccumulator -= sub;
                UpdateUI();
            }
        }

    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.Max(0, score).ToString();

        if (coinsText != null)
            coinsText.text = "Coins: " + collected + " / " + requiredToWin;
    }

    void Win()
    {
        // Optionally clamp score to >=0 on final display; we leave raw score but UI shows >=0.
        Debug.Log("You win! Score: " + score);

        // Load win scene
        SceneManager.LoadScene(winSceneName);
    }

    // Called by Win menu button -> Next Level
    public void NextLevel()
    {
        // increase level if not already at max maze size
        if (GetCurrentMazeSize() < maxMazeSize)
        {
            level++;
            Debug.Log("level updated to " + level);
        }
        else
        {
            Debug.Log("Max maze size reached; level remains " + level);
        }
        // load game scene (OnSceneLoaded will call StartLevel)
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by Win menu button -> Back to Menu
    public void BackToMenu()
    {
        // If you want to reset level to 1 when returning to menu, do it here:
        // level = 1;
        ResetAll();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Called by menu Start button (if MainMenu directly tells GM to start)
    public void StartNewGameFromMenu()
    {
        level = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    // helper used by Win screen to show final score
    public int GetFinalScoreForDisplay()
    {
        return Mathf.Max(0, score);
    }

    // optional: allow external reset
    public void ResetAll()
    {
        level = 1;
        score = 0;
        collected = 0;
    }
}
