using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private int vidaTotal;
    [SerializeField] private CheckpointManager checkpoint;
    private int vida;

    [Header("UI")]
    [SerializeField] private Image healthFill;

    [Header("Damage Effect")]
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float flashDuration;
    [SerializeField] private Color flashColor;

    [Header("Sonido")]
    [SerializeField] private AudioSource sonidoVida;

    private Coroutine flashRoutine;

    private void Start()
    {
        vida = vidaTotal;
        damageOverlay.gameObject.SetActive(true);
        damageOverlay.color = Color.clear;  // Comienza transparente

        // Para cargar la posicion si fuera necesario
        if (SaveSystem.HayCheckpoint())
        {
            vida = SaveSystem.CargarVida();
            ActualizarUI();
        }
        else
        {
            vida = vidaTotal;
        }
    }

    public void QuitarVida()
    {
        vida--;
        Debug.Log("Vida: " + vida);

        ActualizarUI();
        FlashDamage();
        sonidoVida.Play();

        if (vida <= 0)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private void ActualizarUI()
    {
        healthFill.fillAmount = (float)vida / vidaTotal;

        float t = (float)vida / vidaTotal;

        if (t > 0.5f)
        {
            healthFill.color = Color.green;
        }
        else if (t > 0.25)
        {
            healthFill.color = Color.yellow;
        }
        else
        {
            healthFill.color = Color.red;
        }
    }

    private void FlashDamage()
    {
        // Por si la pantalla aún sigue roja que no se solapen
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        // Fade in
        float timer = 0f;
        while (timer < flashDuration / 2f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 0.5f, timer / (flashDuration / 2f));  // alpha va de 0 a 0.5 de opacidad
            damageOverlay.color = new Color(0.35f, 0.08f, 0.12f, alpha);
            yield return null;
        }

        // Fade out
        timer = 0f;
        while (timer < flashDuration / 2f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0, timer / (flashDuration / 2f));  // efecto contrario
            damageOverlay.color = new Color(0.35f, 0.08f, 0.12f, alpha);
            yield return null;
        }

        damageOverlay.color = Color.clear;
    }

    private IEnumerator RespawnRoutine()
    {
        StopRobots();

        // Fade out
        yield return StartCoroutine(FindAnyObjectByType<FadeScreen>().FadeOut());

        // Respawn
        vida = vidaTotal;
        ActualizarUI();
        checkpoint.Respawn();
        yield return new WaitForSeconds(0.5f);

        // Fade in
        yield return StartCoroutine(FindAnyObjectByType<FadeScreen>().FadeIn());
    }

    public int GetVida()
    {
        // Para luego cargar la partida
        return vida;
    }

    private void StopRobots()
    {
        // Para parar a los robots si matan al player
        var robots = FindObjectsByType<Robot_Sphere>(FindObjectsSortMode.None);
        foreach (var bot in robots)
        {
            bot.StopAttacking();
        }
    }
}
