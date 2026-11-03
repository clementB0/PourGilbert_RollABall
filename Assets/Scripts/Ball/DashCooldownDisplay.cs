using TMPro;
using UnityEngine;

public class DashCooldownDisplay : MonoBehaviour
{
    [SerializeField] private BallDash ballDash;
    [SerializeField] private TMP_Text cooldownText;

    private void Update()
    {
        if (ballDash.TimeLeft > 0f)
            cooldownText.text = "Dash : " + ballDash.TimeLeft.ToString("F1") + " s";
        else
            cooldownText.text = "Dash : prêt !";
    }
}
