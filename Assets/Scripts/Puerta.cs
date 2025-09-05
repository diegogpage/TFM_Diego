using Unity.Cinemachine;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private Transform posicionAbierta;
    [SerializeField] private Transform posicionCerrada;
    [SerializeField] private CinemachineCamera camPuerta;

    private Transform posicionPuerta;
    public bool movimientoCompletado = true;
    private bool puertaAbierta = false;
    private bool camActiva = false;
    private AudioSource sonidoPuerta;

    void Start()
    {
        posicionPuerta = posicionCerrada;
        posicionAbierta.SetParent(null);
        posicionCerrada.SetParent(null);
        sonidoPuerta = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (transform.localPosition != posicionPuerta.position)
        {
            if (!camActiva)
            {
                camPuerta.Priority = 20;
                camActiva = true;
            }

            if (!sonidoPuerta.isPlaying)
            {
                sonidoPuerta.Play();
            }

            movimientoCompletado = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, posicionPuerta.position, velocidad * Time.deltaTime);

        }
        else
        {
            if (camActiva)
            {
                camPuerta.Priority = 5;
                camActiva = false;
            }

            if (sonidoPuerta.isPlaying)
            {
                sonidoPuerta.Stop();
            }

            movimientoCompletado = true;
        }
    }


    public void AbrirCerrar()
    {
        puertaAbierta = !puertaAbierta;

        if (puertaAbierta)
        {
            posicionPuerta = posicionAbierta;
        }
        else
        {
            posicionPuerta = posicionCerrada;
        }
    }

}
