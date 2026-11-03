using UnityEngine;

public class PinHitDetector : MonoBehaviour
{
    private bool alreadyHit;

    private void OnCollisionEnter(Collision collision)
    {
        // Une quille ne donne des points qu'une seule fois
        if (alreadyHit) return;

        // On ne réagit que si l'objet touché est la boule
        BallSpeed ball = collision.gameObject.GetComponent<BallSpeed>();
        if (ball == null) return;

        alreadyHit = true;

        float speed = ball.VelocityBeforeImpact.magnitude;
        PinColorType color = GetComponent<PinColor>().ColorType;

        // On prévient le gestionnaire de score
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        scoreManager.PinHit(color, speed);
    }
}
