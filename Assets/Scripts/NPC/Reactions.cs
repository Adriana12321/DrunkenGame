using Sound;
using UnityEngine;

namespace NPC
{
    public class Reactions : MonoBehaviour, ICharacterState
    {
        [Header("Reaction Prefabs")]
        public GameObject positiveObjPrefab;
        public GameObject confusedObjPrefab;
        public GameObject angryObjPrefab;

        [Header("Settings")]
        public loudnessDetection micDetector;
        public LoudnessVisualizer loudnessVisualizer;
        public float volumeThreshold = 0.2f;
        public float overloadThreshold = 0.8f;
        public float delayBeforeSwitch = 1.5f; // Time of inactivity before switching to confused
        public float reactionCooldown = 1.0f;  // Minimum time between reaction switches

        private float lowVolumeDuration = 0f;
        private float smoothedLoudness = 0f;
        private float smoothingFactor = 0.1f;

        private float reactionTimer = 0f;
        private GameObject lastActiveReaction = null;

        private GameObject positiveObj, confusedObj, angryObj;
        private NpcBehaviour npcBehaviour;

        private bool npcIsTalking = false;
        private float npcSpeechTimer = 0f;
        private float npcSpeechDuration = 2.5f;

        public void OnEnter(NpcBehaviour context)
        {
            npcBehaviour = context;
            lowVolumeDuration = delayBeforeSwitch;
            reactionTimer = 0f;

            SoundFxManager.instance.PlayDialogSoundFx(context.transform, 1f);
            npcIsTalking = true;
            npcSpeechTimer = npcSpeechDuration;

            Vector3 offset = new Vector3(0.031f, 1.15f, -0.033f);
            Transform parent = context.transform;

            positiveObj = Instantiate(positiveObjPrefab, parent);
            confusedObj = Instantiate(confusedObjPrefab, parent);
            angryObj = Instantiate(angryObjPrefab, parent);

            foreach (var obj in new[] { positiveObj, confusedObj, angryObj })
            {
                obj.transform.localPosition = offset;
                obj.SetActive(false);
            }
        }

        public void OnExit()
        {
            foreach (var obj in new[] { positiveObj, confusedObj, angryObj })
            {
                if (obj != null) GameObject.Destroy(obj);
            }
        }

        public void OnUpdate()
        {
            if (npcSpeechTimer > 0f)
            {
                npcSpeechTimer -= Time.deltaTime;
                if (npcSpeechTimer <= 0f) npcIsTalking = false;
            }

            reactionTimer -= Time.deltaTime;

            float micLoudness = micDetector.getLoudnessFromMic();
            float envLoudness = loudnessVisualizer != null ? loudnessVisualizer.GetAverageEnvironmentLoudness() : 0f;
            smoothedLoudness = Mathf.Lerp(smoothedLoudness, micLoudness, smoothingFactor);

            GameObject newReaction = null;

            if (smoothedLoudness > overloadThreshold)
            {
                newReaction = angryObj; // too loud
            }
            else if (smoothedLoudness > volumeThreshold)
            {
                if (npcIsTalking)
                {
                    newReaction = angryObj; // talking over NPC
                }
                else if (smoothedLoudness > envLoudness)
                {
                    newReaction = positiveObj; // good volume
                }
                else
                {
                    newReaction = angryObj; // environment louder than player, NPC not talking
                }

                lowVolumeDuration = 0f;
            }
            else
            {
                lowVolumeDuration += Time.deltaTime;

                if (npcIsTalking)
                {
                    newReaction = positiveObj; // silent while NPC talks
                }
                else if (lowVolumeDuration >= delayBeforeSwitch)
                {
                    newReaction = confusedObj; // idle silence
                }
            }

            if (newReaction != lastActiveReaction && reactionTimer <= 0f)
            {
                ShowOnly(newReaction);
                lastActiveReaction = newReaction;
                reactionTimer = reactionCooldown;
            }

            FacePlayer();
        }

        private void FacePlayer()
        {
            Vector3 targetPosition = PlayerController.Instance.GetPlayerMeshPosition();
            Vector3 direction = targetPosition - npcBehaviour.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                npcBehaviour.transform.rotation = Quaternion.Slerp(npcBehaviour.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        private void ShowOnly(GameObject objToShow)
        {
            foreach (var obj in new[] { positiveObj, confusedObj, angryObj })
            {
                if (obj != null) obj.SetActive(obj == objToShow);
            }
        }
    }
}
