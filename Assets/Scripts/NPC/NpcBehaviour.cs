using System.Collections;
using System.Collections.Generic;
using NPC;
using NPC.States;
using UnityEngine;
using UnityEngine.AI;

public class NpcBehaviour : MonoBehaviour
{
    
    public List<Transform> waypoints;
    NavMeshAgent agent;
    private bool isIddling;
    Vector3 targetPosition;

    private float iddleTime;
    
    public PatrolState patrolState = new PatrolState();
    public IdleState idleState = new IdleState();
    public InteractState interactState = new InteractState();

    private ICharacterState _currentState;
    
    [SerializeField] private float idleRange;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPosition = GetRandomWayPoint();
        
        _currentState = patrolState;
        
        _currentState.OnEnter(this);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.OnUpdate();
    }
    
    public Vector3 GetRandomWayPoint()
    {
        if (waypoints.Count <= 0) return Vector3.zero;
        int point = Random.Range(0, waypoints.Count);
        return waypoints[point].position;
    }

    public void SwitchState(ICharacterState nextState)
    {
        _currentState.OnExit();
        _currentState = nextState;
        _currentState.OnEnter(this);
    }

    public ICharacterState GetRandomSate()
    {
        if (Random.Range(0, 2) == 1)
        {
            Debug.Log("random patrol state");
            return patrolState;
        } 
        Debug.Log("random idle state"); 
        return idleState;   
    }
    
    public NavMeshAgent GetNavMeshAgent() => agent;
}
