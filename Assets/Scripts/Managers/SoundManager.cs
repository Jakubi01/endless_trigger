using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum BgmType
    {
        Lobby,
        InGame
    }
    
    public enum SfxType
    {
        ItemGet,
        LevelUp,
        StageClear,
        PopupOpen
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

        [Header("SFX Throttle")]
        [SerializeField] private float sfxMinInterval = 0.05f;

        private readonly Dictionary<AudioClip, float> _lastSfxTime = new();
        
        private SoundData _data;

        private void Awake()
        {
            if (!Instance)
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
            if (bgmSource) 
                bgmSource.loop = true;

            if (uiSource) 
                uiSource.ignoreListenerPause = true;
        }
        
        public void ApplyVolume(bool isBgmOn, float bgmVolume, float sfxVolume)
        {
            bgmSource.mute = !isBgmOn;
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
            uiSource.volume = sfxVolume;
        }

        public void PlayBGM(BgmType bgmType)
        {
            if (!_data || !_data.IsBGMValid()) return;
            
            if(bgmSource.isPlaying)
                bgmSource.Stop();

            bgmSource.clip = bgmType switch
            {
                BgmType.Lobby => _data.lobbyBGM,
                BgmType.InGame => _data.inGameBGM,
                _ => throw new ArgumentOutOfRangeException(nameof(bgmType), bgmType, null)
            };

            bgmSource.Play();
        }
        
        public void StopBGM() => bgmSource.Stop();

        public void PlaySfx(AudioClip clip) => PlayThrottled(sfxSource, clip);

        public void Play(SfxType type)
        {
            if (!_data) return;
            
            switch (type)
            {
                case SfxType.ItemGet:    PlayThrottled(sfxSource, _data.itemGet); break;
                case SfxType.LevelUp:    PlayThrottled(sfxSource, _data.levelUp); break;
                case SfxType.StageClear: PlayThrottled(sfxSource, _data.stageClear); break;
                case SfxType.PopupOpen:  PlayThrottled(uiSource, _data.popupOpen); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void PlayThrottled(AudioSource source, AudioClip clip)
        {
            if (!clip) return;

            var now = Time.unscaledTime;
            if (_lastSfxTime.TryGetValue(clip, out var last) && now - last < sfxMinInterval)
                return;

            _lastSfxTime[clip] = now;
            source.PlayOneShot(clip);
        }
    }
}