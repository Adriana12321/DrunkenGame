using UnityEngine;

namespace NPC.States
{
    public class InteractState : ICharacterState
    {
        private Reactions reaction;

        public void Initialize(Reactions reaction)
        {
            this.reaction = reaction;
        }

        public void OnEnter(NpcBehaviour context)
        {
            reaction?.StartReaction();
        }

        public void OnUpdate()
        {
            reaction?.OnUpdate();
        }

        public void OnExit()
        {
            reaction?.StopReaction();
        }
    }
}