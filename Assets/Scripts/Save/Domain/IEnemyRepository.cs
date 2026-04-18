using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.DTO;

namespace Scripts.Save.Domain
{
    public interface IEnemyRepository
    {
        Task<bool> SaveEnemiesAsync(string saveId, List<EntitySaveData> enemies);
        Task<List<EntitySaveData>> LoadEnemiesAsync(string saveId);
    }
}