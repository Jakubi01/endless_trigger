using System.Collections;
using System.Collections.Generic;
using Character;
using Character.Player;
using Items.Weapon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components
{
    public class EnemyBossCombatComponent : ComponentBase
    {
        private enum EPhase
        {
            Phase1,
            Phase2,
            Phase3
        }

        [Header("Selection")]
        [Tooltip("Pattern 1 ~ 5 weight (index 0 = pattern 1)")]
        [SerializeField] private float[] weights = { 1f, 1f, 1f, 1f, 1f };
        [SerializeField] private float restMin = 1f;
        [SerializeField] private float restMax = 1.5f;
        [SerializeField] private float dashPillarRest = 2f;

        [Header("Phase")]
        [SerializeField] private float phase2DamageMultiplier = 1.1f;
        [SerializeField] private float phase3DamageMultiplier = 1.15f;
        [SerializeField] private float phase3SpeedMultiplier = 1.3f;

        [Header("Cooldowns")]
        [SerializeField] private float pattern1Cooldown = 3f;
        [SerializeField] private float pattern2Cooldown = 4f;
        [SerializeField] private float pattern3Cooldown = 6f;
        [SerializeField] private float pattern4Cooldown = 10f;
        [SerializeField] private float pattern5Cooldown = 8f;

        [Header("Shared")]
        // [Resource] Telegraph_Fan_Red_96x168 프리팹: 4프레임 루프, 피벗 왼쪽 중앙, +X 방향 (②④ 공용)
        [SerializeField] private GameObject fanTelegraphPrefab;

        [Header("Pattern 1 - SwordAura")]
        // [Resource] SwordWave_Fly_64 프리팹: 루프, 오른쪽 향함 + Rigidbody2D + Trigger Collider2D + BossProjectile
        [SerializeField] private GameObject swordAuraPrefab;
        // [Resource] SwordWave_Vanish_64 프리팹: 루프 끔, 재생 후 자동 삭제
        [SerializeField] private GameObject swordAuraVanishPrefab;
        // [Resource] SwordWave_HitSpark_48 프리팹: 재생 후 자동 삭제
        [SerializeField] private GameObject swordAuraHitSparkPrefab;
        // [Resource] 베기 애니메이션 (아직 없으므로 slashFrameDelay로 베기 프레임 시점을 대신함)
        [SerializeField] private float slashFrameDelay = 0.3f;
        [SerializeField] private float swordAuraSpeed = 8f;
        [SerializeField] private float swordAuraLifetime = 2f;
        [SerializeField] private float swordAuraDamage = 30f;
        [SerializeField] private float swordAuraFanAngle = 25f;
        [SerializeField] private int swordAuraSpinCount = 8;
        [SerializeField] private float swordAuraSpinInterval = 0.1f;
        [SerializeField] private float swordAuraSpinDamage = 25f;

        [Header("Pattern 2 - SwingGreatSword")]
        [Min(0.5f)] [SerializeField] private float swingTelegraphTime = 0.6f;
        [SerializeField] private float swingRange = 3f;
        [SerializeField] private float swingAngle = 120f;
        [SerializeField] private float swingDamage = 50f;

        [Header("Pattern 3 - BoneExplosion")]
        // [Resource] Bone_Spin_48 프리팹: 루프 + 자식으로 Bone_Trail_48x32 + Rigidbody2D + Trigger Collider2D + BossProjectile
        [SerializeField] private GameObject bonePrefab;
        // [Resource] Bone_Shatter_48 프리팹: 재생 후 자동 삭제
        [SerializeField] private GameObject boneShatterPrefab;
        // [Resource] BoneBurst_Ring_128 프리팹: 재생 후 자동 삭제
        [SerializeField] private GameObject boneBurstRingPrefab;
        [SerializeField] private int boneCount = 12;
        [SerializeField] private float boneSpeed = 6f;
        [SerializeField] private float boneLifetime = 3f;
        [SerializeField] private float boneDamage = 15f;
        [SerializeField] private float boneBurstInterval = 0.6f;

        [Header("Pattern 4 - DashSlash")]
        // [Resource] Telegraph_Line_Red_256x48 프리팹: 4프레임 루프, 피벗 왼쪽 중앙, +X 방향 (8m×1.5m)
        [SerializeField] private GameObject dashLinePrefab;
        // [Resource] 검기 잔상: SwordWave_Vanish_64 프리팹 재사용 가능
        [SerializeField] private GameObject dashAfterimagePrefab;
        [Min(1f)] [SerializeField] private float dashTelegraphTime = 1f;
        [SerializeField] private float dashDistance = 8f;
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float afterimageSpacing = 1f;
        [SerializeField] private float afterimageLifetime = 1f;
        [SerializeField] private float dashSlashRange = 3f;
        [SerializeField] private float dashSlashAngle = 120f;
        [SerializeField] private float dashSlashDamage = 60f;

        [Header("Pattern 5 - SoulPillar")]
        // [Resource] Telegraph_Circle_Blue_96 프리팹: 4프레임 루프, 피벗 중앙 (지름 3m)
        [SerializeField] private GameObject pillarTelegraphPrefab;
        // [Resource] SoulPillar_96x128 프리팹: 8프레임 루프 끔, 피벗은 바닥이 예고 원 중심에 맞도록 조정
        [SerializeField] private GameObject pillarPrefab;
        [SerializeField] private int pillarCount = 6;
        [SerializeField] private float pillarSpawnRadius = 6f;
        [Min(1.2f)] [SerializeField] private float pillarTelegraphTime = 1.2f;
        [SerializeField] private float pillarRadius = 1.5f;
        [SerializeField] private float pillarLifetime = 1f;
        [SerializeField] private float pillarDamage = 50f;

        private Transform _target;
        private Animator _animator;
        private readonly float[] _nextTime = new float[6]; // 인덱스 = 패턴 번호
        private float _restUntil;
        private float _pairRestUntil;
        private int _lastPattern;
        private EPhase _phase;
        private Vector2 _facing = Vector2.right;

        private WaitForSeconds _pillarWaitForSeconds;
        private WaitForSeconds _dashWaitForSeconds;
        private WaitForSeconds _boneBurstWaitForSeconds;
        private WaitForSeconds _swingWaitForSeconds;
        private WaitForSeconds _swordAuraSpinWaitForSeconds;
        private WaitForSeconds _slashFrameWaitForSeconds;
        
        public bool IsExecutingPattern { get; private set; }

        // 이동 컴포넌트에서 곱해서 사용
        public float MoveSpeedMultiplier => _phase == EPhase.Phase3 ? phase3SpeedMultiplier : 1f;

        private float Mult => _phase switch
        {
            EPhase.Phase3 => phase3DamageMultiplier,
            EPhase.Phase2 => phase2DamageMultiplier,
            _ => 1f
        };

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            
            _pillarWaitForSeconds = new WaitForSeconds(pillarTelegraphTime);
            _dashWaitForSeconds = new WaitForSeconds(dashTelegraphTime);
            _boneBurstWaitForSeconds = new WaitForSeconds(boneBurstInterval);
            _swingWaitForSeconds = new WaitForSeconds(swingTelegraphTime);
            _swordAuraSpinWaitForSeconds = new WaitForSeconds(swordAuraSpinInterval);
        }
        
        public void SetTarget(Transform target)
        {
            if (!target)
            {
                var t = FindFirstObjectByType<PlayerCharacter>();
                _target = t.transform;
                return;
            }
            
            _target = target;
        }

        
        public void TryExecuteNextPattern(HealthComponent healthComponent)
        {
            if (IsExecutingPattern || !healthComponent || !_target) return;
            if (Time.time < _restUntil) return;

            _phase = GetPhase(healthComponent.MaxHealth, healthComponent.CurrentHealth);

            var maxId = _phase switch
            {
                EPhase.Phase1 => 2,
                EPhase.Phase2 => 4,
                _ => 5
            };

            var candidates = new List<int>();
            for (var id = 1; id <= maxId; id++)
            {
                if (CanUse(id)) candidates.Add(id);
            }

            if (candidates.Count == 0) return;

            ExecutePattern(PickWeighted(candidates));
        }

        private bool CanUse(int id)
        {
            if (id == _lastPattern) return false;
            if (Time.time < _nextTime[id]) return false;
            if (id == 2 && Vector2.Distance(_target.position, transform.position) > swingRange) return false;

            // 4와 5 사이에는 최소 dashPillarRest초
            var isDashPillarPair = (id == 4 && _lastPattern == 5) || (id == 5 && _lastPattern == 4);
            if (isDashPillarPair && Time.time < _pairRestUntil) return false;

            return true;
        }

        private int PickWeighted(List<int> candidates)
        {
            var total = 0f;
            foreach (var id in candidates) total += weights[id - 1];

            var roll = Random.Range(0f, total);
            foreach (var id in candidates)
            {
                roll -= weights[id - 1];
                if (roll <= 0f) return id;
            }

            return candidates[^1]; // count - 1
        }

        private float CooldownOf(int id)
        {
            return id switch
            {
                1 => pattern1Cooldown,
                2 => pattern2Cooldown,
                3 => pattern3Cooldown,
                4 => pattern4Cooldown,
                _ => pattern5Cooldown
            };
        }

        private void ExecutePattern(int patternId)
        {
            IsExecutingPattern = true;
            _lastPattern = patternId;
            _nextTime[patternId] = Time.time + CooldownOf(patternId);
            
            switch (patternId)
            {
                case 1: StartCoroutine(SwordAura()); break;
                case 2: StartCoroutine(SwingGreatSword()); break;
                case 3: StartCoroutine(BoneExplosion()); break;
                case 4: StartCoroutine(DashSlash()); break;
                case 5: StartCoroutine(SoulPillar()); break;
            }
        }

        // 패턴 코루틴이 끝날 때 호출됨 (애니메이션 이벤트에 등록하면 이중 호출되니 주의)
        public void ClearExecutingFlag()
        {
            IsExecutingPattern = false;
            _restUntil = Time.time + Random.Range(restMin, restMax);
            _pairRestUntil = Time.time + dashPillarRest;
        }

        private EPhase GetPhase(float maxHealth, float currentHealth)
        {
            var healthPercentage = currentHealth / maxHealth;

            return healthPercentage switch
            {
                <= 0.3f => EPhase.Phase3,
                <= 0.6f => EPhase.Phase2,
                _ => EPhase.Phase1
            };
        }

        private Vector2 DirTo(Vector2 to)
        {
            return (to - (Vector2)transform.position).normalized;
        }

        private static Quaternion AngleOf(Vector2 dir)
        {
            return Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        private static Vector2 Rotate(Vector2 v, float degrees)
        {
            return Quaternion.Euler(0f, 0f, degrees) * v;
        }

        private static void SpawnFx(GameObject prefab, Vector3 pos, Quaternion rot, float lifetime = 1f)
        {
            if (prefab) Destroy(Instantiate(prefab, pos, rot), lifetime);
        }

        private void Launch(GameObject prefab, Vector2 dir, float speed, float lifetime, float damage,
            GameObject hitFx, GameObject endFx, bool pierce)
        {
            var go = Instantiate(prefab, transform.position, AngleOf(dir));
            if(go.TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = dir * speed;
            
            if(go.TryGetComponent<BossProjectile>(out var projectile))
               projectile.Init(damage, pierce, lifetime, hitFx, endFx);
        }

        private void FireAura(Vector2 dir, float damage)
        {
            Launch(swordAuraPrefab, dir, swordAuraSpeed, swordAuraLifetime, damage,
                swordAuraHitSparkPrefab, swordAuraVanishPrefab, true);
        }

        private void Damage(float amount)
        {
            if (_target.TryGetComponent<HealthComponent>(out var health)) health.TakeDamage(amount);
        }

        // _facing 기준 부채꼴 안에 플레이어가 있으면 데미지
        private void TryHitFan(float range, float angle, float damage)
        {
            if (!_target) return;

            var to = (Vector2)_target.position - (Vector2)transform.position;
            if (to.magnitude > range) return;
            if (Vector2.Angle(_facing, to) > angle * 0.5f) return;
            Damage(damage);
        }
        
        /*
         * pattern 1
         * 베기 프레임에서 플레이어 방향으로 초승달 검기 발사. 속도 8m/s, 관통, 수명 2초
         * P1: 단발 / P2: 3방향 부채꼴(±25°) / P3: 8방향 회전 연사(발당 25)
         */
        private IEnumerator SwordAura()
        {
            Debug.LogFormat("{0}: {1}", nameof(EnemyBossCombatComponent), nameof(SwordAura));
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.EnemyAttack);
            
            // [Resource] 베기 애니메이션
            yield return _slashFrameWaitForSeconds;
            if (!_target) { ClearExecutingFlag(); yield break; }

            _facing = DirTo(_target.position);

            switch (_phase)
            {
                case EPhase.Phase1:
                    FireAura(_facing, swordAuraDamage);
                    break;
                case EPhase.Phase2:
                    foreach (var angle in new[] { -swordAuraFanAngle, 0f, swordAuraFanAngle })
                        FireAura(Rotate(_facing, angle), swordAuraDamage * Mult);
                    break;
                default:
                    for (var i = 0; i < swordAuraSpinCount; i++)
                    {
                        FireAura(Rotate(_facing, 360f / swordAuraSpinCount * i), swordAuraSpinDamage * Mult);
                        yield return _swordAuraSpinWaitForSeconds;
                    }
                    break;
            }

            ClearExecutingFlag();
        }
        
        /*
         * pattern 2
         * 3m 안에 플레이어가 있으면 120° 근접 판정
         */
        private IEnumerator SwingGreatSword()
        {
            Debug.LogFormat("{0}: {1}", nameof(EnemyBossCombatComponent), nameof(SwingGreatSword));
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.EnemyAttack);
            
            // [Resource] 대검 휘두르기 애니메이션, 타격 SFX
            _facing = DirTo(_target.position);
            var fan = Instantiate(fanTelegraphPrefab, transform.position, AngleOf(_facing));
            yield return _swingWaitForSeconds; 
            Destroy(fan);

            TryHitFan(swingRange, swingAngle, swingDamage * Mult);
            ClearExecutingFlag();
        }
        
        /*
         * pattern 3
         * 뼈 12개가 원형으로 퍼짐. 속도 6m/s
         * P3: 2연속 (두 번째는 각도를 반 칸 비틈)
         */
        private IEnumerator BoneExplosion()
        {
            Debug.LogFormat("{0}: {1}", nameof(EnemyBossCombatComponent), nameof(BoneExplosion));
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.SpecialAttack);
            
            // [Resource] 뼈 폭발 시전 애니메이션/SFX
            var bursts = _phase == EPhase.Phase3 ? 2 : 1;
            var step = 360f / boneCount;
            
            for (var b = 0; b < bursts; b++)
            {
                if (b > 0) yield return _boneBurstWaitForSeconds;

                SpawnFx(boneBurstRingPrefab, transform.position, Quaternion.identity);
                for (var i = 0; i < boneCount; i++)
                {
                    var dir = Rotate(Vector2.right, step * 0.5f * b + step * i);
                    Launch(bonePrefab, dir, boneSpeed, boneLifetime, boneDamage * Mult, null, boneShatterPrefab, false);
                }
            }

            ClearExecutingFlag();
        }
        
        /*
         * pattern 4
         * 붉은 직선 예고선이 1.0초 뜬 뒤 8m를 12m/s로 돌진하고 끝에서 베기. 지나간 자리에 검기 잔상
         */
        private IEnumerator DashSlash()
        {
            Debug.LogFormat("{0}: {1}", nameof(EnemyBossCombatComponent), nameof(DashSlash));
            // 방향은 예고선이 뜰 때 고정
            _facing = DirTo(_target.position);
            var rotation = AngleOf(_facing);
            var origin = (Vector2)transform.position;

            var line = Instantiate(dashLinePrefab, origin, rotation);
            var fan = Instantiate(fanTelegraphPrefab, origin + _facing * dashDistance, rotation);
            yield return _dashWaitForSeconds;
            Destroy(line);
            Destroy(fan);
            
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.EnemyAttack);

            // [Resource] 돌진 애니메이션, 돌진/베기 SFX
            // 주의: Rigidbody2D(Dynamic)를 쓰면 transform 직접 이동과 충돌하므로 Kinematic으로 두거나 MovePosition으로 교체 필요
            var travelled = 0f;
            var nextAfterimage = 0f;
            while (travelled < dashDistance)
            {
                var step = Mathf.Min(dashSpeed * Time.deltaTime, dashDistance - travelled);
                transform.position += (Vector3)(_facing * step);
                travelled += step;

                if (travelled >= nextAfterimage)
                {
                    SpawnFx(dashAfterimagePrefab, transform.position, rotation, afterimageLifetime);
                    nextAfterimage += afterimageSpacing;
                }
                yield return null;
            }

            TryHitFan(dashSlashRange, dashSlashAngle, dashSlashDamage);
            ClearExecutingFlag();
        }
        
        /*
         * pattern 5
         * 플레이어 주변 6m 안 바닥 6곳에 푸른 예고 원이 1.2초 뜬 뒤 기둥이 솟음. 반경 1.5m
         */
        private IEnumerator SoulPillar()
        {
            Debug.LogFormat("{0}: {1}", nameof(EnemyBossCombatComponent), nameof(SoulPillar));
            if (_animator)
                _animator.SetTrigger(AnimatorParamToHash.SpecialAttack);
            
            // [Resource] 시전 애니메이션, 기둥 솟는 SFX
            var positions = new Vector2[pillarCount];
            var telegraphs = new GameObject[pillarCount];

            for (var i = 0; i < pillarCount; i++)
            {
                positions[i] = (Vector2)_target.position + Random.insideUnitCircle * pillarSpawnRadius;
                telegraphs[i] = Instantiate(pillarTelegraphPrefab, positions[i], Quaternion.identity);
            }

            yield return _pillarWaitForSeconds;

            var hit = false;
            for (var i = 0; i < pillarCount; i++)
            {
                Destroy(telegraphs[i]);
                SpawnFx(pillarPrefab, positions[i], Quaternion.identity, pillarLifetime);

                if (_target && Vector2.Distance(_target.position, positions[i]) <= pillarRadius) hit = true;
            }

            // 기둥이 겹쳐도 한 번만 데미지
            if (hit) Damage(pillarDamage);
            ClearExecutingFlag();
        }
    }
}