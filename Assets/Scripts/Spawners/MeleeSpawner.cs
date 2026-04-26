using UnityEngine;

namespace Scripts.Spawners
{
    public class MeleeSpawner : EnemySpawner
    {
        protected override void SpawnEnemy()
        {
            Vector3 pos = GetRandomSpawnPoint();
            GameObject enemyObj = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            
            var ai = enemyObj.GetComponent<AI.MeleeWalk>();
            ai.Construct(_playerTransform); // Передаем цель (как в GameBootstrapper)
        }
    }
}