using System;
using System.Collections.Generic;
using Character.Enemy;
using UnityEngine;

namespace Items.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BombArea : MonoBehaviour
    {
        private const float DamageTickInterval = 1f;
        private const int SpriteSize = 64;
        private static Sprite _visualSprite;

        public static Sprite VisualSprite
        {
            get
            {
                if (_visualSprite) return _visualSprite;

                Texture2D texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };

                Color[] pixels = new Color[SpriteSize * SpriteSize];
                Vector2 center = new Vector2((SpriteSize - 1) * 0.5f, (SpriteSize - 1) * 0.5f);
                float outerRadius = SpriteSize * 0.48f;
                float innerRadius = SpriteSize * 0.34f;

                for (int y = 0; y < SpriteSize; y++)
                {
                    for (int x = 0; x < SpriteSize; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), center);
                        float alpha = Mathf.Clamp01(outerRadius - distance);
                        if (distance < innerRadius)
                            alpha *= 0.35f;

                        pixels[y * SpriteSize + x] = new Color(1f, 1f, 1f, alpha);
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();
                _visualSprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, SpriteSize, SpriteSize),
                    new Vector2(0.5f, 0.5f),
                    SpriteSize);
                _visualSprite.name = "ClayBombVisual";
                return _visualSprite;
            }
        }

        private float _damagePerSecond;
        private float _duration;
        private float _radius;
        private float _elapsed;
        private float _nextDamageTime;
        private Action _onFinished;
        private readonly HashSet<EnemyCharacterBase> _damagedEnemies = new();

        public void Initialize(float damagePerSecond, float duration, float radius, Action onFinished)
        {
            _damagePerSecond = Mathf.Max(0f, damagePerSecond);
            _duration = Mathf.Max(0f, duration);
            _radius = Mathf.Max(0.1f, radius);
            _onFinished = onFinished;
            _elapsed = 0f;
            _nextDamageTime = 0f;

            transform.localScale = Vector3.one * (_radius * 2f);
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.5f;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _duration)
            {
                Finish();
                return;
            }

            while (_nextDamageTime <= _elapsed && _nextDamageTime < _duration)
            {
                ApplyDamage();
                _nextDamageTime += DamageTickInterval;
            }
        }

        private void ApplyDamage()
        {
            _damagedEnemies.Clear();
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, _radius);

            foreach (Collider2D overlap in overlaps)
            {
                if (!overlap || !overlap.TryGetComponent(out EnemyCharacterBase enemy) || !enemy.IsAlive)
                    continue;

                if (_damagedEnemies.Add(enemy))
                    enemy.TakeDamage(_damagePerSecond);
            }
        }

        private void Finish()
        {
            Action callback = _onFinished;
            _onFinished = null;
            callback?.Invoke();
            Destroy(gameObject);
        }
    }
}
