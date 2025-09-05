using UnityEngine;
using UnityEngine.InputSystem;

public class TailController : MonoBehaviour
{
    [Header("Rangos y capas")]
    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask swingLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform tailOrigin;

    [Header("Swing")]
    [SerializeField] private float swingSpeed;
    [SerializeField] private float swingDuration;

    private PlayerMovement playerMovement;
    private Animator animator;
    private bool isSwinging;
    private Transform currentSwingPoint;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void OnTailAction(InputAction.CallbackContext context)
    {

    }



}
