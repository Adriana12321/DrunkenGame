using UnityEngine;
using Sound;

public class Reactions : MonoBehaviour
{
    [Header("Interact Settings")]
    public GameObject positiveObj;
    public GameObject negativeObj;
    public loudnessDetection detector;
    public float volumeThreshold = 0.2f;
    public AudioSource npcAudio;

    private float lowVolumeDuration = 0f;
    private float delayBeforeSwitch = 1.5f;
    private bool isReacting = false;

    public void StartReaction()
    {
        isReacting = true;
        lowVolumeDuration = delayBeforeSwitch;

        if (npcAudio != null && !npcAudio.isPlaying)
        {
            npcAudio.Play();
        }
    }

    public void StopReaction()
    {
        isReacting = false;
        if (positiveObj != null) positiveObj.SetActive(false);
        if (negativeObj != null) negativeObj.SetActive(false);
    }

    public void OnUpdate()
    {
        float loudness = detector.getLoudnessFromMic();

        if (loudness > volumeThreshold)
        {
            lowVolumeDuration = 0f;
            if (positiveObj != null) positiveObj.SetActive(true);
            if (negativeObj != null) negativeObj.SetActive(false);
        }
        else
        {
            lowVolumeDuration += Time.deltaTime;

            if (lowVolumeDuration >= delayBeforeSwitch)
            {
                if (positiveObj != null) positiveObj.SetActive(false);
                if (negativeObj != null) negativeObj.SetActive(true);
            }
        }
    }
}