using UnityEngine;

public enum PinColorType
{
    Green,
    Red,
    Gold
}

public class PinColor : MonoBehaviour
{
    [SerializeField] private PinColorType colorType;

    public PinColorType ColorType => colorType;

    // Au lancement, on peint la quille de la bonne couleur
    private void Start()
    {
        Renderer pinRenderer = GetComponent<Renderer>();

        if (colorType == PinColorType.Green)
            pinRenderer.material.color = Color.green;
        else if (colorType == PinColorType.Red)
            pinRenderer.material.color = Color.red;
        else
            pinRenderer.material.color = Color.yellow;
    }
}
