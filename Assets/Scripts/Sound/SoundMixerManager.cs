using System;
using UnityEngine;
using UnityEngine.Audio;
namespace Sound
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] AudioMixer mixer;

        public void SetMasterVolume(float volume)
        {
            mixer.SetFloat("masterVolume", volume);
        }

        public void SetFxVolume(float volume)
        {
            mixer.SetFloat("fxVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            mixer.SetFloat("musicVolume", volume);
        }
    
        
    }
    
}