using UnityEngine;

public class BallSpeed : MonoBehaviour
{
    private Rigidbody rb;

    // Les autres scripts peuvent lire cette valeur, mais seul ce script peut la modifier
    public Vector3 VelocityBeforeImpact { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Ce code s'exécute juste avant la physique : on retient la vitesse d'avant un choc
    private void FixedUpdate()
    {
        VelocityBeforeImpact = rb.linearVelocity;
    }
}
