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
        private readonly IGameSaveRepository _repository;
        private readonly IEnumerable<IEntitySaveable> _saveableEntities;
        private readonly IPlayerSaveable _playerSaveable;

        public LoadInteractor(
            IGameSaveRepository repository,
            IEnumerable<IEntitySaveable> saveableEntities,
            IPlayerSaveable playerSaveable)
        {
            _repository = repository;
            _saveableEntities = saveableEntities;
            _playerSaveable = playerSaveable;
        }

        public async Task<LoadGameResponse> ExecuteAsync(LoadGameRequest request)
        {
            try
            {
                GameStateSnapshot snapshot = await _repository.LoadStateAsync(request.SaveId);
                
                if (snapshot == null) 
                    return new LoadGameResponse { Success = false, Message = "Сохранение не найдено" };

                if (_playerSaveable != null)
                {
                    _playerSaveable.RestoreState(snapshot.PlayerPosition, snapshot.PlayerState);
                }

                var saveableDict = new Dictionary<string, IEntitySaveable>();
                foreach (var saveable in _saveableEntities) 
                    saveableDict[saveable.SaveId] = saveable;

                foreach (var enemyState in snapshot.Enemies)
                {
                    if (saveableDict.TryGetValue(enemyState.id, out IEntitySaveable saveable))
                    {
                        saveable.RestoreState(enemyState);
                    }
                }

                // Возвращаем объект Response
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