namespace NPC
{
    public interface ICharacterState
    {
        public void OnEnter(NpcBehaviour context);
        public void OnUpdate();
        public void OnExit();
    }
}