using UnityEngine;

namespace Scripts.Spawners
{
    public class RangedSpawner : EnemySpawner
    {
        protected override void SpawnEnemy()
        {
            if (Target == null) return;

            Vector3 pos = GetRandomSpawnPoint();
            GameObject enemyObj = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            
            var ai = enemyObj.GetComponent<AI.RangedWalk>();
            ai.Construct(Target);
            
            TriggerSpawnEvent(ai);
        }
    }
}