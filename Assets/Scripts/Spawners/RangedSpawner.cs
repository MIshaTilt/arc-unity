using UnityEngine;

namespace Scripts.Spawners
{
    public class RangedSpawner : EnemySpawner
    {
        protected override void SpawnEnemy()
        {
            Vector3 pos = GetRandomSpawnPoint();
            GameObject enemyObj = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            
            var ai = enemyObj.GetComponent<AI.RangedWalk>();
            ai.Construct(_playerTransform);
        }
    }
}