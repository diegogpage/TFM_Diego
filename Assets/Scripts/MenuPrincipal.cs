using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private Button botonCargar;
    [SerializeField] private Image fadeImage;
    private float fadeDuration = 1f;


    private void Start()
    {
        StartCoroutine(FadeIn());

        if (botonCargar != null)
        {
            // Solo puedo pulsarlo si hay algo guardado
            botonCargar.interactable = SaveSystem.HayCheckpoint();
        }

    }

    public IEnumerator FadeIn()
    {
        // Para que haya un fundido y no se vea tan brusco
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 0.4f, t / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }
    }

    public void NuevaPartida()
    {
        SaveSystem.ResetProgress();
        SceneManager.LoadScene("Juego");
    }

    public void CargarPartida()
    {
        SceneManager.LoadScene("Juego");
        SaveSystem.CargarCheckpoint();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    
    public void MenuControles()
    {
        SceneManager.LoadScene("MenuControles");
    }

}
