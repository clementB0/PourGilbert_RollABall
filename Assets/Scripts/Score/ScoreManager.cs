using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float pointsPerSpeed = 10f;
    [SerializeField] private float greenMultiplier = 1f;
    [SerializeField] private float redMultiplier = -1f;
    [SerializeField] private float goldMultiplier = 3f;

    // Lisible par les autres scripts, modifiable seulement ici
    public int Score { get; private set; }

    // Appelée par une quille quand la boule la touche
    public void PinHit(PinColorType color, float speed)
    {
        float multiplier = greenMultiplier;

        if (color == PinColorType.Red)
            multiplier = redMultiplier;
        else if (color == PinColorType.Gold)
            multiplier = goldMultiplier;

        Score += Mathf.RoundToInt(speed * pointsPerSpeed * multiplier);
    }
}
