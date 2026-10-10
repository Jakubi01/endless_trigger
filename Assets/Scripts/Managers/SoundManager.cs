using System;
using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Endless Trigger/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Header("UI Sounds (Common)")]
        public AudioClip buttonClick;
        public AudioClip popupOpen;
        public AudioClip gameStart;

        [Header("Common SFX")]
        public AudioClip playerHit;
        public AudioClip itemGet;

        [Header("Background SFX")] 
        public AudioClip lobbyBGM;
        public AudioClip inGameBGM;
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

        private SoundData _data;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
                _data = Instantiate(soundData);
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
        
        public void ApplyVolume(bool isBgmOn, float bgmVolume, float sfxVolume)
        {
            bgmSource.mute = !isBgmOn;
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
            uiSource.volume = sfxVolume;
        }

        public void PlayLobbyBGM()
        {
            if (!_data || !_data.lobbyBGM) return;
            
            if(bgmSource.isPlaying)
                bgmSource.Stop();

            bgmSource.clip = _data.lobbyBGM;
            bgmSource.Play();
        }

        public void PlayInGameBGM()
        {
            if (!_data || !_data.gameStart) return;
            
            if(bgmSource.isPlaying)
                bgmSource.Stop();
            
            bgmSource.clip = _data.gameStart;
            bgmSource.Play();
        }
        
        public void StopBGM() => bgmSource.Stop();

        public void PlaySFX(AudioClip clip)
        {
            if (!clip) return;
            
            sfxSource.PlayOneShot(clip);
        }
        
        public void PlayUIPopupOpen()
        {
            if (_data.popupOpen != null)
                uiSource.PlayOneShot(_data.popupOpen);
        }
    }
}