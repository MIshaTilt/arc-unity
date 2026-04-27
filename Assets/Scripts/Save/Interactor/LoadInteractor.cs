using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using UnityEngine;
using Scripts.Systems.Score;

namespace Scripts.Save.Interactor
{
    public interface ILoadInteractor
    {
        Task<LoadGameResponse> ExecuteAsync(LoadGameRequest request);
    }

    public class LoadInteractor : ILoadInteractor
    {
        private readonly IGameMetaRepository _metaRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IEnemyRepository _enemyRepository;

        private readonly Systems.Enemies.EnemyRegistry _enemyRegistry;
        private readonly IPlayerSaveable _playerSaveable;
        private readonly ScoreSystem _scoreSystem;

        public LoadInteractor(
            IGameMetaRepository metaRepository,
            IPlayerRepository playerRepository,
            IEnemyRepository enemyRepository,
            Systems.Enemies.EnemyRegistry enemyRegistry,
            IPlayerSaveable playerSaveable,
            ScoreSystem scoreSystem)
        {
            _metaRepository = metaRepository;
            _playerRepository = playerRepository;
            _enemyRepository = enemyRepository;
            _enemyRegistry = enemyRegistry;
            _playerSaveable = playerSaveable;
            _scoreSystem = scoreSystem;
        }

        public async Task<LoadGameResponse> ExecuteAsync(LoadGameRequest request)
        {
            try
            {
                // Проверяем существование сохранения через мета-репозиторий
                var (sceneName, killCount) = await _metaRepository.LoadMetaAsync(request.SaveId);
                if (string.IsNullOrEmpty(sceneName))
                    return new LoadGameResponse { Success = false, Message = "Сохранение не найдено" };

                // 1. Восстанавливаем игрока
                if (_playerSaveable != null)
                {
                    var (pos, state) = await _playerRepository.LoadPlayerAsync(request.SaveId);
                    if (pos != null && state != null)
                        _playerSaveable.RestoreState(pos, state);
                }

                // 2. Восстанавливаем врагов
                var enemiesData = await _enemyRepository.LoadEnemiesAsync(request.SaveId);
                if (enemiesData != null)
                {
                    // 1. Сначала уничтожаем всех живых врагов на сцене (чтобы не было дубликатов)
                    _enemyRegistry.ClearAll();

                    // 2. Спавним тех, кто был в сохранении
                    foreach (var enemyState in enemiesData)
                    {
                        if (enemyState.isAlive)
                        {
                            var enemy = _enemyRegistry.CreateEnemyFromSave(enemyState);
                            if (enemy != null)
                            {
                                enemy.RestoreState(enemyState);
                            }
                        }
                    }
                }

                _scoreSystem.LoadScore(killCount);

                return new LoadGameResponse { Success = true, Message = "Игра успешно загружена" };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LoadInteractor] Ошибка загрузки: {ex.Message}");
                return new LoadGameResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
