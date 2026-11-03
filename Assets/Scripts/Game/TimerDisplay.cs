using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private LevelTimer levelTimer;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        timerText.text = "Temps : " + Mathf.CeilToInt(levelTimer.TimeLeft) + " s";
    }
}
