using UnityEngine;

namespace NPC.States
{
    public class DanceState : ICharacterState
    {
        private NpcBehaviour _context;
        private Animator _animator;
        private RuntimeAnimatorController _originalController;

        private float danceDuration;
        private float elapsedTime;

        private static readonly string[] DanceClipNames = { "dance1", "dance2", "dance3" };
        private static readonly int IsDancing = Animator.StringToHash("act");

        public void OnEnter(NpcBehaviour context)
        {
            _context = context;
            _animator = context.animator;

            if (_animator == null)
            {
                Debug.LogWarning($"[{_context.name}] DanceState: No Animator found!");
                return;
            }

            _context.currentWaypoint.AddOccupant(context);

            string selectedDance = DanceClipNames[Random.Range(0, DanceClipNames.Length)];
            string resourcePath = $"Animations/NPCs/Dance/{selectedDance}";
            if (!_context.ApplyOverrideAnimation(resourcePath))
                return;

            _animator.SetBool(IsDancing, true);
            Debug.Log($"[{_context.name}] DanceState: Started dancing with clip '{selectedDance}'");

            danceDuration = Random.Range(10f, 20f);
            elapsedTime = 0f;
        }


        public void OnUpdate()
        {
            if (_animator == null) return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= danceDuration)
            {
                float chance = Random.value;
                var nextState = (chance < 0.7f) ? CharacterStateID.Patrol : CharacterStateID.Dance;
                Debug.Log($"[{_context.name}] DanceState: Switching to {nextState}");
                _context.SwitchState(nextState);
            }
        }

        public void OnExit()
        {
            if (_animator != null)
            {
                _animator.SetBool(IsDancing, false);
                _animator.runtimeAnimatorController = _originalController;
            }

            _context.currentWaypoint.RemoveOccupant(_context);
            var mesh = _context.GetNpcMesh();
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localPosition = new Vector3(0f, -1f, 0f);

            Debug.Log($"[{_context.name}] DanceState: Exiting.");
        }
    }
}
