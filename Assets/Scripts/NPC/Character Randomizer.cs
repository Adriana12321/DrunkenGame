using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    public class CharacterRandomizer : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] characters = Array.Empty<GameObject>();
        
        [SerializeField]
        private GameObject[] accessories = Array.Empty<GameObject>();

        [SerializeField]
        private Transform accessoryParent;
        void Start()
        {
            foreach (var character in characters)
            {
                character.SetActive(false);
            }
            
            int random = Random.Range(0, characters.Length);
            characters[random].SetActive(true);
            
            Accessorize();
        }

        void Accessorize()
        {
            int rand = Random.Range(0, 2);
            if (rand != 2) return;
            CheckAccessories();
            AddAccessory();
        }
        
        void AddAccessory()
        {
            int random = Random.Range(0, accessories.Length);
            GameObject accessory = Instantiate(accessories[random], accessoryParent);
        }

        void CheckAccessories()
        {
            if (accessoryParent.childCount > 0)
            {
                for (int i = 0; i < accessoryParent.childCount; i++)
                {
                    GameObject accessory = accessoryParent.GetChild(i).gameObject;
                    accessory.SetActive(false);
                }
            }
        }

    }
}
