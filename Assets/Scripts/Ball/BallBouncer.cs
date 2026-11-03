using UnityEngine;

public class BallBouncer : MonoBehaviour
{
    // 1 = même vitesse après le rebond, plus de 1 = la boule accélère
    [SerializeField] private float speedMultiplier = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        // On ne réagit que si l'objet touché est la boule
        BallSpeed ball = collision.gameObject.GetComponent<BallSpeed>();
        if (ball == null) return;

        // Direction d'arrivée et perpendiculaire à la surface touchée
        Vector3 velocity = ball.VelocityBeforeImpact;
        Vector3 normal = collision.GetContact(0).normal;

        // Reflect calcule le rebond, la vitesse reste la même
        Vector3 bounce = Vector3.Reflect(velocity, normal);
        collision.rigidbody.linearVelocity = bounce * speedMultiplier;
    }
}
