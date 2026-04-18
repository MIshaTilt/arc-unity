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
    }

    public class PocketBaseMetaRepository : IGameMetaRepository
    {
        private readonly PocketBaseRepository<GameMetaDbData> _api;

        public PocketBaseMetaRepository(PocketBaseConfig config) 
        {
            config.SavesCollection = "game_meta"; 
            _api = new PocketBaseRepository<GameMetaDbData>(config);
        }

        public async Task<bool> SaveMetaAsync(string saveId, string sceneName)
        {
            var data = new GameMetaDbData { id = saveId, sceneName = sceneName, timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            if (await _api.ExistsAsync(saveId)) await _api.UpdateAsync(saveId, data);
            else await _api.CreateAsync(data);
            return true;
        }

        public async Task<string> LoadSceneNameAsync(string saveId)
        {
            var data = await _api.GetByIdAsync(saveId);
            return data?.sceneName;
        }
    }
}