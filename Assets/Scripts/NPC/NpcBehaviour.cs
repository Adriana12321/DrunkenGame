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
        [Header("Waypoints & Idle Settings")]
        public List<Waypoint> waypoints;
        public Waypoint currentWaypoint;

        [Header("State Machine")]
        private ICharacterState _currentState;
        private Dictionary<CharacterStateID, ICharacterState> stateMap;

        [SerializeField]
        public CharacterStateID currentState;

        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

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
            currentState = GetCurrentStateID(); // For debugging/inspector display
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

        public NavMeshAgent GetNavMeshAgent() => agent;
    }
}