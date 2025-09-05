using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    [SerializeField] private string playerDetectorTag = "";

    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    private Transform target;

    private float timer = 0;
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);
        timer = attackCooldown;
    }
    public override void OnUpdateState()
    {
        timer += Time.deltaTime;
        if (timer > attackCooldown)
        {
            this.GetComponent<ICanAttack>().Attack();
            timer = 0f;
        }

        if (Vector3.Distance(transform.position, target.position) > attackDistance)
        {
            this.GetComponent<ICanAttack>().StopAttacking();
            controller.ChangeState(controller.ChaseState);
        }
    }
    public override void OnExitState()
    {
        Debug.Log("Exiting attack State");
        this.GetComponent<ICanAttack>().StopAttacking();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(playerDetectorTag))
        {
            target = collision.transform;
        }
    }

}
