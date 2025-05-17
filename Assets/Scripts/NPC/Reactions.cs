using Sound;
using UnityEngine;

namespace NPC
{
    public class Reactions : MonoBehaviour, ICharacterState 
    {
        [Header("Interact Settings")]
        public GameObject positiveObjPrefab;
        public GameObject negativeObjPrefab;
        public loudnessDetection detector;
        public float volumeThreshold = 0.2f;

        private float lowVolumeDuration = 0f;
        private float delayBeforeSwitch = 1.5f;

        private float smoothedLoudness = 0f;
        private float smoothingFactor = 0.1f;

        private GameObject instantiatedPositiveObj;
        private GameObject instantiatedNegativeObj;

        private NpcBehaviour npcBehaviour;

        public void OnEnter(NpcBehaviour context)
        {
            lowVolumeDuration = delayBeforeSwitch;
            npcBehaviour = context;

            SoundFxManager.instance.PlayDialogSoundFx(context.transform, 1f);

            Vector3 localOffset = new Vector3(0.031f, 1.15f, -0.033f);

            // Instantiate positive reaction object only if not already instantiated
            if (positiveObjPrefab != null && instantiatedPositiveObj == null)
            {
                instantiatedPositiveObj = Instantiate(positiveObjPrefab, context.transform);
                instantiatedPositiveObj.transform.localPosition = localOffset;
                instantiatedPositiveObj.SetActive(false);
            }

            // Instantiate negative reaction object only if not already instantiated
            if (negativeObjPrefab != null && instantiatedNegativeObj == null)
            {
                instantiatedNegativeObj = Instantiate(negativeObjPrefab, context.transform);
                instantiatedNegativeObj.transform.localPosition = localOffset;
                instantiatedNegativeObj.SetActive(false);
            }
        }

        public void OnExit()
        {
            if (instantiatedPositiveObj != null)
            {
                Destroy(instantiatedPositiveObj);
                instantiatedPositiveObj = null;
            }

            if (instantiatedNegativeObj != null)
            {
                Destroy(instantiatedNegativeObj);
                instantiatedNegativeObj = null;
            }
        }

        public void OnUpdate()
        {
            float currentLoudness = detector.getLoudnessFromMic();
            smoothedLoudness = Mathf.Lerp(smoothedLoudness, currentLoudness, smoothingFactor);

            if (smoothedLoudness > volumeThreshold)
            {
                lowVolumeDuration = 0f;

                if (instantiatedPositiveObj != null) instantiatedPositiveObj.SetActive(true);
                if (instantiatedNegativeObj != null) instantiatedNegativeObj.SetActive(false);
            }
            else
            {
                lowVolumeDuration += Time.deltaTime;

                if (lowVolumeDuration >= delayBeforeSwitch)
                {
                    if (instantiatedPositiveObj != null) instantiatedPositiveObj.SetActive(false);
                    if (instantiatedNegativeObj != null) instantiatedNegativeObj.SetActive(true);
                }
            }

            // NPC faces the player
            Vector3 targetPosition = PlayerController.Instance.GetPlayerMeshPosition();
            Vector3 direction = targetPosition - npcBehaviour.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                npcBehaviour.transform.rotation = Quaternion.Slerp(npcBehaviour.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}
