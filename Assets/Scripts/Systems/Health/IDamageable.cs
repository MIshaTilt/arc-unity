using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Интерфейс для объектов, которые могут получать урон
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
