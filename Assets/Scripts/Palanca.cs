using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Palanca : MonoBehaviour, IInteractuable
{
    [SerializeField] private Puerta puertaAsociada;
    [SerializeField] private float smoothSpeed;


    private bool activada = false;
    private Quaternion targetRotation;
    private Coroutine rotateRoutine;

    private void Start()
    {
        targetRotation = transform.localRotation;  // Con locales
    }
    public void Interact(GameObject interactor)
    {
        if (!puertaAsociada.movimientoCompletado) return;  // Si está rotando no se hace nada

        activada = !activada;  // Cambio el estado

        if (activada)
        {
            targetRotation = Quaternion.Euler(0, 0, 45f);
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, 0f);
        }

        puertaAsociada.AbrirCerrar();

        rotateRoutine = StartCoroutine(SmoothRotate());
    }


    private IEnumerator SmoothRotate()
    {
        Quaternion startRot = transform.localRotation;

        float elapsed = 0f;  // Para almacenar lo transcurrido

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * smoothSpeed;
            transform.localRotation = Quaternion.Slerp(startRot, targetRotation, elapsed);
            yield return null;
        }
    }

}
