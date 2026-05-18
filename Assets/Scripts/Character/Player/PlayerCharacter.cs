using UnityEngine;

namespace Character.Player
{
    public class PlayerCharacter : CharacterBase
    {
        private void FixedUpdate()
        {
            ProcessTranslation();
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            MoveInput = moveInput;

            if (MoveInput.x == 0) return;
            
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (MoveInput.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }
        
        private void ProcessTranslation()
        {
            Rb?.MovePosition(Rb.position + MoveInput * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}