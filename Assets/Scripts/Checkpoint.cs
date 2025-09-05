using Unity.Cinemachine;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activado = false;

    private void OnTriggerEnter(Collider elOtro)
    {
        if (elOtro.CompareTag("Player") && !activado)
        {
            activado = true;
            FindAnyObjectByType<CheckpointManager>().UpdateRespawnPoint(transform);

            PlayerLife vida = elOtro.GetComponent<PlayerLife>();
            NightVisionManager gafas = elOtro.GetComponent<NightVisionManager>();
            
            SaveSystem.GuardarCheckpoint(transform.position, vida.GetVida(), gafas.TieneGafas());
            Debug.Log("vida: " +  vida.GetVida() + "gafas: " + gafas.TieneGafas());
        }
    }
}
