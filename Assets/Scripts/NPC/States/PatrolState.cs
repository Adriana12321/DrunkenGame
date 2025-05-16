using UnityEngine;
using UnityEngine.AI;

namespace NPC.States
{
    public class PatrolState : ICharacterState
    {
        private NpcBehaviour context;
        private NavMeshAgent agent;
        private Vector3 nextDestination;

        public void OnEnter(NpcBehaviour npcBehaviour)
        {
            context = npcBehaviour;
            agent = context.GetNavMeshAgent();
            agent.isStopped = false;
            
            FindValidWaypoint();
            
        }

        public void OnUpdate()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                context.SwitchState(context.GetRandomState());
            }
        }

        public void OnExit()
        {
            agent.isStopped = true;
        }

        private void FindValidWaypoint()
        {
            for (int i = 0; i < context.waypoints.Count; i++)
            {
                var wp = context.GetRandomWayPoint();
                if (wp.CanOccupy())
                {
                    nextDestination = wp.GetFreeIdlePosition();
                    agent.SetDestination(nextDestination);
                    return;
                }
            }

            // fallback if all full
            nextDestination = context.transform.position;
        }
    }
}