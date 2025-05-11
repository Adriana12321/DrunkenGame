using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
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

    public bool isInteracting = false;
    
    public CinemachineTargetGroup targetGroup;
    
    private void Start()
    {
        textUI.SetActive(false);
    }


    void Update()
    {
        if (isInteracting)
        {
            InteractionUpdate();
        }
        else
        {
            BrowsingUpdate();
        }
    }



    void InteractionUpdate()
    {
        PlayerController.Instance.canMove = false;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isInteracting = false;

            targetGroup.m_Targets = Array.Empty<CinemachineTargetGroup.Target>();
            
            PlayerController.Instance.canMove = true;
            PlayerController.Instance.fpsCamera.enabled = true;
            PlayerController.Instance.thirdPersonCamera.enabled = false;
        }
    }

    void BrowsingUpdate()
    {
        PlayerController.Instance.canMove = true;
        
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
        if (npcsInRange.Count>0)
        {
            textUI.SetActive(true);
            if(Input.GetKeyDown(interactKey))
            {
                isInteracting = true;
                
                CameraSetUp();
                
                for (int i = 0; i < npcColliders.Length; i++)
                {
                    Debug.Log(npcColliders[i].name);
                }
            }
        }
        else
        {
            textUI.SetActive(false);
        }
    }


    void CameraSetUp()
    {

        foreach (Transform t in npcsInRange)
        {
            targetGroup.AddMember(t,1,1);
            
        }

        targetGroup.AddMember(PlayerController.Instance.transform,1,1);
        
        PlayerController.Instance.fpsCamera.enabled = false;
        PlayerController.Instance.thirdPersonCamera.enabled = true;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    
    
    
}