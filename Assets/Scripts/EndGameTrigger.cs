using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider elOtro)
    {
        if (elOtro.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("FinJuego");
        }
    }
}
