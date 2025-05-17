using System.Collections;
using UnityEngine;

namespace Sound
{
    public class SoundFxManager : MonoBehaviour
    {
        public static SoundFxManager instance;

        [SerializeField] private AudioSource soundFxPrefab;
        [SerializeField] private AudioSource npcSoundFxPrefab;
        [SerializeField] private AudioClip[] dialogFxClips;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void PlaySoundFx(AudioClip clip, Transform spawnTransform, float volume)
        {
            AudioSource audioSource = Instantiate(soundFxPrefab, spawnTransform.position, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            var visualizer = FindObjectOfType<LoudnessVisualizer>();
            visualizer?.RegisterSource(audioSource);

            Destroy(audioSource.gameObject, clip.length);
        }

        public void PlayDialogSoundFx(Transform npcTransform, float volume)
        {
            float pitch = Random.Range(0.85f, 1.2f);

            AudioSource audioSource = Instantiate(npcSoundFxPrefab, npcTransform.position, Quaternion.identity, npcTransform);
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            var visualizer = FindObjectOfType<LoudnessVisualizer>();
            visualizer?.RegisterSource(audioSource);

            StartCoroutine(PlayGibberishSequence(audioSource));
        }

        private IEnumerator PlayGibberishSequence(AudioSource source)
        {
            if (dialogFxClips.Length == 0) yield break;

            int totalClips = Random.Range(6, 10);
            int clipsPlayed = 0;

            while (clipsPlayed < totalClips)
            {
                int burstCount = Random.Range(2, 5);
                burstCount = Mathf.Min(burstCount, totalClips - clipsPlayed);

                for (int i = 0; i < burstCount; i++)
                {
                    AudioClip gibberishClip = dialogFxClips[Random.Range(0, dialogFxClips.Length)];
                    source.clip = gibberishClip;
                    source.Play();
                    yield return new WaitForSeconds(gibberishClip.length);

                    clipsPlayed++;
                }

                if (clipsPlayed < totalClips)
                {
                    float burstDelay = Random.Range(0.3f, 0.6f);
                    yield return new WaitForSeconds(burstDelay);
                }
            }

            Destroy(source.gameObject);
        }
    }
}
