using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    //[SerializeField] private string playerDetectorTag = "";

    //private Transform target;
    //[SerializeField] private float chaseSpeed;
    //[SerializeField] private float stoppingDistance;
    //private float chasingDistance;

    //[SerializeField] private float groundDetectionDistance = 0.3f;
    //[SerializeField] private LayerMask whatIsJumpable;
    //public bool isChasing = false;
    //public override void OnEnterState(EnemyController controller)
    //{
    //    base.OnEnterState(controller);
    //    Debug.Log(gameObject.name + " entered chasing state");
    //}

    //public override void OnUpdateState()
    //{
    //    if (target != null)
    //    {
    //        Move();
    //        LookAtTarget();

    //        if (Vector3.Distance(transform.position, target.position) <= stoppingDistance)
    //        {
    //            controller.ChangeState(controller.AttackState);
    //        }
    //        if (chasingDistance != 0)
    //        {
    //            if (Vector3.Distance(transform.position, target.position) > chasingDistance)
    //            {
    //                Debug.Log(Vector3.Distance(transform.position, target.position).ToString() + " > " + chasingDistance.ToString());
    //                controller.ChangeState(controller.PatrolState);
    //            }
    //        }

    //    }
    //}

    //private void Move()
    //{
    //    isChasing = Physics.Raycast(transform.position, Vector3.down, groundDetectionDistance, whatIsJumpable);
    //    if (Physics.Raycast(transform.position, Vector3.down, groundDetectionDistance, whatIsJumpable))
    //    {
    //        Debug.Log("Chase");
    //        transform.Translate(Vector3.right * chaseSpeed * Time.deltaTime);
    //    }


    //}

    //private void LookAtTarget()
    //{
    //    if (target.transform.position.x > transform.position.x)
    //    {
    //        transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        transform.rotation = Quaternion.Euler(0, 180, 0);
    //    }
    //}
    //private void OnTriggerEnter(Collider collision)
    //{
    //    if (collision.gameObject.CompareTag(playerDetectorTag))
    //    {
    //        target = collision.transform;
    //        chasingDistance = Vector3.Distance(transform.position, target.position) + 1;

    //    }
    //}
    //public override void OnExitState()
    //{

    //}

    //private void OnDrawGizmos()
    //{
    //    //Physics2D.Raycast(transform.position, Vector3.down, groundDetectionDistance, whatIsJumpable);
    //    Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectionDistance);
    //}

    [SerializeField] private string playerDetectorTag = "Player"; // el tag del player
    private Transform target;

    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    private float chasingDistance;

    [SerializeField] private float groundDetectionDistance = 0.3f;
    [SerializeField] private LayerMask whatIsGround;

    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);
        Debug.Log(gameObject.name + " entered chasing state");

        // Buscar directamente al player
        GameObject player = GameObject.FindGameObjectWithTag(playerDetectorTag);
        if (player != null)
        {
            target = player.transform;
            chasingDistance = Vector3.Distance(transform.position, target.position) + 1;
        }
    }

    public override void OnUpdateState()
    {
        if (target == null) return;

        // movimiento hacia el player
        if (Physics.Raycast(transform.position, Vector3.down, groundDetectionDistance, whatIsGround))
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0, 0) * chaseSpeed * Time.deltaTime;
        }

        // mirar hacia el player
        LookAtTarget();

        // transición a Attack
        if (Vector3.Distance(transform.position, target.position) <= stoppingDistance)
        {
            controller.ChangeState(controller.AttackState);
        }

        // transición de vuelta a Patrol
        if (Vector3.Distance(transform.position, target.position) > chasingDistance)
        {
            controller.ChangeState(controller.PatrolState);
        }
    }

    private void LookAtTarget()
    {
        if (target.position.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public override void OnExitState() { }

}
