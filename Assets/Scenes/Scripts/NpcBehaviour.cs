using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcBehaviour : MonoBehaviour
{
    
    public List<Transform> waypoints;
    NavMeshAgent agent;
    private bool isIddling;
    Vector3 targetPosition;

    private float iddleTime;


    [SerializeField] private float iddleRange;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPosition = GetRandomWayPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (iddleTime >= 0)
        {
            iddleTime -= Time.deltaTime;
            agent.isStopped = true;
        }
        
        if (iddleTime<=0)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition);
        }
        
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            targetPosition = GetRandomWayPoint();
            iddleTime = Random.Range(1, iddleRange);
        }
    }
    
    public Vector3 GetRandomWayPoint()
    {
        if (waypoints.Count <= 0) return Vector3.zero;
        int point = Random.Range(0, waypoints.Count);
        Debug.Log(point);
        return waypoints[point].position;
    }
}
