using System.Collections.Generic;
using Character.Enemy;
using Items.Weapon;
using UnityEngine;

namespace Components.AbilitySystemComponent.Abilities
{
    [CreateAssetMenu(fileName = "Bomb Ability", menuName = "Endless Trigger/Abilities/Bomb Ability")]
    public class BombAbility : AbilityBase
    {
        [SerializeField] private GameObject bombPrefab;
        
        private const float TargetRange = 10f;
        private const float BombSpeed = 5f;
        private const float AreaDuration = 5f;
        private const float ShotInterval = 0.1f;

        private float _damagePerSecond;
        private float _areaRadius;
        private int _throwCount;
        private int _shotsRemaining;
        private float _nextShotTime;
        private bool _burstActive;
        private bool _cooldownStartedForBurst;
        
        private readonly List<Collider2D> _colliderBuffer = new();

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            _damagePerSecond = 20f;
            _areaRadius = 1f;
            _throwCount = 1;
            _shotsRemaining = 0;
            _burstActive = false;
            _cooldownStartedForBurst = false;
        }

        public override bool Execute()
        {
            if (!Owner) return false;

            if (!_burstActive)
            {
                if (!base.Execute()) return false;

                _burstActive = true;
                _cooldownStartedForBurst = false;
                _shotsRemaining = _throwCount;
                _nextShotTime = Time.time;
            }

            var launchedBomb = false;
            if (_shotsRemaining > 0 && Time.time >= _nextShotTime)
            {
                launchedBomb = LaunchBomb();
                if (launchedBomb && !_cooldownStartedForBurst)
                {
                    StartCooldown();
                    _cooldownStartedForBurst = true;
                }

                _shotsRemaining--;
                _nextShotTime = Time.time + ShotInterval;
            }

            CompleteBurstIfReady();
            return launchedBomb;
        }

        public override void Upgrade(float amount)
        {
            if (amount <= 0f) return;

            base.Upgrade(amount);
            _throwCount++;
            _damagePerSecond += 10f;
            _areaRadius *= 1.2f;
        }

        private bool LaunchBomb()
        {
            var result = Physics2D.OverlapCircle(Owner.transform.position, TargetRange, ContactFilter2D.noFilter, _colliderBuffer);
            if (result <= 0) return false;
            
            List<EnemyCharacterBase> targets = new();

            foreach (var nearbyCollider in _colliderBuffer)
            {
                if (!nearbyCollider || !nearbyCollider.TryGetComponent(out EnemyCharacterBase enemy) || !enemy.IsAlive)
                    continue;

                if ((enemy.transform.position - Owner.transform.position).sqrMagnitude > TargetRange * TargetRange)
                    continue;

                if (!targets.Contains(enemy))
                    targets.Add(enemy);
            }

            if (targets.Count == 0 || !bombPrefab) return false;

            EnemyCharacterBase target = targets[Random.Range(0, targets.Count)];
            
            var bombObject = Instantiate(bombPrefab, Owner.transform.position, Quaternion.identity);
            if (!bombObject.TryGetComponent(out Bomb bomb))
            {
                Destroy(bombObject);
                return false;
            }
            
            bomb.Initialize(target, BombSpeed, _damagePerSecond, AreaDuration, _areaRadius, null);
            SoundManagerInstance?.PlaySfx(executeClip);
            return true;
        }

        private void CompleteBurstIfReady()
        {
            if (_burstActive && _shotsRemaining == 0)
                _burstActive = false;
        }
    }
}
