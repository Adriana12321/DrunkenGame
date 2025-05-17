using Sound;
using UnityEngine;

namespace NPC.States
{
    public class InteractState : ICharacterState
    {
        private ICharacterState _action;
        
        public void OnEnter(NpcBehaviour context)
        {
            _action?.OnEnter(context);
        }

        public void OnUpdate()
        {
            _action?.OnUpdate();
        }

        public void OnExit()
        {
            _action?.OnExit();
        }

        public void SetAction(ICharacterState action)
        {
            _action = action;
        }
    }
}