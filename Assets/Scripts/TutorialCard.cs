using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCard : MonoBehaviour
{
    [SerializeField] private Image imagen;
    [SerializeField] private TMP_Text mensaje;
    [SerializeField] private GameObject panel;

    private bool isOpen = false;

    private void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (isOpen && Input.GetKeyUp(KeyCode.X))
        {
            Cerrar();
        }
    }

    public void Mostrar(Sprite nuevaImagen, string nuevoMensaje)
    {
        imagen.sprite = nuevaImagen;
        mensaje.text = nuevoMensaje;

        panel.SetActive(true);
        isOpen = true;
        Time.timeScale = 0f;
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
    }
}
