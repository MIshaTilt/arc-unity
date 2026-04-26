using System;
using System.Threading.Tasks;
using Scripts.Save.Domain;

namespace Scripts.Save.Repository
{
    [Serializable]
    public class GameMetaDbData 
    { 
        public string id; 
        public string sceneName; 
        public string timestamp; 
        public int killCount; // НОВОЕ ПОЛЕ
    }

    public class PocketBaseMetaRepository : IGameMetaRepository
    {
        private readonly PocketBaseRepository<GameMetaDbData> _api;

        public PocketBaseMetaRepository(PocketBaseConfig config) 
        {
            config.SavesCollection = "game_meta"; 
            _api = new PocketBaseRepository<GameMetaDbData>(config);
        }

        public async Task<bool> SaveMetaAsync(string saveId, string sceneName, int killCount)
        {
            var data = new GameMetaDbData { 
                id = saveId, 
                sceneName = sceneName, 
                killCount = killCount, // ЗАПИСЫВАЕМ СЧЕТ
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") 
            };
            if (await _api.ExistsAsync(saveId)) await _api.UpdateAsync(saveId, data);
            else await _api.CreateAsync(data);
            return true;
        }

        // Изменили тип возвращаемого значения
        public async Task<(string sceneName, int killCount)> LoadMetaAsync(string saveId)
        {
            var data = await _api.GetByIdAsync(saveId);
            if (data == null) return (null, 0);
            return (data.sceneName, data.killCount);
        }
    }
}