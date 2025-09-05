using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private Canvas panelPausa;
    private bool juegoPausado;

    [SerializeField] private AudioSource sonidoMenu;

    void Start()
    {
        juegoPausado = false;
        panelPausa.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Menu");
            if (!juegoPausado)
            {
                Pausar();
            }
            else
            {
                Reanudar();
            }
        }
    }

    private void Pausar()
    {
        panelPausa.gameObject.SetActive(true);
        juegoPausado = true;
        Time.timeScale = 0f;  // Se para el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        sonidoMenu.Play();
    }

    public void Reanudar()
    {
        panelPausa.gameObject.SetActive(false);
        juegoPausado = false;
        Time.timeScale = 1f; // Se reanuda el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sonidoMenu.Play();
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
    
    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego");
    }
}
