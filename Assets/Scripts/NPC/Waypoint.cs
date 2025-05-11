using System.Collections.Generic;

namespace NPC
{
    public class Waypoint
    {
        List<NpcBehaviour> occupantsList = new List<NpcBehaviour>();
        
        public int maxOccupants;
        public float radius;
        
        public void AddOccupant(NpcBehaviour character)
        {
            occupantsList.Add(character);
        }

        public void RemoveOccupant(NpcBehaviour character)
        {
            occupantsList.Remove(character);
        }


        //Get Random Area Position (chat gpt)
        
        public bool CheckOccupation()
        {
            if (occupantsList.Count > maxOccupants)
            {
                return false;
            }
            return true;
        }
    }
}