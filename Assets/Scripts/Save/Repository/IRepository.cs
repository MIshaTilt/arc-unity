using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scripts.Save.Repository
{
    /// <summary>
    /// Паттерн Репозиторий: абстракция доступа к данным.
    /// Позволяет заменить хранилище (PocketBase → PostgreSQL → File) без изменения бизнес-логики.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T data);
        Task<T> UpdateAsync(string id, T data);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
