using System.Collections;
using UnityEngine;

namespace Scripts.Spawners
{
    public abstract class EnemySpawner : MonoBehaviour
    {
        [SerializeField] protected GameObject _enemyPrefab;
        [SerializeField] protected float _spawnRadius = 5f;
        [SerializeField] protected Transform _playerTransform; // Кому назначать Target
        
        [Header("Spawn Settings")]
        [SerializeField] protected float _spawnInterval = 15f; // Задержка между спавнами

        private void Start()
        {
            // Запускаем бесконечный цикл спавна
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                // Спавним врага
                SpawnEnemy();
                
                // Ждем указанное время (15 секунд) перед следующим спавном
                // WaitForSeconds реагирует на Time.timeScale, так что при паузе игры спавн тоже замрет
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        // Тот самый фабричный метод
        protected abstract void SpawnEnemy();

        protected Vector3 GetRandomSpawnPoint()
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            return spawnPos;
        }

        // Рисуем радиус в редакторе Unity для удобства
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        }
    }
}