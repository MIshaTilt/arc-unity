using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.DTO;
using Scripts.Save.Repository;
using UnityEngine;

namespace Scripts.Save.Interactor
{
    /// <summary>
    /// Интерфейс интерактора сохранения/загрузки.
    /// Паттерн Interactor: оркестрирует взаимодействие между репозиторием и игровыми моделями.
    /// </summary>
    public interface ISaveLoadInteractor
    {
        Task<bool> SaveGameAsync(GameSaveData saveData);
        Task<GameSaveData> LoadGameAsync(string saveId);
        Task<List<GameSaveData>> GetAllSavesAsync();
        Task<bool> DeleteSaveAsync(string saveId);
    }

    /// <summary>
    /// Реализация интерактора: координирует сохранение и загрузку игрового состояния.
    /// Собирает данные из IEntitySaveable компонентов, сериализует и отправляет в репозиторий.
    /// </summary>
    public class SaveLoadInteractor : ISaveLoadInteractor
    {
        private readonly IRepository<GameSaveData> _repository;
        private readonly IEnumerable<IEntitySaveable> _saveableEntities;
        private readonly IEntitySaveable _playerSaveable;

        public SaveLoadInteractor(
            IRepository<GameSaveData> repository,
            IEnumerable<IEntitySaveable> saveableEntities,
            IEntitySaveable playerSaveable)
        {
            _repository = repository;
            _saveableEntities = saveableEntities;
            _playerSaveable = playerSaveable;
        }

        /// <summary>
        /// Сохраняет полное состояние игры:
        /// 1. Собирает данные из всех IEntitySaveable (игрок + враги).
        /// 2. Формирует GameSaveData.
        /// 3. Отправляет в репозиторий (PocketBase).
        /// </summary>
        public async Task<bool> SaveGameAsync(GameSaveData saveData)
        {
            Debug.Log("[SaveLoadInteractor] Начало сохранения...");

            try
            {
                // Заполняем данные игрока
                if (_playerSaveable != null)
                {
                    var playerStateData = _playerSaveable.CaptureState();
                    saveData.playerPosition = new PlayerPositionData
                    {
                        positionX = playerStateData.positionX,
                        positionY = playerStateData.positionY,
                        positionZ = playerStateData.positionZ,
                        rotationY = playerStateData.rotationY
                    };

                    // Парсим extraData для получения HP/кулдаунов
                    saveData.playerState = JsonUtility.FromJson<PlayerStateData>(playerStateData.extraData);
                }

                // Заполняем данные врагов (пропускаем уничтоженных)
                var enemyList = new List<EnemySaveData>();
                foreach (var saveable in _saveableEntities)
                {
                    // Пропускаем уничтоженные объекты (Unity перегружает == для Object)
                    if (saveable is UnityEngine.Object unityObj && unityObj == null)
                        continue;

                    var state = saveable.CaptureState();
                    enemyList.Add(new EnemySaveData
                    {
                        id = state.id,
                        enemyType = state.entityType,
                        positionX = state.positionX,
                        positionY = state.positionY,
                        positionZ = state.positionZ,
                        rotationY = state.rotationY,
                        currentHealth = state.currentHealth,
                        maxHealth = state.maxHealth,
                        isAlive = state.isAlive
                    });
                }
                saveData.enemies = enemyList.ToArray();

                // Метаданные: id — встроенный PK PocketBase
                saveData.id = string.IsNullOrEmpty(saveData.id)
                    ? Guid.NewGuid().ToString()
                    : saveData.id;
                saveData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                saveData.sceneName = string.IsNullOrEmpty(saveData.sceneName)
                    ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                    : saveData.sceneName;

                // Отправляем в репозиторий (существует ли запись с таким id?)
                bool exists = await _repository.ExistsAsync(saveData.id);
                if (exists)
                {
                    await _repository.UpdateAsync(saveData.id, saveData);
                    Debug.Log($"[SaveLoadInteractor] Сохранение обновлено: {saveData.id}");
                }
                else
                {
                    await _repository.CreateAsync(saveData);
                    Debug.Log($"[SaveLoadInteractor] Новое сохранение создано: {saveData.id}");
                }

                Debug.Log("[SaveLoadInteractor] Сохранение завершено успешно.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadInteractor] Ошибка сохранения: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Загружает состояние игры из репозитория и восстанавливает все IEntitySaveable сущности.
        /// </summary>
        public async Task<GameSaveData> LoadGameAsync(string saveId)
        {
            Debug.Log($"[SaveLoadInteractor] Загрузка сохранения: {saveId}");

            try
            {
                GameSaveData saveData = await _repository.GetByIdAsync(saveId);
                if (saveData == null)
                {
                    Debug.LogError($"[SaveLoadInteractor] Сохранение {saveId} не найдено.");
                    return null;
                }

                // Восстанавливаем игрока
                if (_playerSaveable != null && saveData.playerPosition != null)
                {
                    var entityData = new EntitySaveData
                    {
                        id = _playerSaveable.SaveId,
                        entityType = _playerSaveable.EntityType,
                        positionX = saveData.playerPosition.positionX,
                        positionY = saveData.playerPosition.positionY,
                        positionZ = saveData.playerPosition.positionZ,
                        rotationY = saveData.playerPosition.rotationY,
                        extraData = saveData.playerState != null
                            ? JsonUtility.ToJson(saveData.playerState)
                            : ""
                    };

                    _playerSaveable.RestoreState(entityData);
                }

                // Словарь для быстрого поиска врагов по id
                var saveableDict = new Dictionary<string, IEntitySaveable>();
                foreach (var saveable in _saveableEntities)
                {
                    saveableDict[saveable.SaveId] = saveable;
                }

                // Восстанавливаем врагов
                if (saveData.enemies != null)
                {
                    foreach (var enemyData in saveData.enemies)
                    {
                        if (saveableDict.TryGetValue(enemyData.id, out IEntitySaveable saveable))
                        {
                            var entityData = new EntitySaveData
                            {
                                id = enemyData.id,
                                entityType = enemyData.enemyType,
                                positionX = enemyData.positionX,
                                positionY = enemyData.positionY,
                                positionZ = enemyData.positionZ,
                                rotationY = enemyData.rotationY,
                                currentHealth = enemyData.currentHealth,
                                maxHealth = enemyData.maxHealth,
                                isAlive = enemyData.isAlive
                            };

                            saveable.RestoreState(entityData);
                        }
                        else
                        {
                            Debug.LogWarning($"[SaveLoadInteractor] Враг {enemyData.id} не найден на сцене.");
                        }
                    }
                }

                Debug.Log("[SaveLoadInteractor] Загрузка завершена успешно.");
                return saveData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadInteractor] Ошибка загрузки: {ex.Message}");
                return null;
            }
        }

        public async Task<List<GameSaveData>> GetAllSavesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> DeleteSaveAsync(string saveId)
        {
            return await _repository.DeleteAsync(saveId);
        }
    }
}
