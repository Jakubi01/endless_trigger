using System;
using Character.Player;
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
        private float _timer;
        private float _interval;

        protected override void Awake()
        {
            base.Awake();
            
            _playerCharacter = GetComponent<PlayerCharacter>();

            _interval = 5f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _interval)
            {
                _timer = 0f;
                // 여기에 공격 로직
                _playerCharacter.DoAttack();
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            _playerCharacter.SetMoveInput(_moveInput);
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}