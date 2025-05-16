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
        
            float clipLenght = clip.length;
        
            Destroy(audioSource.gameObject, clipLenght);
        }

        public void PlayDialogSoundFx(Transform npcTransform, float volume)
        {
            float pitch = Random.Range(0.85f, 1.2f);
    
            AudioSource audioSource = Instantiate(npcSoundFxPrefab, npcTransform.position, Quaternion.identity, npcTransform);
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            
            StartCoroutine(PlayGibberishSequence(audioSource));
        }
        
        private IEnumerator PlayGibberishSequence(AudioSource source)
        {
            int clipCount = Random.Range(3, 7);

            for (int i = 0; i < clipCount; i++)
            {
                if (dialogFxClips.Length == 0) yield break;

                AudioClip gibberishClip = dialogFxClips[Random.Range(0, dialogFxClips.Length)];
                source.clip = gibberishClip;
                source.Play();

                yield return new WaitForSeconds(gibberishClip.length + Random.Range(0.01f, 0.2f));
            }
            Destroy(source.gameObject);
        }

    }
}