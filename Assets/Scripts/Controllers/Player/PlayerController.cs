using Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    [RequireComponent(typeof(PlayerInput))]
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
            if (ctx.performed)
            {
                _moveInput = ctx.ReadValue<Vector2>();
                _playerCharacter.SetMoveInput(_moveInput);
            }

            if (ctx.canceled)
            {
                _playerCharacter.SetMoveInput(Vector2.zero);
                _moveInput = Vector2.zero;
            }
        }
    }
}