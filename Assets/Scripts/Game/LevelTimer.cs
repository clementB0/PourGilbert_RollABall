using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private GameResult gameResult;
    [SerializeField] private float timeLimit = 60f;

    // Temps qu'il reste avant de perdre
    public float TimeLeft { get; private set; }

    private void Start()
    {
        TimeLeft = timeLimit;
    }

    private void Update()
    {
        if (gameResult.IsFinished) return;

        TimeLeft -= Time.deltaTime;

        // Plus de temps : c'est perdu
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            gameResult.Lose();
        }
    }
}
