using System;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using Scripts.Save.DTO;

namespace Scripts.Save.Repository
{
    [Serializable]
    public class PlayerDbData 
    { 
        public string id; 
        public PlayerPositionData position; 
        public PlayerStateData state; 
    }

    public class PocketBasePlayerRepository : IPlayerRepository
    {
        private readonly PocketBaseRepository<PlayerDbData> _api;

        public PocketBasePlayerRepository(PocketBaseConfig config)
        {
            config.SavesCollection = "player_saves";
            _api = new PocketBaseRepository<PlayerDbData>(config);
        }

        public async Task<bool> SavePlayerAsync(string saveId, PlayerPositionData position, PlayerStateData state)
        {
            var data = new PlayerDbData { id = saveId, position = position, state = state };
            if (await _api.ExistsAsync(data.id)) await _api.UpdateAsync(data.id, data);
            else await _api.CreateAsync(data);
            return true;
        }

        public async Task<(PlayerPositionData, PlayerStateData)> LoadPlayerAsync(string saveId)
        {
            var data = await _api.GetByIdAsync(saveId);
            if (data == null) return (null, null);
            return (data.position, data.state);
        }
    }
}