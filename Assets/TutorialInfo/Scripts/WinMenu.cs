using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinMenu : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public Button backToMenuButton;
    public Button nextLevelButton;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            finalScoreText.text = "Final Score: " + GameManager.Instance.GetFinalScoreForDisplay();
        }
        else
        {
            finalScoreText.text = "Final Score: 0";
        }

        // Hook up buttons (optional if using Inspector)
        backToMenuButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.BackToMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });

        nextLevelButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.NextLevel();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        });
    }
}
