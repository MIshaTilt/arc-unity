using System.Threading.Tasks;
using Scripts.Save.DTO;

namespace Scripts.Save.Domain
{
    public interface IPlayerRepository
    {
        Task<bool> SavePlayerAsync(string saveId, PlayerPositionData position, PlayerStateData state);
        Task<(PlayerPositionData position, PlayerStateData state)> LoadPlayerAsync(string saveId);
    }
}