using System.Collections.Generic;
using NPC.States;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public enum CharacterStateID
    {
        Patrol,
        Idle,
        Interact
    }

    public class NpcBehaviour : MonoBehaviour
    {
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int MoveX = Animator.StringToHash("MoveX");

        [Header("Waypoints & Idle Settings")]
        public List<Waypoint> waypoints;
        public Waypoint currentWaypoint;

        [Header("State Machine")]
        private ICharacterState _currentState;
        private Dictionary<CharacterStateID, ICharacterState> stateMap;

        [SerializeField]
        public CharacterStateID currentState;

        private NavMeshAgent agent;
        public Animator animator;

        private Vector3 previousPosition;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            previousPosition = transform.position;
            agent.updateRotation = false;

            stateMap = new Dictionary<CharacterStateID, ICharacterState>
            {
                { CharacterStateID.Patrol, new PatrolState() },
                { CharacterStateID.Idle, new IdleState() },
                { CharacterStateID.Interact, new InteractState() }
            };

            SwitchState(CharacterStateID.Patrol);
        }

        void Update()
        {
            _currentState?.OnUpdate();
            currentState = GetCurrentStateID();
            UpdateAnimator();
        }

        public void SwitchState(CharacterStateID newState)
        {
            _currentState?.OnExit();
            _currentState = stateMap[newState];
            _currentState.OnEnter(this);
        }

        private CharacterStateID GetCurrentStateID()
        {
            foreach (var kvp in stateMap)
            {
                if (kvp.Value == _currentState)
                    return kvp.Key;
            }
            return default;
        }

        public CharacterStateID GetRandomState()
        {
            return (Random.Range(0, 2) == 0) ? CharacterStateID.Patrol : CharacterStateID.Idle;
        }

        public Waypoint GetRandomWayPoint()
        {
            if (waypoints.Count == 0) return null;
            currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];
            return currentWaypoint;
        }

        private void UpdateAnimator()
        {
            if (agent.velocity.sqrMagnitude > 0.01f)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
                animator.SetFloat(MoveX, localVelocity.x);
                animator.SetFloat(MoveY, localVelocity.z);
            }
            else
            {
                animator.SetFloat(MoveX, 0f);
                animator.SetFloat(MoveY, 0f);
            }

            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(agent.velocity.normalized),
                    Time.deltaTime * 10f
                );
            }
        }

        public NavMeshAgent GetNavMeshAgent() => agent;

        public void SetInteractionAction(ICharacterState action)
        {
            if (stateMap[CharacterStateID.Interact] is InteractState interactState)
            {
                interactState.SetAction(action);
            }
        }
    }
}
