using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicInput : MonoBehaviour
{
    public string microphoneDevice;
    public int sampleRate = 44100;
    public int sampleWindow = 1024;
    public float knockThreshold = 0.2f;
    public float zcrThreshold = 0.15f;
    public int micIndex = 1;

    private AudioClip micClip;
    private float[] samples;


    
    
    // Start is called before the first frame update
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[micIndex];
            micClip = Microphone.Start(microphoneDevice, true, 1, sampleRate);
            samples = new float[sampleWindow];
            Debug.Log("Microphone started: " + microphoneDevice);
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }

    }

    void Update()
    {
        if (micClip == null) return;

        int micPos = Microphone.GetPosition(microphoneDevice) - sampleWindow;
        if (micPos < 0) return;

        micClip.GetData(samples, micPos);
        float rms = ComputeRMS(samples);
        float zcr = ComputeZCR(samples);

        if (rms > knockThreshold && zcr > zcrThreshold)
        {
            Debug.Log("Knock detected! RMS: " + rms.ToString("F3") + " ZCR: " + zcr.ToString("F3"));
            // Trigger your game logic here
        }
    }

    float ComputeRMS(float[] data)
    {
        float sum = 0f;
        foreach (float s in data)
        {
            sum += s * s;
        }
        return Mathf.Sqrt(sum / data.Length);
    }

    float ComputeZCR(float[] data)
    {
        int zeroCrossings = 0;
        for (int i = 1; i < data.Length; i++)
        {
            if ((data[i - 1] > 0f && data[i] < 0f) || (data[i - 1] < 0f && data[i] > 0f))
            {
                zeroCrossings++;
            }
        }
        return (float)zeroCrossings / data.Length;
    }
}
