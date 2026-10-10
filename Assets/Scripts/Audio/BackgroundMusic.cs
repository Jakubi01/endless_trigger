using System;
using Managers;
using UnityEngine;

namespace Audio
{
    public enum BgmType
    {
        Lobby,
        InGame
    }
    
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] private BgmType bgmType = BgmType.Lobby;
        
        private void Start()
        {
            var manager = SoundManager.Instance;
            if(!manager) return;
            
            switch (bgmType)
            {
                case BgmType.Lobby:
                    manager.PlayLobbyBGM();
                    break;
                case BgmType.InGame:
                    manager.PlayInGameBGM();
                    break;
            }
        }
    }
}