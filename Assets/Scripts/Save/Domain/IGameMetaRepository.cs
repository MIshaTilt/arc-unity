using System.Threading.Tasks;

namespace Scripts.Save.Domain
{
    public interface IGameMetaRepository
    {
        Task<bool> SaveMetaAsync(string saveId, string sceneName, int killCount);
        Task<(string sceneName, int killCount)> LoadMetaAsync(string saveId);

    }
}