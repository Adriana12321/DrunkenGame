using Sound;
using UnityEngine;

namespace NPC.States
{
    public class InteractState : ICharacterState
    {
        public void OnEnter(NpcBehaviour context)
        {
            SoundFxManager.instance.PlayDialogSoundFx(context.transform, 1f);
        }

        public void OnUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}