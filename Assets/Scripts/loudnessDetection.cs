using System.Collections;
using UnityEngine;

public class loudnessDetection : MonoBehaviour
{
    private AudioClip micClip;
    public int sampleWindow = 64;

    [Header("Debug GUI Settings")]
    public bool showDebugGUI = true;
    public float debugBarWidth = 200f;
    public float debugBarHeight = 20f;
    public Vector2 debugBarPosition = new Vector2(10f, 10f);

    void Start()
    {
        MicToAudioClip();
    }

    public void MicToAudioClip()
    {
        if (Microphone.devices.Length > 0)
        {
            string mic = Microphone.devices[1];
            micClip = Microphone.Start(mic, true, 20, AudioSettings.outputSampleRate);
        }
        else
        {
            Debug.LogWarning("No microphone detected!");
        }
    }

    public float getLoudnessFromMic()
    {
        if (Microphone.devices.Length == 0 || micClip == null)
            return 0;

        int micPosition = Microphone.GetPosition(Microphone.devices[0]);
        return getLoudnessFromAudioclip(micPosition, micClip);
    }

    public float getLoudnessFromAudioclip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if (startPosition < 0 || clip == null)
            return 0;

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }

    void OnGUI()
    {
        if (!showDebugGUI) return;

        float loudness = getLoudnessFromMic();
        float barLength = Mathf.Clamp(loudness * debugBarWidth, 0, debugBarWidth);

        GUI.Box(
            new Rect(debugBarPosition.x, debugBarPosition.y, barLength, debugBarHeight),
            $"Loudness: {loudness:0.000}"
        );
    }
}
