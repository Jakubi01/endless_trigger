using Managers;
using UnityEngine;

namespace Audio
{
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
                    manager.PlayBGM(BgmType.Lobby);
                    break;
                case BgmType.InGame:
                    manager.PlayBGM(BgmType.InGame);
                    break;
            }
        }
    }
}