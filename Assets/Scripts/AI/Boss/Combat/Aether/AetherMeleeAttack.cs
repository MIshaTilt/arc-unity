using UnityEngine;
using Scripts.MVC; // Для доступа к HealthController

namespace Scripts.AI.Boss.Combat
{
    public class AetherMeleeAttack : IBossMeleeAttack
    {
        private GameObject _vfxPrefab;

        public AetherMeleeAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteMelee(Transform boss, Transform target)
        {
            // Урон небольшой
            target.GetComponent<IDamageable>()?.TakeDamage(10f);
            Debug.Log("<color=magenta>ЭФИРНЫЙ КЛИНОК!</color> Босс высасывает вашу жизненную силу! (Урон: 10)");

            // Восстанавливаем 10 ХП боссу
            HealthController bossHealth = boss.GetComponent<HealthController>();
            if (bossHealth != null)
            {
                bossHealth.SetHealth(bossHealth.CurrentHealth + 10f);
            }

            if (_vfxPrefab != null)
            {
                // Спавним перед боссом
                Vector3 spawnPos = boss.position + boss.forward * 1.5f + Vector3.up;
                GameObject vfx = GameObject.Instantiate(_vfxPrefab, spawnPos, boss.rotation);
                
                // Уничтожаем через 10 секунд
                GameObject.Destroy(vfx, 10f);
            }
        }
    }
}