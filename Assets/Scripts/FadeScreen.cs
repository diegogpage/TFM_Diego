using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private float fadeDuration = 1f;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);  
    }

    public IEnumerator FadeOut()
    {
        Debug.Log("FadeOut");
        // Se pone la pantalla en negro
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("FadeIn");
        // Vuelve la imagen
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
