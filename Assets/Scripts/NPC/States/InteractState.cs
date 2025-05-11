using UnityEngine;

namespace NPC.States
{
    public class InteractState : ICharacterState
    {
        public void OnEnter(NpcBehaviour context)
        {
            Debug.Log("interacting!");
        }

        public void OnUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}