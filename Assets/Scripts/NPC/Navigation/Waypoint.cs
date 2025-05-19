using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

namespace NPC
{ 
    public class Waypoint : MonoBehaviour
    {
        [Header("Occupancy Settings")]
        [SerializeField] private int maxOccupants = 3;
        [SerializeField] private float idleRadius = 2.0f;
        [SerializeField] private float minDistanceBetweenNPCs = 1.0f;

        [Header("Activity Settings")]
        [SerializeField] private WaypointActivityType activityType = WaypointActivityType.None;

        private List<NpcBehaviour> occupantsList = new List<NpcBehaviour>();

        public void AddOccupant(NpcBehaviour npc)
        {
            if (!occupantsList.Contains(npc))
                occupantsList.Add(npc);
        }

        public void RemoveOccupant(NpcBehaviour npc)
        {
            occupantsList.Remove(npc);
        }

        public bool CanOccupy() => occupantsList.Count < maxOccupants;

        public Vector3 GetFreeIdlePosition()
        {
            const int maxTries = 10;

            for (int i = 0; i < maxTries; i++)
            {
                Vector2 offset = Random.insideUnitCircle * idleRadius;
                Vector3 candidate = transform.position + new Vector3(offset.x, 0, offset.y);

                if (IsPositionFree(candidate) && NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                    return hit.position;
            }

            return transform.position; // fallback
        }

        private bool IsPositionFree(Vector3 pos)
        {
            foreach (var npc in occupantsList)
            {
                if (Vector3.Distance(npc.transform.position, pos) < minDistanceBetweenNPCs)
                    return false;
            }
            return true;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, idleRadius);
        }

        public float GetIdleTime()
        {
            return activityType == WaypointActivityType.None ? Random.Range(8f, 25f) : 0f;
        }
        public CharacterStateID GetActivityState()
        {
            switch (activityType)
            {
                case WaypointActivityType.Dance:
                    return CharacterStateID.Dance;
                case WaypointActivityType.Talk:
                    return CharacterStateID.Idle;
                case WaypointActivityType.None:
                default:
                    return CharacterStateID.Idle;
            }
        }

        public WaypointActivityType GetActivityType() => activityType;
    }
    
    public enum WaypointActivityType
    {
        None,
        Dance,
        Talk
    }

    
}
