using System;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AI;
using Scripts.Save;

namespace Scripts.Systems.Enemies
{
    public class EnemyRegistry
    {
        private readonly List<EnemyAI> _activeEnemies = new List<EnemyAI>();
        
        // Для загрузки (фабрика)
        private readonly Transform _playerTransform;
        private readonly GameObject _meleePrefab;
        private readonly GameObject _rangedPrefab;
        private readonly bool _isPeacefulMode;

        // Событие, которое сработает при регистрации ЛЮБОГО врага (спавн или загрузка)
        public event Action<EnemyAI> OnEnemyRegistered;

        public EnemyRegistry(Transform playerTransform, GameObject meleePrefab, GameObject rangedPrefab, bool isPeacefulMode)
        {
            _playerTransform = playerTransform;
            _meleePrefab = meleePrefab;
            _rangedPrefab = rangedPrefab;
            _isPeacefulMode = isPeacefulMode;
        }

        public IReadOnlyList<EnemyAI> GetActiveEnemies() => _activeEnemies;

        public void Register(EnemyAI enemy)
        {
            if (!_activeEnemies.Contains(enemy))
            {
                enemy.IsPeacefulMode = _isPeacefulMode; 

                _activeEnemies.Add(enemy);
                
                // Автоматически вычеркиваем из списка при смерти
                if (enemy.Health != null)
                {
                    enemy.Health.OnDeathEvent.AddListener(() => Unregister(enemy));
                }
                
                OnEnemyRegistered?.Invoke(enemy);
            }
        }

        public void Unregister(EnemyAI enemy)
        {
            if (_activeEnemies.Contains(enemy))
            {
                _activeEnemies.Remove(enemy);
            }
        }

        public void ClearAll()
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    GameObject.Destroy(enemy.gameObject);
                }
            }
            _activeEnemies.Clear();
        }

        // Вызывается из LoadInteractor при загрузке сохранения
        public EnemyAI CreateEnemyFromSave(EntitySaveData data)
        {
            GameObject prefab = data.entityType == "MeleeWalk" ? _meleePrefab : _rangedPrefab;
            if (prefab == null) return null;

            Vector3 pos = new Vector3(data.positionX, data.positionY, data.positionZ);
            Quaternion rot = Quaternion.Euler(0f, data.rotationY, 0f);
            
            GameObject enemyObj = GameObject.Instantiate(prefab, pos, rot);
            var ai = enemyObj.GetComponent<EnemyAI>();
            
            ai.SetSaveId(data.id);
            ai.Construct(_playerTransform);
            
            Register(ai);
            return ai;
        }
    }
}