using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System;
//using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mazeGeneratorPrefab;

    public AudioClip collecFragtSound;

    public AudioClip obstacleSound;
    
    // Referência à instância do MazeGenerator que está ativa na cena
    private MazeGenerator currentMazeGenerator;


    [Header("Level / Maze settings")]
    public int startingMazeSize = 10;    // size at level 1 (square)
    public int maxMazeSize = 50;
    public int level = 1;               // current level (1-based)

    [Header("Scoring")]
    public int pointsPerCoin = 10; // Modificado para 10 pontos por moeda
    public int score = 0;
    public float timer = 30f; // Tempo inicial em segundos

    // runtime
    private int collected = 0;
    private int requiredToWin = 3;
    private float timeAccumulator = 0f;

    [Header("UI (GameScene)")]
    public TextMeshProUGUI scoreText; // assign in GameScene

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinsText; // optional: show collected/required
    public TextMeshProUGUI playerName;
    public TMP_InputField nameInput;

    [Header("Scenes (names)")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    public string winSceneName = "WinScene";
    public string endSceneName = "WinGameScene";
    public string gameOverSceneName = "GameOverScene"; // Adicionar cena de Game Over

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
        //Debug.Break();
        // nothing here; StartLevel will be called by menu or scene load
    }

    // Called when a scene loads so we can hook UI references automatically if needed
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            scoreText = GameObject.Find("scoreText")?.GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("timerText")?.GetComponent<TextMeshProUGUI>();
            coinsText = GameObject.Find("coinText")?.GetComponent<TextMeshProUGUI>();
            playerName = GameObject.Find("playerName")?.GetComponent<TextMeshProUGUI>();
            nameInput = GameObject.Find("nameInput")?.GetComponent<TMP_InputField>();
            
            // --- Lógica de Geração do Labirinto Corrigida ---
            
            // 1. Tenta encontrar uma instância existente na cena
            //currentMazeGenerator = FindObjectOfType<MazeGenerator>();
            MazeGenerator currentMazeGenerator = MazeGenerator.Instance;

            // 2. Se não encontrar, instancia a partir do prefab
            if (currentMazeGenerator == null)
            {
                //Debug.Log("Instanciando novo MazeGenerator.");
                GameObject mazeObj = Instantiate(mazeGeneratorPrefab);
                currentMazeGenerator = mazeObj.GetComponent<MazeGenerator>();
            }
            else
            {
                //Debug.Log("Reutilizando MazeGenerator existente.");
            }

            // 3. Verifica se a referência é válida antes de chamar Generate
            if (currentMazeGenerator != null)
            {
                StartLevel();
                //Debug.Log("LV: " + level);
                int size = GetCurrentMazeSize();
                
                // Chama Generate na instância (existente ou nova)
                currentMazeGenerator.Generate(size, size);
                currentMazeGenerator.PlaceCollectibles(requiredToWin);
                currentMazeGenerator.PlaceObstacles();
            }
            else
            {
                ////Debug.LogError("MazeGenerator não encontrado ou não pôde ser instanciado!");
            }
            
            // --- Fim da Lógica de Geração do Labirinto Corrigida ---

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
        }else if (scene.name == gameOverSceneName)
        {
            // Find the score text in the WinScene and update it
            TextMeshProUGUI finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<TextMeshProUGUI>();
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + GetFinalScoreForDisplay();
            }
        }else if (scene.name == endSceneName)
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

        // Encontra o MazeGenerator e coloca obstáculos
        /*MazeGenerator mazeGen = Object.FindFirstObjectByType<MazeGenerator>();
        
        if (mazeGen != null)
        {
            mazeGen.PlaceObstacles();
        }*/

        // Find UI elements in scene (if not assigned)
        UpdateUI();
    }

    public int GetCurrentMazeSize()
    {
        // Maze size grows by 1 each level (you can adjust growth formula here)
        int size = startingMazeSize + ((level - 1)*3);
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
        AudioSource.PlayClipAtPoint(collecFragtSound, Camera.main.transform.position, 1f);
        score += pointsPerCoin;
        timer += pointsPerCoin;
        UpdateUI();

        if (collected >= requiredToWin)
        {
            if (level >= 15)
            {
                Win_the_game();
            }else
            {
                Win();
            }
        }
    }

    public void SubtractTime(int amount)
    {
        timer -= amount;
        AudioSource.PlayClipAtPoint(obstacleSound, Camera.main.transform.position, 1f);
        if (timer <= 0)
        {
            GameOver();
        }
        UpdateUI();
    }

    void Update()
    {
        // only apply time penalty in GameScene (basic check)
       /* GameObject mazeObj = GameObject.FindWithTag("MazeGenerator");
        MazeGenerator generator = mazeObj.GetComponent<MazeGenerator>();*/
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == gameSceneName)
        {
            // decrement 1 point per second
            timeAccumulator += Time.deltaTime;
            if (timeAccumulator >= 1f)
            {
                int sub = Mathf.FloorToInt(timeAccumulator);
                timer -= sub;
                if (timer <= 0)
                {
                    GameOver();
                }
                timeAccumulator -= sub;
                UpdateUI();
            }
        }
        //generator.update();

    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();

        TextMeshProUGUI timerText = GameObject.Find("timerText")?.GetComponent<TextMeshProUGUI>();

        if (timerText != null)
            timerText.text = "Time: " + Mathf.Max(0, Mathf.CeilToInt(timer)).ToString();

        if (coinsText != null)
            coinsText.text = "Pieces: " + collected + " / " + requiredToWin;
            
        if (playerName != null)
        playerName.text = PlayerPrefs.GetString("PlayerName", "Default");
    }

    void Win()
    {
        // Adiciona o tempo restante à pontuação
        score += Mathf.Max(0, Mathf.CeilToInt(timer));

        /*// Salva a pontuação
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        HighScoreData highScoreData = HighScoreData.Load();
        highScoreData.AddScore(playerName, score, level);*/

        ////Debug.Log("You win! Final Score: " + score);

        // Carrega a cena de vitória
        SceneManager.LoadScene(winSceneName);
    }

