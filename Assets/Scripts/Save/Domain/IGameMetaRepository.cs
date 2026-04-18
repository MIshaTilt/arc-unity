using System.Threading.Tasks;

namespace Scripts.Save.Domain
{
    public interface IGameMetaRepository
    {
        Task<bool> SaveMetaAsync(string saveId, string sceneName);
        Task<string> LoadSceneNameAsync(string saveId);
    }
}