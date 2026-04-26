using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class EarthRangedAttack : IBossRangedAttack
    {
        private GameObject _vfxPrefab;

        public EarthRangedAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteRanged(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(35f);
            Debug.Log("<color=orange>БРОСОК ВАЛУНА!</color> Вздымается облако пыли! (Урон: 35)");

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