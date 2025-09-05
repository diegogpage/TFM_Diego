using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolState : State<EnemyController>
{
    [SerializeField] private Transform route;
    private List<Vector3> waypoints = new List<Vector3>();
    [SerializeField] private string playerDetectorTag = "";

    private Vector3 currentDestination;
    private int currentDestinationIndex = 0;
    private bool isRouteInitialized = false;
    [SerializeField] private float patrolSpeed;
    public override void OnEnterState(EnemyController controller)
    {
        base.OnEnterState(controller);

        if(!isRouteInitialized) InitializeRoute();

        currentDestination = waypoints[currentDestinationIndex];
        LookAtDestination();
    }

    private void InitializeRoute()
    {
        foreach (Transform waypoint in route)
        {
            waypoints.Add(waypoint.position);
        }
        isRouteInitialized = true;
    }

    public override void OnUpdateState()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentDestination, patrolSpeed * Time.deltaTime);

        if (transform.position == currentDestination)
        {
            CalculateNewDestination();
        }
    }

    private void CalculateNewDestination()
    {
        currentDestinationIndex++;
        if(currentDestinationIndex > waypoints.Count - 1)
        {
            currentDestinationIndex = 0;
        }

        currentDestination = waypoints[currentDestinationIndex];
        LookAtDestination();
    }
    private void LookAtDestination()
    {
        if (currentDestination.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public override void OnExitState()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(playerDetectorTag))
        {
            Debug.Log("Player Detected by " + gameObject.name);
            controller.ChangeState(controller.ChaseState);
        }
    }
}
