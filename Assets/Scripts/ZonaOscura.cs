using UnityEngine;

public class ZonaOscura : MonoBehaviour
{
    private void OnTriggerEnter(Collider elOtro)
    {
        if (elOtro.CompareTag("Player"))
        {
            elOtro.GetComponent<NightVisionManager>()?.EntrarZonaOscura();
        }
    }

    private void OnTriggerExit(Collider elOtro)
    {
        if (elOtro.CompareTag("Player"))
        {
            elOtro.GetComponent<NightVisionManager>()?.SalirZonaOscura();
        }
    }
}
