using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Header("UI Sounds (Common)")]
        public AudioClip buttonClick;
        public AudioClip popupOpen;
        public AudioClip gameStart;

        [Header("Common SFX")]
        public AudioClip playerHit;
        public AudioClip itemGet;
    }
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Sound Data")]
        [SerializeField] private SoundData soundData;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetupAudioSources()
        {
            if (bgmSource != null) 
                bgmSource.loop = true;

            if (uiSource != null) 
                uiSource.ignoreListenerPause = true;
        }

        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource.clip == clip) return;
            
            bgmSource.Stop();
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        
        public void StopBGM() => bgmSource.Stop();

        public void PlaySFX(AudioClip clip)
        {
            if (!clip) return;
            
            sfxSource.PlayOneShot(clip);
        }
        
        public void PlayPlayerHitSFX()
        {
            if (soundData.playerHit != null)
                sfxSource.PlayOneShot(soundData.playerHit);
        }
        
        public void PlayUIPopupOpen()
        {
            if (soundData.popupOpen != null)
                uiSource.PlayOneShot(soundData.popupOpen);
        }
    }
}