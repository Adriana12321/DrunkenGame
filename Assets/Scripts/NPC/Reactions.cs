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
        public float volumeThreshold = 0.005f;
        public float overloadThreshold = 0.35f;
        public float delayBeforeConfused = 1.5f;

        public float positiveCooldown = 0.4f;
        public float angryCooldown = 1.0f;

        private float lowVolumeDuration = 0f;
        private float smoothedLoudness = 0f;
        private float smoothingFactor = 0.25f;

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
            lowVolumeDuration = 0f;
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

            Debug.Log($"[{npcBehaviour.name}] Interaction started. Listening for reactions...");
        }

        public void OnExit()
        {
            foreach (var obj in new[] { positiveObj, confusedObj, angryObj })
            {
                if (obj != null) GameObject.Destroy(obj);
            }

            Debug.Log($"[{npcBehaviour.name}] Interaction ended.");
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
            string reason = "";

            if (smoothedLoudness > overloadThreshold)
            {
                newReaction = angryObj;
                reason = "Too loud (overload threshold)";
            }
            else if (smoothedLoudness > volumeThreshold)
            {
                if (npcIsTalking)
                {
                    newReaction = angryObj;
                    reason = "Talking over NPC";
                }
                else if (smoothedLoudness > envLoudness)
                {
                    newReaction = positiveObj;
                    reason = "Louder than environment";
                }
                else
                {
                    newReaction = angryObj;
                    reason = "Too quiet vs environment";
                }

                lowVolumeDuration = 0f;
            }
            else
            {
                lowVolumeDuration += Time.deltaTime;

                if (npcIsTalking)
                {
                    newReaction = positiveObj;
                    reason = "Respectful silence during NPC speech";
                }
                else if (lowVolumeDuration >= delayBeforeConfused)
                {
                    newReaction = confusedObj;
                    reason = "Prolonged silence";
                }
            }

            if (newReaction == null)
            {
                newReaction = confusedObj;
                reason = "Default fallback to confused";
            }

            bool canReact = reactionTimer <= 0f;

            if (newReaction != lastActiveReaction && canReact)
            {
                ShowOnly(newReaction);
                lastActiveReaction = newReaction;
                reactionTimer = (newReaction == positiveObj) ? positiveCooldown : angryCooldown;

                int scoreChange = 0;

                if (newReaction == positiveObj)
                    scoreChange = +5;
                else if (newReaction == angryObj)
                    scoreChange = -3;
                else if (newReaction == confusedObj)
                    scoreChange = -1;

                npcBehaviour.AdjustScore(scoreChange);
                NpcReputationUI.UpdateScore(npcBehaviour.GetScore(), npcBehaviour.GetMaxScore());

                Debug.Log($"[{npcBehaviour.name}] Reaction: {newReaction.name} ({reason}) | Mic: {micLoudness:F3}, Env: {envLoudness:F3}, Score: {npcBehaviour.GetScore()}/{npcBehaviour.GetMaxScore()}");
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
