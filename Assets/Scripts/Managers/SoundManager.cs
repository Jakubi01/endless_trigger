
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum BgmType
    {
        Lobby,
        InGame,
        Boss
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
        [SerializeField] private AudioSource nextBgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        [Header("BGM Settings")]
        [SerializeField, Min(0.01f)] private float bgmFadeDuration = 2f;

        [Header("SFX Throttle")]
        [SerializeField] private float sfxMinInterval = 0.05f;

        private readonly Dictionary<AudioClip, float> _lastSfxTime = new();

        private SoundData _data;

        private AudioSource _currentSource;
        private AudioSource _nextSource;

        private Coroutine _bgmCoroutine;

        private float _bgmVolume = 1f;
        private float _currentWeight = 1f;
        private float _nextWeight;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _data = Instantiate(soundData);

            SetupAudioSources();
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            Instance = null;
        }

        private void SetupAudioSources()
        {
            _currentSource = bgmSource;
            _nextSource = nextBgmSource;

            if (bgmSource)
            {
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.volume = 1f;
            }

            if (nextBgmSource)
            {
                nextBgmSource.loop = true;
                nextBgmSource.playOnAwake = false;
                nextBgmSource.volume = 0f;
            }

            if (uiSource)
                uiSource.ignoreListenerPause = true;
        }

        public void ApplyVolume(bool isBgmOn, float bgmVolume, float sfxVolume)
        {
            _bgmVolume = isBgmOn ? bgmVolume : 0f;

            UpdateBgmVolume();

            if (sfxSource)
                sfxSource.volume = sfxVolume;

            if (uiSource)
                uiSource.volume = sfxVolume;
        }

        private void UpdateBgmVolume()
        {
            if (_currentSource)
                _currentSource.volume = _bgmVolume * _currentWeight;

            if (_nextSource)
                _nextSource.volume = _bgmVolume * _nextWeight;
        }

        public void PlayBGM(BgmType bgmType)
        {
            if (!_data || !_data.IsBGMValid())
                return;

            AudioClip clip = bgmType switch
            {
                BgmType.Lobby => _data.lobbyBGM,
                BgmType.InGame => _data.inGameBGM,
                BgmType.Boss => _data.bossBGM,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(bgmType), bgmType, null)
            };

            if (!clip)
                return;

            if (_bgmCoroutine == null && _currentSource.clip == clip && _currentSource.isPlaying)
                return;

            StartBgmTransition(clip);
        }

        private void StartBgmTransition(AudioClip clip)
        {
            if (_bgmCoroutine != null)
            {
                StopCoroutine(_bgmCoroutine);
                _bgmCoroutine = null;

                // 현재 볼륨이 더 큰 AudioSource를 유지한다.
                if (_nextWeight > _currentWeight)
                {
                    (_currentSource, _nextSource) =
                        (_nextSource, _currentSource);

                    (_currentWeight, _nextWeight) =
                        (_nextWeight, _currentWeight);
                }

                // 기존 전환에서 사용하던 나머지 AudioSource 정리
                _nextSource.Stop();
                _nextSource.clip = null;

                _nextWeight = 0f;
            }

            _nextSource.clip = clip;
            _nextSource.volume = 0f;
            _nextSource.Play();

            _bgmCoroutine = StartCoroutine(CrossfadeBGM());
        }

        private IEnumerator CrossfadeBGM()
        {
            float elapsed = 0f;

            float startWeight = _currentWeight;

            while (elapsed < bgmFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / bgmFadeDuration);

                _currentWeight = Mathf.Lerp(startWeight, 0f, t);
                _nextWeight = Mathf.Lerp(0f, 1f, t);

                UpdateBgmVolume();

                yield return null;
            }

            _currentSource.Stop();
            _currentSource.clip = null;

            (_currentSource, _nextSource) =
                (_nextSource, _currentSource);

            _currentWeight = 1f;
            _nextWeight = 0f;

            UpdateBgmVolume();

            _bgmCoroutine = null;
        }

        public void StopBGM()
        {
            if (_bgmCoroutine != null)
            {
                StopCoroutine(_bgmCoroutine);
                _bgmCoroutine = null;
            }

            _currentSource.Stop();
            _nextSource.Stop();

            _currentSource.clip = null;
            _nextSource.clip = null;

            _currentWeight = 1f;
            _nextWeight = 0f;

            UpdateBgmVolume();
        }

        public void PlaySfx(AudioClip clip)
        {
            PlayThrottled(sfxSource, clip);
        }

        public void Play(SfxType type)
        {
            if (!_data)
                return;

            switch (type)
            {
                case SfxType.ItemGet: PlayThrottled(sfxSource, _data.itemGet); break;
                case SfxType.LevelUp: PlayThrottled(sfxSource, _data.levelUp); break;
                case SfxType.StageClear: PlayThrottled(sfxSource, _data.stageClear); break;
                case SfxType.PopupOpen: PlayThrottled(uiSource, _data.popupOpen); break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void PlayThrottled(AudioSource source, AudioClip clip)
        {
            if (!source || !clip)
                return;

            var now = Time.unscaledTime;

            if (_lastSfxTime.TryGetValue(clip, out var last) &&
                now - last < sfxMinInterval)
                return;

            _lastSfxTime[clip] = now;

            source.PlayOneShot(clip);
        }
    }
}
