using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace NPC.States
{
    public class IdleState: ICharacterState
    {
        private NpcBehaviour context;
        private float idleTime;
        
        public void OnEnter(NpcBehaviour npcBehaviour)
        {
            context = npcBehaviour;
            idleTime = Random.Range(3f, 10f);
        }

        public void OnUpdate()
        {
            if (idleTime >= 0)
            {
                idleTime -= Time.deltaTime;
            }

            if (idleTime <= 0)
            {
                context.SwitchState(context.GetRandomSate());
            }
        }

        public void OnExit()
        {
            
        }
    }
}