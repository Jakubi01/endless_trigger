using System.Collections.Generic;
using Character.Enemy;
using Character.Player;
using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        /// <summary>
        /// 특정 타이밍에 스폰할 적들의 마리 수 및 종류별 확률 가중치 데이터
        /// </summary>
        private struct SpawnRate
        {
            public int SpawnCount;
            public float ZombieWeight;
            public float RusherWeight;
            public float TankerWeight;
        }

        /// <summary>
        /// enemy 종류별 원본 데이터와 해당 풀을 묶어 관리
        /// </summary>
        private sealed class EnemyPool
        {
            public EnemyData Data;
            public IObjectPool<EnemyCharacterBase> Pool;
        }

        [Header("Spawn Settings")]
        [SerializeField] private EnemyData[] enemyData;                   // 스폰 가능한 적들의 ScriptableObject 데이터 배열
        [SerializeField] private float minSpawnDistanceFromPlayer = 5f;   // 플레이어 기준 최소 스폰 거리
        [SerializeField] private float maxSpawnDistanceFromPlayer = 9f;   // 플레이어 기준 최대 스폰 거리
        [SerializeField] private float initialSpawnDelay = 5f;            // 게임 시작 후 첫 스폰까지의 대기 시간
        [SerializeField] private float waveDuration = 30f;                // 한 웨이브의 지속 시간
        [SerializeField] private int baseSpawnCount = 2;                  // 게임 시작 시 기본 스폰 마리 수
        [SerializeField] private int spawnCountIncreasePerSection = 2;    // 한 웨이브 내 섹션 진행 시 증가할 스폰 마리 수
        [SerializeField] private int spawnCountIncreasePerWave = 1;       // 웨이브 단계 상승 시 증가할 기본 스폰 마리 수
        [SerializeField] private float minimumZombieRatio = 0.05f;        // 난이도가 극에 달해도 유지될 최소 좀비 비율
        [SerializeField] private float maximumTankerRatio = 0.75f;        // 탱커가 가질 수 있는 최대 확률 제한
         
        [Header("Pool Settings")] 
        [SerializeField] private int defaultPoolCapacity = 32;            // 풀 생성 시 내부 배열의 기본 용량
        [SerializeField] private int maxPoolSize = 256;                   // 풀이 가질 수 있는 최대 오브젝트 수
        [SerializeField] private int prewarmCountPerEnemy = 8;            // 게임 시작 시 미리 생성해 둘 적의 수

        private const float SpawnInterval = 5f;                           // 스폰 발생 주기

        private readonly Dictionary<EnemyType, EnemyPool> _pools = new(); // 타입별 풀 빠른 탐색용 딕셔너리
        private readonly List<EnemyPool> _poolList = new();               // 일괄 순회 및 메모리 해제용 풀 리스트
        private readonly List<EnemyCharacterBase> _prewarmBuffer = new(); // 프리웜 도중 일시적으로 객체들을 담아둘 버퍼 리스트
        private PlayerCharacter _player;

        public PlayerCharacter Player
        {
            get => _player;
            set => _player = value;
        }
        
        private float _elapsedTime;         // 게임 시작 후 흐른 총 시간
        private float _nextSpawnTime;       // 다음 스폰이 일어날 타겟 시간값
        private int _completedWaveCount;    // 현재까지 클리어 처리된 총 웨이브 수

        // --- 외부 UI 나 매니저 시스템에서 정보 확인용으로 사용할 프로퍼티들 ---
        public float TimeUntilGameStart => Mathf.Max(0f, initialSpawnDelay - _elapsedTime);
        
        /// <summary>
        /// 현재 진행 중인 웨이브의 남은 시간을 계산하여 반환
        /// </summary>
        public float CurrentWaveRemainingTime
        {
            get
            {
                float activeTime = Mathf.Max(0f, _elapsedTime - initialSpawnDelay);
                float timeInWave = activeTime % waveDuration;
                return Mathf.Max(0f, waveDuration - timeInWave);
            }
        }

        private void Awake()
        {
            // 최대 거리가 최소 거리보다 작다면 최소 거리와 같게 맞춰줌
            if (maxSpawnDistanceFromPlayer < minSpawnDistanceFromPlayer)
            {
                maxSpawnDistanceFromPlayer = minSpawnDistanceFromPlayer;
            }

            initialSpawnDelay = Mathf.Max(0f, initialSpawnDelay);
            waveDuration = Mathf.Max(0.1f, waveDuration);
            
            // 첫 번째 스폰 타이밍 설정
            _nextSpawnTime = initialSpawnDelay;
            
            // 오브젝트 풀 초기화 및 세팅
            BuildPools();
        }

        private void OnDestroy()
        {
            // 스포너가 파괴될 때 풀 내부의 인스턴스들을 모두 제거하여 메모리 누수 방지
            for (int i = 0, count = _poolList.Count; i < count; i++)
            {
                EnemyPool enemyPool = _poolList[i];
                enemyPool.Pool?.Clear();
            }

            _pools.Clear();
            _poolList.Clear();
            _prewarmBuffer.Clear();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            
            // 웨이브가 끝났는지 체크 및 갱신
            UpdateWaveClearedCount();
            
            // 아직 스폰 타이밍이 되지 않았다면 리턴
            if (_elapsedTime < _nextSpawnTime) return;

            // 딜레이 시간을 뺀 스폰 구동 시간을 구해 스폰 로직 실행
            float activeSpawnTime = Mathf.Max(0f, _nextSpawnTime - initialSpawnDelay);
            SpawnWaveSection(activeSpawnTime);
            
            // 다음 스폰 목표 시간 갱신
            _nextSpawnTime += SpawnInterval;
        }

        /// <summary>
        /// 흐른 시간을 지속적으로 체크해 웨이브 시간이 만료될 때마다 GameManager에 클리어를 알림
        /// </summary>
        private void UpdateWaveClearedCount()
        {
            float activeTime = _elapsedTime - initialSpawnDelay;
            if (activeTime < waveDuration) return;

            // 현재 도달해야 하는 이론적인 클리어 웨이브 수 계산
            int completedWaveCount = Mathf.FloorToInt(activeTime / waveDuration);
            
            // 실제 반영된 카운트가 이에 못 미치면 추적해가며 매니저에 등록
            while (_completedWaveCount < completedWaveCount)
            {
                _completedWaveCount++;
                GameManager.Instance?.RegisterWaveCleared();
            }
        }

        /// <summary>
        /// 인스펙터에 할당된 EnemyData를 기반으로 유니티 ObjectPool을 동적 생성하고 초기 객체를 채워둠.
        /// </summary>
        private void BuildPools()
        {
            _pools.Clear();
            _poolList.Clear();
            
            if (enemyData == null || enemyData.Length == 0)
            {
                Debug.LogError($"{nameof(EnemySpawner)}: EnemyData list is empty.", this);
                return;
            }

            for (int i = 0, count = enemyData.Length; i < count; i++)
            {
                EnemyData data = enemyData[i];
                if (!data || !data.Prefab)
                {
                    Debug.LogError($"{nameof(EnemySpawner)}: EnemyData or prefab is not assigned.", this);
                    continue;
                }

                if (_pools.ContainsKey(data.EnemyType))
                {
                    Debug.LogError($"{nameof(EnemySpawner)}: Duplicate EnemyData for {data.EnemyType}.", this);
                    continue;
                }

                EnemyPool enemyPool = new EnemyPool { Data = data };
                
                enemyPool.Pool = new ObjectPool<EnemyCharacterBase>(
                    createFunc: () => CreateEnemy(enemyPool),                    // 풀에 여분이 없을 때 새로 생성할 함수
                    actionOnGet: enemy => enemy.gameObject.SetActive(true),      // 풀에서 꺼낼 때
                    actionOnRelease: enemy => enemy.gameObject.SetActive(false), // 풀로 반환할 때
                    actionOnDestroy: enemy => Destroy(enemy.gameObject),         // 풀 공간 초과 등으로 파괴할 때
                    collectionCheck: true,                                       // 이미 반환된 오브젝트를 또 반환하는지 검사 여부
                    defaultCapacity: defaultPoolCapacity,
                    maxSize: maxPoolSize
                );

                _pools.Add(data.EnemyType, enemyPool);
                _poolList.Add(enemyPool);
                
                // 씬 시작 전 최적화를 위해 더미 객체 미리 생성 후 반환 프로세스 진행
                Prewarm(enemyPool);
            }
        }

        /// <summary>
        /// 풀 내부에서 사용될 실제 프리팹 인스턴스를 인스턴스화
        /// </summary>
        private EnemyCharacterBase CreateEnemy(EnemyPool enemyPool)
        {
            EnemyCharacterBase enemy = Instantiate(enemyPool.Data.Prefab, transform);
            enemy.gameObject.SetActive(false);
            
            // 적 캐릭터가 스스로 죽었을 때 자신을 풀로 돌려보낼 수 있도록 Action 디지게이트 연결
            enemy.SetPoolReleaseAction(enemyPool.Pool.Release);
            return enemy;
        }

        /// <summary>
        /// 설정된 개수(prewarmCountPerEnemy)만큼 오브젝트를 풀에서 강제로 꺼냈다가 집어넣어, 런타임 중 끊김 현상을 방지
        /// </summary>
        private void Prewarm(EnemyPool enemyPool)
        {
            _prewarmBuffer.Clear();
            
            // 지정된 개수만큼 꺼내서 버퍼에 보관 (이때 CreateEnemy가 호출되면서 메모리에 인스턴스 적재됨)
            for (int i = 0; i < prewarmCountPerEnemy; i++)
            {
                _prewarmBuffer.Add(enemyPool.Pool.Get());
            }

            // 꺼내온 오브젝트들을 다시 풀 내부로 원상 복귀 (비활성화 상태로 풀 대기조 안착)
            for (int i = 0, count = _prewarmBuffer.Count; i < count; i++)
            {
                EnemyCharacterBase enemy = _prewarmBuffer[i];
                enemyPool.Pool.Release(enemy);
            }
        }

        /// <summary>
        /// 특정 스폰 시간대에 맞춰 계산된 횟수만큼 적을 무작위로 추첨해 스폰
        /// </summary>
        private void SpawnWaveSection(float spawnTime)
        {
            SpawnRate rate = GetSpawnRate(spawnTime);
            for (int i = 0; i < rate.SpawnCount; i++)
            {
                SpawnEnemy(PickEnemyType(rate), spawnTime);
            }
        }

        /// <summary>
        /// 진행된 시간에 맞추어 이번 섹션의 [총 스폰 마리 수]와 [종류별 등장 가중치]를 도출
        /// </summary>
        private SpawnRate GetSpawnRate(float spawnTime)
        {
            int waveNumber = Mathf.FloorToInt(spawnTime / waveDuration) + 1;
            float timeInWave = spawnTime % waveDuration;
            int sectionIndex = Mathf.Clamp(Mathf.FloorToInt(timeInWave / SpawnInterval), 0, 5);
            float sectionProgress = sectionIndex / 5f;
            float difficulty = waveNumber - 1 + sectionProgress;

            float zombieRatio = Mathf.Clamp(1f - difficulty * 0.18f, minimumZombieRatio, 1f);
            float tankerRatio = Mathf.Clamp((difficulty - 1f) * 0.1f, 0f, maximumTankerRatio);
            float rusherRatio = Mathf.Max(0f, 1f - zombieRatio - tankerRatio);

            if (zombieRatio + rusherRatio + tankerRatio > 1f)
            {
                float overflow = zombieRatio + rusherRatio + tankerRatio - 1f;
                zombieRatio = Mathf.Max(minimumZombieRatio, zombieRatio - overflow);
            }

            return new SpawnRate
            {
                SpawnCount = GetSpawnCount(waveNumber, sectionIndex),
                ZombieWeight = zombieRatio,
                RusherWeight = rusherRatio,
                TankerWeight = tankerRatio
            };
        }

        /// <summary>
        /// 룰렛 휠 가중치 알고리즘을 이용해 비율 데이터(SpawnRate)로부터 하나의 적 종류를 무작위 선택
        /// </summary>
        private EnemyType PickEnemyType(SpawnRate rate)
        {
            float totalWeight = rate.ZombieWeight + rate.RusherWeight + rate.TankerWeight;
            if (totalWeight <= 0f) return EnemyType.Zombie;

            float roll = Random.Range(0f, totalWeight);
            if (roll < rate.ZombieWeight) return EnemyType.Zombie;

            roll -= rate.ZombieWeight;
            return roll < rate.RusherWeight ? EnemyType.Rusher : EnemyType.Tanker;
        }

        /// <summary>
        /// 웨이브 단계 및 현재 구간 보너스를 더해 스폰할 적의 마리 수를 반환
        /// </summary>
        private int GetSpawnCount(int waveNumber, int sectionIndex)
        {
            int waveBonus = Mathf.Max(0, waveNumber - 1) * spawnCountIncreasePerWave;
            int sectionBonus = sectionIndex * spawnCountIncreasePerSection;
            return Mathf.Max(1, baseSpawnCount + waveBonus + sectionBonus);
        }

        /// <summary>
        /// 오브젝트 풀에서 적을 하나 활성화한 후, 플레이어 주변에 배치하고 현재 웨이브 단계에 맞춰 스탯을 최종 부여(스케일링)
        /// </summary>
        private void SpawnEnemy(EnemyType type, float spawnTime)
        {
            if (!_pools.TryGetValue(type, out EnemyPool enemyPool))
            {
                Debug.LogError($"{nameof(EnemySpawner)}: EnemyData for {type} is not assigned.", this);
                return;
            }

            // 풀에서 오브젝트 하나 수령 및 초기 위치 세팅
            EnemyCharacterBase enemy = enemyPool.Pool.Get();
            enemy.transform.position = GetSpawnPosition();
            enemy.transform.rotation = Quaternion.identity;

            // 현재 웨이브 단계를 기준으로 ScriptableObject 내부에서 스탯 배율을 계산해 가져옴
            int waveNumber = Mathf.FloorToInt(spawnTime / waveDuration) + 1;
            enemyPool.Data.GetScaledStats(waveNumber, out float hp, out float speed, out float damage, out int experience);
            
            // 적 실체 캐릭터 컴포넌트에 최종 스탯 및 타입 주입하여 활성화 완료
            enemy.Initialize(type, hp, speed, damage, experience);
        }

        /// <summary>
        /// 플레이어 중심의 미니멈/맥시멈 사이 반지름을 가지는 2D 도넛 원형 영역 안에서 무작위 월드 좌표 반환
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            // 반지름이 1인 원 내부의 임의의 점 벡터를 뽑고 노멀라이즈하여 무작위 방향 점 벡터 추출
            Vector2 direction = Random.insideUnitCircle.normalized;
            
            // 혹시라도 (0,0)이 뽑혀 노멀라이즈가 깨졌을 때를 대비한 예외 처리
            if (direction.sqrMagnitude < 0.01f)
            {
                direction = Vector2.right;
            }

            // 최소~최대 거리 사이의 임의의 스폰 거리 계산
            float distance = Random.Range(minSpawnDistanceFromPlayer, maxSpawnDistanceFromPlayer);
            
            // 플레이어의 현재 평면 위치에 방향 * 거리를 더해 최종 Vector3 월드 좌표 연산
            return _player.transform.position + (Vector3)(direction * distance);
        }
    }
}