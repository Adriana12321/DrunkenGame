using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loudnessDetection : MonoBehaviour
{
    private AudioClip micClip;
    public int sampleWindow = 64;

    // Start is called before the first frame update
    void Start()
    {
        MicToAudioClip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MicToAudioClip(){
        string mic = Microphone.devices[0];
        micClip = Microphone.Start(mic, true, 20, AudioSettings.outputSampleRate);
    }

    public float getLoudnessFromMic(){
        return getLoudnessFromAudioclip(Microphone.GetPosition(Microphone.devices[0]), micClip);
    }

    public float getLoudnessFromAudioclip(int clipPosition, AudioClip clip){
        int startPosition = clipPosition - sampleWindow;

        if(startPosition < 0){
            return 0;
        }

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        for(int i = 0; i < sampleWindow; i++){
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        float op = totalLoudness / sampleWindow;
        return op;
    }
 }
