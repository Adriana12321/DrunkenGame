using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loudnessDetection : MonoBehaviour
{
    private AudioClip micClip;
    public int sampleWindow = 64;
    
    private string currentMicDevice;

    [Header("Debug GUI Settings")]
    public bool showDebugGUI = true;
    public float debugBarWidth = 200f;
    public float debugBarHeight = 20f;
    public Vector2 debugBarPosition = new Vector2(10f, 10f);

    void Start()
    {
        MicToAudioClip();
    }

    private void Update()
    {
        Debug.Log("mic: " + currentMicDevice);

    }

    public void MicToAudioClip()
    {
        if (Microphone.devices.Length > 0)
        {
            currentMicDevice = Microphone.devices[0]; // default to first
            micClip = Microphone.Start(currentMicDevice, true, 20, AudioSettings.outputSampleRate);
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

        int micPosition = Microphone.GetPosition(currentMicDevice);
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
    
    public List<string> GetAvailableMicDevices()
    {
        return new List<string>(Microphone.devices);
    }
    public void SetMicDevice(string deviceName)
    {
        if (Microphone.devices.Length == 0 || !System.Array.Exists(Microphone.devices, d => d == deviceName))
        {
            Debug.LogWarning($"Microphone device '{deviceName}' not found.");
            return;
        }

        if (Microphone.IsRecording(currentMicDevice))
        {
            Microphone.End(currentMicDevice);
        }

        currentMicDevice = deviceName;
        micClip = Microphone.Start(currentMicDevice, true, 20, AudioSettings.outputSampleRate);
    }

}
