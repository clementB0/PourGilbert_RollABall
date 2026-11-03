using UnityEngine;

public class ShowWhenScore : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private int scoreNeeded = 150;
    [SerializeField] private GameObject objectToShow;

    private void Start()
    {
        // L'objet est caché au début du niveau
        objectToShow.SetActive(false);
    }

    private void Update()
    {
        // Il apparaît quand on a assez de points
        if (scoreManager.Score >= scoreNeeded)
            objectToShow.SetActive(true);
    }
}
