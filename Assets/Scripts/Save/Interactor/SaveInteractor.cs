using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Systems.Score;

namespace Scripts.Save.Interactor
{
    public interface ISaveInteractor
    {
        Task<SaveGameResponse> ExecuteAsync(SaveGameRequest request);
    }

    public class SaveInteractor : ISaveInteractor
    {
        private readonly IGameMetaRepository _metaRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IEnemyRepository _enemyRepository;
        private readonly Systems.Enemies.EnemyRegistry _enemyRegistry;
        private readonly IPlayerSaveable _playerSaveable;
        private readonly ScoreSystem _scoreSystem;

        public SaveInteractor(
            IGameMetaRepository metaRepository,
            IPlayerRepository playerRepository,
            IEnemyRepository enemyRepository,
            Systems.Enemies.EnemyRegistry enemyRegistry,
            IPlayerSaveable playerSaveable,
            ScoreSystem scoreSystem
            )
        {
            _metaRepository = metaRepository;
            _playerRepository = playerRepository;
            _enemyRepository = enemyRepository;
            _enemyRegistry = enemyRegistry;
            _playerSaveable = playerSaveable;
            _scoreSystem = scoreSystem;
        }

        public async Task<SaveGameResponse> ExecuteAsync(SaveGameRequest request)
        {
            try
            {
                // 1. Сохраняем мету (Сцену)
                string sceneName = SceneManager.GetActiveScene().name;
                await _metaRepository.SaveMetaAsync(request.SaveId, sceneName, _scoreSystem.KillCount);

                // 2. Сохраняем игрока
                if (_playerSaveable != null)
                {
                    var playerPos = _playerSaveable.CapturePosition();
                    var playerState = _playerSaveable.CaptureState();
                    await _playerRepository.SavePlayerAsync(request.SaveId, playerPos, playerState);
                }

                // 3. Сохраняем врагов
                var enemiesData = new List<EntitySaveData>();
                foreach (var enemy in _enemyRegistry.GetActiveEnemies())
                {
                    if (enemy != null && enemy.gameObject.activeInHierarchy)
                    {
                        enemiesData.Add(enemy.CaptureState());
                    }
                }
                await _enemyRepository.SaveEnemiesAsync(request.SaveId, enemiesData);

                return new SaveGameResponse { Success = true, Message = "Игра успешно сохранена" };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveInteractor] Ошибка сохранения: {ex.Message}");
                return new SaveGameResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
