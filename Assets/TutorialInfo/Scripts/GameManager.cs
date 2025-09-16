using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int collected = 0;
    public int requiredToWin = 3;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Collect()
    {
        collected++;
        Debug.Log("Collected: " + collected);

        if (collected >= requiredToWin)
        {
            Win();
        }
    }

    void Win()
    {
        Debug.Log("You win!");
        // You can add UI or load next scene
        // Example: SceneManager.LoadScene("WinScene");
    }
}