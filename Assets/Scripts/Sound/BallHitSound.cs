using UnityEngine;

public class BallHitSound : MonoBehaviour
{
    [SerializeField] private AudioSource pinSound;
    [SerializeField] private AudioSource wallSound;

    private void OnCollisionEnter(Collision collision)
    {
        // La boule touche une quille
        if (collision.gameObject.GetComponent<PinColor>() != null)
            pinSound.Play();

        // La boule rebondit sur un mur
        else if (collision.gameObject.GetComponent<BallBouncer>() != null)
            wallSound.Play();
    }
}
