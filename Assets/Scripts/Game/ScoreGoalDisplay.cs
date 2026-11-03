using TMPro;
using UnityEngine;

public class ScoreGoalDisplay : MonoBehaviour
{
    [SerializeField] private WinByScore winByScore;
    [SerializeField] private TMP_Text goalText;

    private void Start()
    {
        goalText.text = "Objectif : " + winByScore.ScoreToWin;
    }
}
