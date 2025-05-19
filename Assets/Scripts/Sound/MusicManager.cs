using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class MusicManager : MonoBehaviour
    {
        [Header("Playlist")]
        [SerializeField] private List<AudioClip> musicClips;

        private List<AudioSource> djSources = new List<AudioSource>();
        private Dictionary<AudioSource, AudioClip> currentClips = new Dictionary<AudioSource, AudioClip>();

        private void Start()
        {
            djSources.AddRange(GetComponentsInChildren<AudioSource>());

            foreach (AudioSource source in djSources)
            {
                PlayRandomClip(source);
            }
        }

        private void Update()
        {
            foreach (AudioSource source in djSources)
            {
                if (!source.isPlaying)
                {
                    PlayRandomClip(source);
                }
            }
        }

        private void PlayRandomClip(AudioSource source)
        {
            if (musicClips.Count == 0) return;

            AudioClip nextClip;
            do
            {
                nextClip = musicClips[Random.Range(0, musicClips.Count)];
            } while (currentClips.ContainsKey(source) && nextClip == currentClips[source] && musicClips.Count > 1);

            
            source.clip = nextClip;
            source.Play();

            currentClips[source] = nextClip;
        }
    }
}