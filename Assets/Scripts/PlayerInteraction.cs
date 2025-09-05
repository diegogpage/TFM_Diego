using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float rangoInteraccion;
    [SerializeField] private LayerMask capaInteractuable;
    [SerializeField] private Transform puntoInteraccion;
    [SerializeField] private TextMeshProUGUI textoInteractuar;

    private Vector3 direccion;
    private IInteractuable interactuableDetectado;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //void Update()
    //{
    //    direccion = transform.right;
    //    Debug.DrawRay(puntoInteraccion.position, direccion * rangoInteraccion, Color.green);

    //    if (Input.GetKeyDown(KeyCode.E))    // Uso el sistema viejo por comodidad
    //    {
    //        Debug.Log("intento interactuar");
    //        IntentarInteractuar();
    //    }   
    //}

    //private void IntentarInteractuar()
    //{
    //    Ray ray = new Ray(puntoInteraccion.position, direccion);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, rangoInteraccion, capaInteractuable))
    //    {
    //        IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();

    //        if (interactuable != null)  // Si contiene la interfaz IInteractuable
    //        {
    //            Debug.Log("Interactuo");
    //            interactuable.Interact(gameObject);
    //        }
    //    }
    //}

    void Update()
    {
        direccion = transform.forward;
        Debug.DrawRay(puntoInteraccion.position, direccion * rangoInteraccion, Color.green);

        // Raycast continuo para detección
        RaycastHit hit;
        if (Physics.Raycast(puntoInteraccion.position, direccion, out hit, rangoInteraccion, capaInteractuable))
        {
            interactuableDetectado = hit.collider.GetComponent<IInteractuable>();

            if (interactuableDetectado != null)
            {
                textoInteractuar.gameObject.SetActive(true);
            }
            else
            {
                // El objeto no lo implementa
                interactuableDetectado = null;  // Vacío 
                textoInteractuar.gameObject.SetActive(false);
            }
        }
        else
        {
            // No colisiona con nada
            interactuableDetectado = null;  // Vacío
            textoInteractuar.gameObject.SetActive(false);
        }


        // Interactúo si pulso E y hay algo delante
        if (Input.GetKeyDown(KeyCode.E) && interactuableDetectado != null)
        {
            anim.SetTrigger("interact");
            interactuableDetectado.Interact(gameObject);
        }
    }
}
