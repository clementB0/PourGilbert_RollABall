using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
