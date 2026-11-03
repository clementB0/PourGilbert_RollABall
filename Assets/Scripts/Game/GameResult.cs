using UnityEngine;

public class GameResult : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource loseSound;

    // Vrai quand la partie est gagnée ou perdue
    public bool IsFinished { get; private set; }

    private void Start()
    {
        // Au début du niveau, le jeu tourne normalement et les menus sont cachés
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void Win()
    {
        if (IsFinished) return;
        IsFinished = true;

        winPanel.SetActive(true);
        winSound.Play();
        Time.timeScale = 0f; // met le jeu en pause
    }

    public void Lose()
    {
        if (IsFinished) return;
        IsFinished = true;

        losePanel.SetActive(true);
        loseSound.Play();
        Time.timeScale = 0f; // met le jeu en pause
    }
}
