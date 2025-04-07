using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    public GameObject textUI;

    [Header("Detection Settings")]
    public float detectionRadius = 3f;
    public LayerMask npcLayer;

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    private Collider[] npcColliders;
    private Transform closestNPC;
    
    public List<Transform> npcsInRange;
    private void Start()
    {
        textUI.SetActive(false);
    }


    void Update()
    {
        DetectNearbyNPCs();
        HandleInteractionInput();
    }

    void DetectNearbyNPCs()
    {
        npcColliders = Physics.OverlapSphere(transform.position, detectionRadius, npcLayer);
        
        npcsInRange = npcColliders.Select(n => n.GetComponent<Transform>()).Where(n => n != null).ToList();
    }


    void GetClosestNPC()
    {
        float closestDistance = Mathf.Infinity;
        Transform nearest = null;

        for (int i = 0; i < npcsInRange.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, npcsInRange[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = npcsInRange[i];
            }
        }

        closestNPC = nearest;
    }

    void HandleInteractionInput()
    {
        if (npcColliders != null && Input.GetKeyDown(interactKey))
        {
            for (int i = 0; i < npcColliders.Length; i++)
            {
                Debug.Log(npcColliders[i].name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    
    
    
}