using UnityEngine;

namespace Character.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Endless Trigger/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private EnemyCharacterBase prefab;
        [SerializeField] private float health = 10f;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float contactDamage = 5f;
        [SerializeField] private int experienceReward = 10;
        [SerializeField] private float healthGrowthPerWave = 0.15f;
        [SerializeField] private float moveSpeedGrowthPerWave = 0.02f;
        [SerializeField] private float damageGrowthPerWave = 0.15f;

        public EnemyType EnemyType => enemyType;
        public EnemyCharacterBase Prefab => prefab;

        public void GetScaledStats(int waveNumber, out float scaledHealth, out float scaledMoveSpeed, out float scaledDamage, out int scaledExperienceReward)
        {
            var completedWaveCount = Mathf.Max(0, waveNumber - 1);
            scaledHealth = health * (1f + completedWaveCount * healthGrowthPerWave);
            scaledMoveSpeed = moveSpeed * (1f + completedWaveCount * moveSpeedGrowthPerWave);
            scaledDamage = contactDamage * (1f + completedWaveCount * damageGrowthPerWave);
            scaledExperienceReward = experienceReward + Mathf.RoundToInt(completedWaveCount * 1.2f);
        }
    }
}
