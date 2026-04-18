using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        private readonly IEnumerable<IEntitySaveable> _saveableEntities;
        private readonly IPlayerSaveable _playerSaveable;

        public SaveInteractor(
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

        public async Task<SaveGameResponse> ExecuteAsync(SaveGameRequest request)
        {
            try
            {
                // 1. Сохраняем мету (Сцену)
                string sceneName = SceneManager.GetActiveScene().name;
                await _metaRepository.SaveMetaAsync(request.SaveId, sceneName);

                // 2. Сохраняем игрока
                if (_playerSaveable != null)
                {
                    var playerPos = _playerSaveable.CapturePosition();
                    var playerState = _playerSaveable.CaptureState();
                    await _playerRepository.SavePlayerAsync(request.SaveId, playerPos, playerState);
                }

                // 3. Сохраняем врагов
                var enemiesData = new List<EntitySaveData>();
                foreach (var saveable in _saveableEntities)
                {
                    if (saveable is UnityEngine.Object unityObj && unityObj != null)
                    {
                        enemiesData.Add(saveable.CaptureState());
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
