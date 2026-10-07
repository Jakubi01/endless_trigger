using System.Collections.Generic;
using Character.Enemy;
using Items.Weapon;
using UnityEngine;

namespace Components.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Bomb Ability", menuName = "Endless Trigger/Abilities/Bomb Ability")]
    public class BombAbility : AbilityBase
    {
        private const float TargetRange = 10f;
        private const float BombSpeed = 20f;
        private const float AreaDuration = 5f;
        private const float ShotInterval = 0.1f;

        private float _damagePerSecond;
        private float _areaRadius;
        private int _throwCount;
        private int _shotsRemaining;
        private int _activeAreas;
        private float _nextShotTime;
        private bool _burstActive;

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            _damagePerSecond = 20f;
            _areaRadius = 1.5f;
            _throwCount = 1;
            _shotsRemaining = 0;
            _activeAreas = 0;
            _burstActive = false;
        }

        public override bool Execute()
        {
            if (!Owner) return false;

            if (!_burstActive)
            {
                if (!base.Execute()) return false;

                _burstActive = true;
                _shotsRemaining = _throwCount;
                _nextShotTime = Time.time;
            }

            if (_shotsRemaining > 0 && Time.time >= _nextShotTime)
            {
                LaunchBomb();
                _shotsRemaining--;
                _nextShotTime = Time.time + ShotInterval;
            }

            CompleteBurstIfReady();
            return true;
        }

        public override void Upgrade(float amount)
        {
            if (amount <= 0f) return;

            base.Upgrade(amount);
            _throwCount++;
            _damagePerSecond += 10f;
            _areaRadius *= 1.2f;
        }

        private void LaunchBomb()
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(Owner.transform.position, TargetRange);
            List<EnemyCharacterBase> targets = new();

            foreach (Collider2D nearbyCollider in nearbyColliders)
            {
                if (!nearbyCollider || !nearbyCollider.TryGetComponent(out EnemyCharacterBase enemy) || !enemy.IsAlive)
                    continue;

                if ((enemy.transform.position - Owner.transform.position).sqrMagnitude > TargetRange * TargetRange)
                    continue;

                if (!targets.Contains(enemy))
                    targets.Add(enemy);
            }

            if (targets.Count == 0) return;

            EnemyCharacterBase target = targets[Random.Range(0, targets.Count)];
            GameObject bombObject = new GameObject("ClayBomb");
            bombObject.transform.position = Owner.transform.position;
            SpriteRenderer spriteRenderer = bombObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = BombArea.VisualSprite;
            spriteRenderer.color = new Color(0.63f, 0.42f, 0.25f);
            spriteRenderer.sortingOrder = 1;
            bombObject.AddComponent<CircleCollider2D>().radius = 0.18f;

            Bomb bomb = bombObject.AddComponent<Bomb>();
            _activeAreas++;
            bomb.Initialize(target, BombSpeed, _damagePerSecond, AreaDuration, _areaRadius, OnAreaFinished);
        }

        private void OnAreaFinished()
        {
            _activeAreas = Mathf.Max(0, _activeAreas - 1);
            CompleteBurstIfReady();
        }

        private void CompleteBurstIfReady()
        {
            if (_burstActive && _shotsRemaining == 0 && _activeAreas == 0)
                _burstActive = false;
        }
    }
}
