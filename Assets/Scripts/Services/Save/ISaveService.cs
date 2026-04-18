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
        Task<bool> LoadGameAsync(string saveId);

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
        // Теперь сервис зависит от двух разных сценариев
        private readonly Save.Interactor.ISaveInteractor _saveInteractor;
        private readonly Save.Interactor.ILoadInteractor _loadInteractor;
        
        private string _defaultSaveId = "savequicksave01";

        public event Action<bool> OnSaveCompleted;
        public event Action<bool> OnLoadCompleted;

        public PocketBaseSaveService(
            Save.Interactor.ISaveInteractor saveInteractor, 
            Save.Interactor.ILoadInteractor loadInteractor)
        {
            _saveInteractor = saveInteractor;
            _loadInteractor = loadInteractor;
        }

        public async Task<bool> SaveGameAsync()
        {
            Debug.Log("[PocketBaseSaveService] Инициируем сохранение...");
            
            var request = new Save.Interactor.SaveGameRequest { SaveId = _defaultSaveId };
            var response = await _saveInteractor.ExecuteAsync(request);
            
            Debug.Log($"[PocketBaseSaveService] Результат: {response.Message}");
            
            OnSaveCompleted?.Invoke(response.Success);
            return response.Success;
        }


        public async Task<bool> LoadGameAsync(string saveId)
        {
            Debug.Log($"[PocketBaseSaveService] Инициируем загрузку: {saveId}");
            
            var request = new Save.Interactor.LoadGameRequest { SaveId = saveId };
            var response = await _loadInteractor.ExecuteAsync(request);
            
            Debug.Log($"[PocketBaseSaveService] Результат: {response.Message}");
            
            OnLoadCompleted?.Invoke(response.Success);
            return response.Success;
        }


        public string GetLastSaveId() => _defaultSaveId;
        public void SetDefaultSaveId(string saveId) => _defaultSaveId = saveId;
    }


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

        // ВАЖНО: Изменился тип возвращаемого значения с Task<GameSaveData> на Task<bool>
        public Task<bool> LoadGameAsync(string saveId)
        {
            Debug.Log($"[PlayerPrefsSaveService] Загрузка (заглушка): {saveId}");
            OnLoadCompleted?.Invoke(false);
            return Task.FromResult(false); // Возвращаем false (означает "не загружено")
        }

        public string GetLastSaveId() => "quicksave";
        public void SetDefaultSaveId(string saveId) { }
    }


}