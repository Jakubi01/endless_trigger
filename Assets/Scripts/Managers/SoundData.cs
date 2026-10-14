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
        public AudioClip itemGet;
        public AudioClip levelUp;
        public AudioClip stageClear;

        [Header("Background SFX")] 
        public AudioClip lobbyBGM;
        public AudioClip inGameBGM;

        public bool IsBGMValid()
        {
            return lobbyBGM && inGameBGM;
        }
    }
}