using UnityEngine;

namespace NPC.States
{
    public class DanceState : ICharacterState
    {
        private NpcBehaviour _context;
        private Animator _animator;
        private RuntimeAnimatorController originalController;

        private float transitionCooldown = 0.5f;
        private float cooldownTimer;


        private static readonly string[] danceClipNames = { "dance1", "dance2", "dance3" };
        private static readonly int IsDancing = Animator.StringToHash("isDancing");

        public void OnEnter(NpcBehaviour context)
        {
            _context = context;
            _animator = context.animator;
            
            _context.currentWaypoint.AddOccupant(context);
            
            if (_animator == null)
            {
                Debug.LogWarning($"[{_context.name}] DanceState: No Animator found!");
                return;
            }

            originalController = _animator.runtimeAnimatorController;

            // Pick a random dance
            string selectedDance = danceClipNames[Random.Range(0, danceClipNames.Length)];
            Debug.Log($"[{_context.name}] DanceState: Selected dance: {selectedDance}");

            string resourcePath = $"Animations/NPCs/Dance/{selectedDance}";
            AnimationClip clip = Resources.Load<AnimationClip>(resourcePath);

            if (clip == null)
            {
                Debug.LogWarning($"[{_context.name}] DanceState: Could not find animation at '{resourcePath}'");
                return;
            }

            AnimatorOverrideController overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            overrideController["empty_dance"] = clip;

            _animator.runtimeAnimatorController = overrideController;
            _animator.SetBool(IsDancing, true);

            Debug.Log($"[{_context.name}] DanceState: Started dancing with clip '{clip.name}'");
        }

        public void OnUpdate()
        {
            if (_animator == null) return;

            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0f) return;

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1f)
            {
                int roll = Random.Range(0, 2);
                if (roll == 1)
                {
                    Debug.Log($"[{_context.name}] DanceState: Finished dance, switching to Patrol.");
                    _context.SwitchState(CharacterStateID.Patrol);
                }
                else
                {
                    Debug.Log($"[{_context.name}] DanceState: Finished dance, doing another random dance.");
                    _context.SwitchState(CharacterStateID.Dance); // Re-enter DanceState for a new dance
                }
            }
        }

        public void OnExit()
        {
            if (_animator != null)
            {
                _context.currentWaypoint.RemoveOccupant(_context);

                _animator.SetBool(IsDancing, false);
                _animator.runtimeAnimatorController = originalController;
                Debug.Log($"[{_context.name}] DanceState: Stopped dancing, restored original controller.");
                
                _context.currentWaypoint.RemoveOccupant(_context);
                _context.GetNpcMesh().transform.localRotation = Quaternion.identity;
                _context.GetNpcMesh().transform.localPosition = new Vector3(0f,-1f,0f);
            }
        }
    }
}
