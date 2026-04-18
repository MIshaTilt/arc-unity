using System.Collections.Generic;
using Scripts.Save.DTO;

namespace Scripts.Save.Domain
{
    /// <summary>
    /// Чистый доменный объект состояния игры. 
    /// Ничего не знает о том, как данные будут лежать в JSON или БД.
    /// </summary>
    public class GameStateSnapshot
    {
        public string SaveId;
        public string SceneName;
        public PlayerPositionData PlayerPosition;
        public PlayerStateData PlayerState;
        public List<EntitySaveData> Enemies = new List<EntitySaveData>();
    }

    /// <summary>
    /// Интерфейс репозитория, который оперирует доменными объектами.
    /// </summary>
    public interface IGameSaveRepository
    {
        System.Threading.Tasks.Task<bool> SaveStateAsync(GameStateSnapshot snapshot);
        System.Threading.Tasks.Task<GameStateSnapshot> LoadStateAsync(string saveId);
    }
}