using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Le jeu se ferme");
        Application.Quit();
    }
}
