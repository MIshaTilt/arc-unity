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
        private readonly IGameSaveRepository _repository;
        private readonly IEnumerable<IEntitySaveable> _saveableEntities;
        private readonly IPlayerSaveable _playerSaveable;

        public SaveInteractor(
            IGameSaveRepository repository,
            IEnumerable<IEntitySaveable> saveableEntities,
            IPlayerSaveable playerSaveable)
        {
            _repository = repository;
            _saveableEntities = saveableEntities;
            _playerSaveable = playerSaveable;
        }

        public async Task<SaveGameResponse> ExecuteAsync(SaveGameRequest request)
        {
            try
            {
                var snapshot = new GameStateSnapshot
                {
                    SaveId = request.SaveId, // Данные берем из объекта Request
                    SceneName = SceneManager.GetActiveScene().name
                };

                if (_playerSaveable != null)
                {
                    snapshot.PlayerPosition = _playerSaveable.CapturePosition();
                    snapshot.PlayerState = _playerSaveable.CaptureState();
                }

                foreach (var saveable in _saveableEntities)
                {
                    if (saveable is UnityEngine.Object unityObj && unityObj != null)
                    {
                        snapshot.Enemies.Add(saveable.CaptureState());
                    }
                }

                bool result = await _repository.SaveStateAsync(snapshot);
                
                // Возвращаем объект Response (как в лекции)
                return new SaveGameResponse 
                { 
                    Success = result, 
                    Message = result ? "Игра успешно сохранена" : "Ошибка записи в репозиторий" 
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveInteractor] Ошибка сохранения: {ex.Message}");
                return new SaveGameResponse { Success = false, Message = ex.Message };
            }
        }
    }
}