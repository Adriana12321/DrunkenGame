using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class MusicManager : MonoBehaviour
    {
        [Header("Playlist")]
        [SerializeField] private List<AudioClip> musicClips;

        [Header("Volume Dynamics")]
        [SerializeField] private float minVolume = 0.3f;
        [SerializeField] private float maxVolume = 1.0f;
        [SerializeField] private float volumeChangeInterval = 10f;
        [SerializeField, Range(0f, 1f)] private float muteChance = 0.1f;

        private List<AudioSource> djSources = new();
        private float volumeTimer;
        private AudioClip currentClip;

        void Start()
        {
            djSources.AddRange(GetComponentsInChildren<AudioSource>());
            PlayNewTrack();
        }

        void Update()
        {
            if (!djSources[0].isPlaying)
                PlayNewTrack();

            volumeTimer -= Time.deltaTime;
            if (volumeTimer <= 0f)
            {
                foreach (var source in djSources)
                    source.volume = Random.value < muteChance ? 0f : Random.Range(minVolume, maxVolume);

                volumeTimer = volumeChangeInterval;
            }
        }

        void PlayNewTrack()
        {
            if (musicClips.Count == 0) return;

            currentClip = musicClips[Random.Range(0, musicClips.Count)];
            foreach (var source in djSources)
            {
                source.clip = currentClip;
                source.Play();
            }
        }
    }
}