using UnityEngine;

public class RespawnCaida : MonoBehaviour
{
    [SerializeField] private CheckpointManager checkpointManager;

    private void OnTriggerEnter(Collider elOtro)
    {
        if (elOtro.CompareTag("Player"))
        {
            checkpointManager.Respawn();
        }
    }
}
