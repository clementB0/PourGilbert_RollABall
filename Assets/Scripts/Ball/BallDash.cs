using UnityEngine;

public class BallDash : MonoBehaviour
{
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private float minDashSpeed = 10f;

    private Rigidbody rb;

    // Temps restant avant le prochain dash (0 = prêt)
    public float TimeLeft { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Pendant l'attente, on fait descendre le temps et on s'arrête là
        if (TimeLeft > 0f)
        {
            TimeLeft -= Time.deltaTime;
            return;
        }

        // On ne dash que si on appuie sur la touche
        if (!Input.GetKeyDown(dashKey)) return;

        // On trouve le point de la souris sur le sol
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);
        if (!ground.Raycast(ray, out float distance)) return;

        Vector3 target = ray.GetPoint(distance);
        Vector3 direction = (target - transform.position).normalized;

        // On garde la vitesse actuelle, mais pas moins que la vitesse minimale
        float speed = rb.linearVelocity.magnitude;
        if (speed < minDashSpeed) speed = minDashSpeed;

        rb.linearVelocity = direction * speed;
        TimeLeft = cooldown;
    }
}
