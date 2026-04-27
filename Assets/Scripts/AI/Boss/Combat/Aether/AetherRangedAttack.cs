using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class AetherRangedAttack : IBossRangedAttack
    {
        private GameObject _vfxPrefab;

        public AetherRangedAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteRanged(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(25f);
            Debug.Log("<color=magenta>ЭФИРНЫЙ ЛУЧ!</color> Пронзает насквозь фиолетовым свечением! (Урон: 25)");

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