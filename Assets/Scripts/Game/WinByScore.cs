using UnityEngine;

public class WinByScore : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameResult gameResult;
    [SerializeField] private int scoreToWin = 300;

    public int ScoreToWin => scoreToWin;

    private void Update()
    {
        // Assez de points : c'est gagné
        if (scoreManager.Score >= scoreToWin)
            gameResult.Win();
    }
}
