using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class FireMeleeAttack : IBossMeleeAttack
    {
        private GameObject _vfxPrefab;

        public FireMeleeAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteMelee(Transform boss, Transform target)
        {
            // Урон выше среднего
            target.GetComponent<IDamageable>()?.TakeDamage(25f);
            Debug.Log("<color=red>ОГНЕННЫЙ УДАР!</color> Меч оставляет след пламени. (Урон: 25)");

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