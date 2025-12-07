using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipIntroBtn : MonoBehaviour
{
    public Button SkipIntroButton;

    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // Hook up the buttons to GameManager
        if (SkipIntroButton != null)
            SkipIntroButton.onClick.AddListener(SkipIntro);
    }

    void SkipIntro()
    {
        Debug.Log("intro skipped...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
