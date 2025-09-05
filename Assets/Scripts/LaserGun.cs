using UnityEngine;

public class LaserGun : MonoBehaviour, ICanAttack
{
    [SerializeField] private LaserBehaviour laserPrefab;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private Vector3 targetingCorrection; // Para que en lugar de disparar a los pies dispare más arriba
    [SerializeField] private float attackCooldown;
    private bool lockedOn = false;
    private float timer;
    private Transform playerLocation;

    private void Update()
    {
        if (lockedOn)
        {
            LookAtTarget();

            timer += Time.deltaTime;
            if (timer > attackCooldown)
            {
                this.GetComponent<ICanAttack>().Attack();
                timer = 0f;
            }
        }
    }

    private void LookAtTarget()
    {
        transform.LookAt(playerLocation.position);
        Debug.DrawLine(attackPoint.transform.position, playerLocation.position, Color.red);
    }

    public void Attack()
    {
        Instantiate(laserPrefab, attackPoint.transform.position, Quaternion.LookRotation(playerLocation.position + targetingCorrection - attackPoint.transform.position));
        Debug.Log("distancia: " + Vector3.Distance(transform.position, playerLocation.position));
    }

    public void StopAttacking()
    {
        lockedOn = false;
        playerLocation = null;
        timer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lockedOn = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        playerLocation = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAttacking();
        }
    }
}
