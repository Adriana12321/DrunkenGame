using UnityEngine;
using UnityEngine.AI;

namespace NPC.States
{
    public class PatrolState : ICharacterState
    {
        NpcBehaviour context;
        Vector3 nextWaypoint;
        NavMeshAgent agent;
        public void OnEnter(NpcBehaviour npcBehaviour)
        {
            context = npcBehaviour;
            
            agent = context.GetNavMeshAgent();
            nextWaypoint = context.GetRandomWayPoint();
            agent.isStopped = false;
        }

        public void OnUpdate()
        {
            agent.SetDestination(nextWaypoint);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                context.SwitchState(context.GetRandomSate());
            }
        }

        public void OnExit()
        {
            agent.isStopped = true;
        }
    }
}