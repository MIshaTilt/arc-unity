using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using UnityEngine;

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

        private readonly IEnumerable<IEntitySaveable> _saveableEntities;
        private readonly IPlayerSaveable _playerSaveable;

        public LoadInteractor(
            IGameMetaRepository metaRepository,
            IPlayerRepository playerRepository,
            IEnemyRepository enemyRepository,
            IEnumerable<IEntitySaveable> saveableEntities,
            IPlayerSaveable playerSaveable)
        {
            _metaRepository = metaRepository;
            _playerRepository = playerRepository;
            _enemyRepository = enemyRepository;
            _saveableEntities = saveableEntities;
            _playerSaveable = playerSaveable;
        }

        public async Task<LoadGameResponse> ExecuteAsync(LoadGameRequest request)
        {
            try
            {
                // Проверяем существование сохранения через мета-репозиторий
                string sceneName = await _metaRepository.LoadSceneNameAsync(request.SaveId);
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
                    var saveableDict = new Dictionary<string, IEntitySaveable>();
                    foreach (var saveable in _saveableEntities)
                        saveableDict[saveable.SaveId] = saveable;

                    foreach (var enemyState in enemiesData)
                    {
                        if (saveableDict.TryGetValue(enemyState.id, out IEntitySaveable saveable))
                        {
                            saveable.RestoreState(enemyState);
                        }
                    }
                }

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
