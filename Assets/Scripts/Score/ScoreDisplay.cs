using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TMP_Text scoreText;

    private void Update()
    {
        scoreText.text = "Score : " + scoreManager.Score;
    }
}
