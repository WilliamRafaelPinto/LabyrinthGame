using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void LoadGameScene()
    {
        // Replace "GameScene" with your actual game scene name
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        //Debug.Log("Quit pressed!");
        Application.Quit(); // Works in build, not in editor
    }
}
