using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private PatrolState patrolState;
    private AttackState attackState;
    private ChaseState chaseState;

    [SerializeField] private State<EnemyController> currentState;

    public PatrolState PatrolState { get => patrolState;}
    public AttackState AttackState { get => attackState;}
    public ChaseState ChaseState { get => chaseState;}

    private void Start()
    {
        patrolState = GetComponent<PatrolState>();
        attackState = GetComponent<AttackState>();  
        chaseState = GetComponent<ChaseState>();

        ChangeState(patrolState);
    }

    private void Update()
    {
        if (currentState)
        {
            currentState.OnUpdateState();
        }
    }

    public void ChangeState(State<EnemyController> newState)
    {
        if (currentState)
        {
            currentState.OnExitState();
        }

        currentState = newState;

        currentState.OnEnterState(this);
    }
}
