using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class FireRangedAttack : IBossRangedAttack
    {
        private GameObject _vfxPrefab;

        public FireRangedAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteRanged(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(45f);
            Debug.Log("<color=red>ОГНЕННЫЙ ШАР!</color> Взрыв красных искр! (Урон: 45)");

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