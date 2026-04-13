using System;
using System.Threading.Tasks;
using Scripts.Save.DTO;
using UnityEngine;

namespace Scripts.Services
{
    /// <summary>
    /// Сервис сохранения/загрузки игры.
    /// Работает через слой Interactor → Repository → PocketBase.
    /// </summary>
    public interface ISaveService
    {
        /// <summary>
        /// Сохранить текущее состояние игры.
        /// </summary>
        Task<bool> SaveGameAsync();

        /// <summary>
        /// Загрузить указанное сохранение.
        /// </summary>
        Task<GameSaveData> LoadGameAsync(string saveId);

        /// <summary>
        /// Получить ID последнего сохранения (для быстрой загрузки).
        /// </summary>
        string GetLastSaveId();

        /// <summary>
        /// Установить ID сохранения по умолчанию (используется при Save/Load без параметров).
        /// </summary>
        void SetDefaultSaveId(string saveId);

        /// <summary>
        /// Событие: сохранение завершено.
        /// </summary>
        event Action<bool> OnSaveCompleted;

        /// <summary>
        /// Событие: загрузка завершена.
        /// </summary>
        event Action<bool> OnLoadCompleted;
    }

    /// <summary>
    /// Реализация сервиса сохранения через PocketBase.
    /// </summary>
    public class PocketBaseSaveService : ISaveService
    {
        private readonly Save.Interactor.ISaveLoadInteractor _interactor;
        // PocketBase: id = min 15, max 20 символов, только [a-z0-9-]
        private string _defaultSaveId = "save-quicksave-01";

        public event Action<bool> OnSaveCompleted;
        public event Action<bool> OnLoadCompleted;

        public PocketBaseSaveService(Save.Interactor.ISaveLoadInteractor interactor)
        {
            _interactor = interactor;
        }

        public async Task<bool> SaveGameAsync()
        {
            Debug.Log("[PocketBaseSaveService] Сохранение игры...");
            var saveData = new GameSaveData { id = _defaultSaveId };
            bool result = await _interactor.SaveGameAsync(saveData);
            OnSaveCompleted?.Invoke(result);
            return result;
        }

        public async Task<GameSaveData> LoadGameAsync(string saveId)
        {
            Debug.Log($"[PocketBaseSaveService] Загрузка игры: {saveId}");
            GameSaveData result = await _interactor.LoadGameAsync(saveId);
            OnLoadCompleted?.Invoke(result != null);
            return result;
        }

        public string GetLastSaveId() => _defaultSaveId;
        public void SetDefaultSaveId(string saveId) => _defaultSaveId = saveId;
    }

    /// <summary>
    /// Заглушка для тестирования без сервера.
    /// </summary>
    public class PlayerPrefsSaveService : ISaveService
    {
        public event Action<bool> OnSaveCompleted;
        public event Action<bool> OnLoadCompleted;

        public Task<bool> SaveGameAsync()
        {
            Debug.Log("[PlayerPrefsSaveService] Сохранение (заглушка).");
            OnSaveCompleted?.Invoke(true);
            return Task.FromResult(true);
        }

        public Task<GameSaveData> LoadGameAsync(string saveId)
        {
            Debug.Log($"[PlayerPrefsSaveService] Загрузка (заглушка): {saveId}");
            OnLoadCompleted?.Invoke(false);
            return Task.FromResult<GameSaveData>(null);
        }

        public string GetLastSaveId() => "quicksave";
        public void SetDefaultSaveId(string saveId) { }
    }
}