using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace NPC.States
{
    public class IdleState : ICharacterState
    {
        private NpcBehaviour context;
        private float idleTime;

        public void OnEnter(NpcBehaviour npcBehaviour)
        {
            context = npcBehaviour;
            
            idleTime = context.currentWaypoint.GetIdleTime();

            context.currentWaypoint.AddOccupant(context);
            Vector3 idleSpot = context.currentWaypoint.GetFreeIdlePosition();
            context.GetNavMeshAgent().SetDestination(idleSpot);
        }

        public void OnUpdate()
        {
            if (idleTime <= 0)
            {
                context.SwitchState(context.GetRandomState());
            }
            else idleTime -= Time.deltaTime;
        }

        public void OnExit()
        {
            context.currentWaypoint.RemoveOccupant(context);
        }
    }
}