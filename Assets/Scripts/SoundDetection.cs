using UnityEngine;
using AS.MicControl;

public class SoundDetection : MonoBehaviour
{
    public MicControlC micControl;
    public float knockThreshold = 0.7f; // Threshold to detect a knock based on loudness
    public float blowThreshold = 0.2f; // Threshold to detect a blow based on spectrum data
    public float blowDurationThreshold = 1.0f; // Duration of blow in seconds
    private float blowStartTime;

    void Start()
    {
        // Ensure the MicControl is initialized
        if (micControl != null && micControl.Initialized)
        {
            // Optionally, start listening for microphone input
            micControl.StartMicrophone();
        }
    }

    void Update()
    {
        if (micControl != null && micControl.Initialized)
        {
            // Get loudness and spectrum data from the microphone
            float loudness = micControl.loudness;
            float[] spectrum = micControl.spectrumData;

            // Check if the loudness exceeds the knock threshold
            if (loudness > knockThreshold)
            {
                // If the loudness is above the knock threshold, detect it as a knock
                DetectKnock();
            }

            // Check if the spectrum data indicates a blowing sound (sustained frequencies)
            if (IsBlowingSound(spectrum))
            {
                // If a blow sound is detected, check duration
                DetectBlow();
            }
        }
    }

    // Detect a sharp knock sound based on loudness
    void DetectKnock()
    {
        Debug.Log("Knock Detected!");
        // Add your knock-related functionality here (e.g., trigger a visual cue or sound)
    }

    // Check if the spectrum data indicates a blow (sustained low frequencies)
    bool IsBlowingSound(float[] spectrum)
    {
        // Check for sustained low frequencies which are common in blowing sounds
        float lowFrequencySum = 0f;
        for (int i = 0; i < spectrum.Length / 4; i++) // Check the lower frequencies (could be tweaked)
        {
            lowFrequencySum += spectrum[i];
        }

        // Check if the low frequency sum exceeds a threshold, indicating blowing
        return lowFrequencySum / (spectrum.Length / 4) > blowThreshold;
    }

    // Detect if a blow sound has been sustained for a certain duration
    void DetectBlow()
    {
        // Start counting time when a blow is first detected
        if (blowStartTime == 0)
        {
            blowStartTime = Time.time;
        }

        // If the blow has lasted longer than the threshold, register it
        if (Time.time - blowStartTime > blowDurationThreshold)
        {
            Debug.Log("Blow Detected!");
            // Add your blow-related functionality here (e.g., trigger a different event)
            blowStartTime = 0; // Reset the blow start time
        }
    }
}
