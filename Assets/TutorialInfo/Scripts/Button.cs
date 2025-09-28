using UnityEngine;
using UnityEngine.UI;

public class WinMenuUI : MonoBehaviour
{
    public Button nextLevelButton;
    public Button backToMenuButton;
    public Button quitButton;

    void Start()
    {
        // Hook up the buttons to GameManager
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(() => GameManager.Instance.NextLevel());

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => GameManager.Instance.BackToMenu());

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // In editor, stop play mode too
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
