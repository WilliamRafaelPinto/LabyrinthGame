using UnityEngine;
using UnityEngine.UI;

public class StartBtn : MonoBehaviour
{
    public Button startGameButton;
    public Button quitButton;

    void Start()
    {
        // Hook up the buttons to GameManager
        if (startGameButton != null)
            startGameButton.onClick.AddListener(() => GameManager.Instance.StartNewGameFromMenu());



        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        //Debug.Log("Quitting game...");
        Application.Quit();

        // In editor, stop play mode too
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
