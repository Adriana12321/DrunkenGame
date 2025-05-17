using System;
using System.Collections.Generic;
using Cinemachine;
using NPC;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject textUI;

    [Header("Detection Settings")]
    public float detectionRadius = 3f;
    public LayerMask npcLayer;

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    public List<NpcBehaviour> npcsInRange = new();
    public bool isInteracting = false;

    [Header("Camera Settings")]
    public CinemachineTargetGroup targetGroup;

    [Header("Reaction (Interact) Component")]
    [SerializeField] private Reactions reaction;

    void Start()
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
            EndInteractionMode();
        }
    }

    void BrowsingUpdate()
    {
        PlayerController.Instance.canMove = true;

        DetectNearbyNPCs();
        textUI.SetActive(npcsInRange.Count > 0);
        HandleInteractionInput();
    }

    void DetectNearbyNPCs()
    {
        npcsInRange.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, npcLayer);

        foreach (var hit in hits)
        {
            NpcBehaviour npc = hit.GetComponent<NpcBehaviour>();
            if (npc != null && !npcsInRange.Contains(npc))
            {
                npcsInRange.Add(npc);
            }
        }
    }

    void HandleInteractionInput()
    {
        if (npcsInRange.Count == 0) return;

        if (Input.GetKeyDown(interactKey))
        {
            textUI.SetActive(false);
            isInteracting = true;

            foreach (var npc in npcsInRange)
            {
                npc.SetInteractionAction(reaction);
                npc.SwitchState(CharacterStateID.Interact);

                // ✅ Reputation bar trigger
                NpcReputationUI.Show(npc.GetScore(), npc.GetMaxScore());
            }

            SetupCameras();
        }
    }

    void SetupCameras()
    {
        targetGroup.m_Targets = Array.Empty<CinemachineTargetGroup.Target>();

        foreach (var npc in npcsInRange)
        {
            targetGroup.AddMember(npc.transform, 1, 1);
        }

        targetGroup.AddMember(PlayerController.Instance.transform, 1, 1);

        PlayerController.Instance.fpsCamera.enabled = false;
        PlayerController.Instance.thirdPersonCamera.enabled = true;
    }

    void EndInteractionMode()
    {
        isInteracting = false;
        targetGroup.m_Targets = Array.Empty<CinemachineTargetGroup.Target>();

        PlayerController.Instance.canMove = true;
        PlayerController.Instance.fpsCamera.enabled = true;
        PlayerController.Instance.thirdPersonCamera.enabled = false;

        foreach (var npc in npcsInRange)
        {
            npc.SwitchState(CharacterStateID.Idle);
        }

        // ✅ Hide the bar
        NpcReputationUI.Hide();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
