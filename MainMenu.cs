using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNarration()
    {
        Debug.Log("Narration Loaded");

        if (!string.IsNullOrEmpty("StartNarration"))
        {
            SceneManager.LoadScene("StartNarration");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
