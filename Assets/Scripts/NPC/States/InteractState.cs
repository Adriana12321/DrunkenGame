using Sound;
using UnityEngine;

namespace NPC.States
{
    public class InteractState : ICharacterState
    {
    public loudnessDetection detector;
    public GameObject positiveObj;
    public GameObject negativeObj;
    public float volumeThreshold = 0.1f;
    public bool ifClicked = false;
    private float lowVolumeDuration = 0f;
    private float delayBeforeSwitch = 1.5f; 
    private AudioSource npcAudio;

        public void Initialize(loudnessDetection detector, GameObject positiveObj,
        GameObject negativeObj, float volumeThreshold, AudioSource npcAudio)
        {
            this.detector = detector;
            this.positiveObj = positiveObj;
            this.negativeObj = negativeObj;
            this.volumeThreshold = volumeThreshold;
            this.npcAudio = npcAudio;
        }
        
        public void OnEnter(NpcBehaviour context)
        {
            SoundFxManager.instance.PlayDialogSoundFx(context.transform, 1f);
            lowVolumeDuration = 1.5f;
            if (npcAudio != null && !npcAudio.isPlaying)
            {
                npcAudio.Play();
            }
        }

        public void OnUpdate()
        {
           float loudness = detector.getLoudnessFromMic();

    if (loudness > volumeThreshold)
    {
        lowVolumeDuration = 0f;
        positiveObj.SetActive(true);
        negativeObj.SetActive(false);
    }
    else
    {
        lowVolumeDuration += Time.deltaTime;

        if (lowVolumeDuration >= delayBeforeSwitch)
        {
            positiveObj.SetActive(false);
            negativeObj.SetActive(true);
        }
    }
        }

        public void OnExit()
        {   
            positiveObj.SetActive(false);
            negativeObj.SetActive(false);  
        }
    }
}