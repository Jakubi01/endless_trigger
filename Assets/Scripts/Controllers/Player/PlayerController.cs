using Character.Player;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerController : Controller
    {
        private PlayerCharacter _playerCharacter;
        private Vector2 _moveInput;

        protected override void Awake()
        {
            base.Awake();

            _playerCharacter = GetComponent<PlayerCharacter>();
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            _playerCharacter.SetMoveInput(_moveInput);
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
        }

        public void OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (UIManager.Instance)
                    UIManager.Instance.ShowPauseWindow();
            }
        }
    }
}
