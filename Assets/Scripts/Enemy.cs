using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float vidaTotal;
    private float vida;

    [SerializeField] private Canvas healthBar;
    [SerializeField] private Image healthFill;
    

    private void Start()
    {
        vida = vidaTotal;
        healthBar.enabled = false;
    }

    public void QuitarVida(float danho)
    {
        vida -= danho;
        MostrarVidaEnemy();

        if (vida <= 0)
        {
            Destroy(this.gameObject);          
        }
    }

    private void MostrarVidaEnemy()
    {
        healthBar.enabled = true;
        healthFill.fillAmount = vida / vidaTotal;

        if (healthFill.fillAmount <= 0.5f)
        {
            healthFill.color = Color.yellow;
        }
    }

}
