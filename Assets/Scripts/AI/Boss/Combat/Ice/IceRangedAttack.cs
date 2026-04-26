using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class IceRangedAttack : IBossRangedAttack
    {
        private GameObject _vfxPrefab;

        public IceRangedAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteRanged(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(30f);
            Debug.Log("<color=cyan>ЛЕДЯНОЙ ШИП!</color> Синие частицы льда разлетаются вокруг! (Урон: 30)");

            if (_vfxPrefab != null)
            {
                // Для дальней атаки логичнее заспавнить эффект прямо на цели (взрыв)
                GameObject vfx = GameObject.Instantiate(_vfxPrefab, target.position, Quaternion.identity);
                
                // Уничтожаем через 10 секунд
                GameObject.Destroy(vfx, 10f);
            }
        }
    }
}