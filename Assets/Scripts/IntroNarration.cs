using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroNarration : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textUI;
    public float typingSpeed = 0.03f;
    public float pauseTime = 2f;

    [Header("Texts")]
    [TextArea(3, 10)] public string texto1;
    [TextArea(3, 10)] public string texto2;
    [TextArea(3, 10)] public string texto3;

    private bool skipRequested = false;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    public void SkipIntro()
    {
        skipRequested = true;
    }

    IEnumerator PlayIntro()
    {
        yield return ShowText(texto1);
        if (skipRequested) yield break;

        yield return new WaitForSeconds(pauseTime);
        textUI.text = "";

        yield return ShowText(texto2);
        if (skipRequested) yield break;

        yield return new WaitForSeconds(pauseTime);
        textUI.text = "";

        yield return ShowText(texto3);
        if (skipRequested) yield break;

        yield return new WaitForSeconds(pauseTime);

        // Aqui você troca de cena (para o menu)
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator ShowText(string fullText)
    {
        textUI.text = "";

        foreach (char c in fullText)
        {
            if (skipRequested)
            {
                textUI.text = fullText; 
                yield break;
            }

            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
