using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        // Recharge le niveau actuel
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