void Win_the_game()
    {
        // Adiciona o tempo restante à pontuação
        score += Mathf.Max(0, Mathf.CeilToInt(timer));

        /*// Salva a pontuação
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        HighScoreData highScoreData = HighScoreData.Load();
        highScoreData.AddScore(playerName, score, level);*/

        ////Debug.Log("Congratulations! Final Score: " + score);

        // Carrega a cena de vitória
        SceneManager.LoadScene(endSceneName);
    }
    public void Win2()
    {
        // Salva a pontuação
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        HighScoreData highScoreData = HighScoreData.Load();
        highScoreData.AddScore(playerName, score, level);
    }

    // Called by Win menu button -> Next Level
    public void NextLevel()
    {
        // increase level if not already at max maze size
        if (GetCurrentMazeSize() < maxMazeSize)
        {
            level++;
            //Debug.Log("level updated to " + level);
        }
        else
        {
            //Debug.Log("Max maze size reached; level remains " + level);
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
        //StartCoroutine(FindUIAfterLoad());
    }

   /* private IEnumerator FindUIAfterLoad()
    {
        // Esperar um frame para garantir que a cena foi carregada
        yield return null;
        
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
        {
            nameInput = GameObject.Find("NameInput")?.GetComponent<TMP_InputField>();
            if (nameInput != null)
            {
                nameInput.text = PlayerPrefs.GetString("PlayerName", "");
            }
        }
    }*/

    // Called by menu Start button (if MainMenu directly tells GM to start)
    public void StartNewGameFromMenu()
    {
        nameInput = GameObject.Find("nameInput")?.GetComponent<TMP_InputField>();
        level = 1;
        if (nameInput != null)
        {
            PlayerPrefs.SetString("PlayerName", nameInput.text);
        }
        else
        {
            //Debug.LogWarning("nameInput não foi encontrado! Usando nome padrão.");
            PlayerPrefs.SetString("PlayerName", "Player");
        }
        SceneManager.LoadScene(gameSceneName);//endSceneName gameSceneName
        //Debug.Log("Starting new game from menu, level set to 1.");
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
        timer = 30f;
        collected = 0;
    }

    void GameOver()
    {
        //Debug.Log("Game Over!");
        // Salva a pontuação
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        HighScoreData highScoreData = HighScoreData.Load();
        highScoreData.AddScore(playerName, score, level);

        SceneManager.LoadScene(gameOverSceneName);
    }
}
