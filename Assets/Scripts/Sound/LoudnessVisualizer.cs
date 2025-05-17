using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoudnessVisualizer : MonoBehaviour
{
    [Header("Mic Settings")]
    public loudnessDetection micDetector;
    public Slider micLoudnessSlider;
    public Text micLoudnessText;

    [Header("Environment Settings")]
    public Slider envLoudnessSlider;
    public Text envLoudnessText;
    public int envSampleWindow = 256;

    public List<AudioSource> envAudioSources = new();

    private float smoothedEnvLoudness = 0f;
    private float smoothing = 0.2f;

    void Start()
    {
        envAudioSources.Clear();

        var musicManager = FindObjectOfType<Sound.MusicManager>();
        if (musicManager != null)
        {
            foreach (var source in musicManager.GetComponentsInChildren<AudioSource>())
            {
                RegisterSource(source);
            }
        }

        // Optional: auto-attach to player's listener
        if (TryGetComponent<AudioListener>(out _) == false)
        {
            var listener = FindObjectOfType<AudioListener>();
            if (listener != null)
            {
                transform.position = listener.transform.position;
            }
        }
    }

    void Update()
    {
        // Mic loudness
        float micLoudness = micDetector.getLoudnessFromMic();
        float micDisplay = Mathf.Clamp01(micLoudness * 5f);
        micLoudnessSlider.value = micDisplay;
        if (micLoudnessText) micLoudnessText.text = $"Mic: {micLoudness:F3}";

        // Environment loudness (GetOutputData)
        float envLoudness = GetAverageEnvironmentLoudness();
        smoothedEnvLoudness = Mathf.Lerp(smoothedEnvLoudness, envLoudness, smoothing);
        float envDisplay = Mathf.Clamp01(smoothedEnvLoudness * 5f);
        envLoudnessSlider.value = envDisplay;
        if (envLoudnessText) envLoudnessText.text = $"Env: {smoothedEnvLoudness:F3}";
    }

    public float GetAverageEnvironmentLoudness()
    {
        float total = 0f;
        int activeSources = 0;

        foreach (AudioSource source in envAudioSources)
        {
            if (source != null && source.isPlaying)
            {
                float[] samples = new float[envSampleWindow];
                source.GetOutputData(samples, 0);

                float sum = 0f;
                foreach (var s in samples)
                    sum += Mathf.Abs(s);

                total += sum / envSampleWindow;
                activeSources++;
            }
        }

        return activeSources > 0 ? total / activeSources : 0f;
    }

    public void RegisterSource(AudioSource source)
    {
        if (source != null && !envAudioSources.Contains(source))
        {
            envAudioSources.Add(source);
        }
    }

    public void UnregisterSource(AudioSource source)
    {
        if (source != null && envAudioSources.Contains(source))
        {
            envAudioSources.Remove(source);
        }
    }
}
