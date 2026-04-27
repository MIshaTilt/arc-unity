using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class EarthMeleeAttack : IBossMeleeAttack
    {
        private GameObject _vfxPrefab;

        public EarthMeleeAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteMelee(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(20f);
            Debug.Log("<color=orange>УДАР КАМНЕМ!</color> Земля дрожит под ногами! (Урон: 20)");

            // Простая механика отбрасывания (knockback)
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                Vector3 pushDirection = (target.position - boss.position).normalized;
                targetRb.AddForce(pushDirection * 15f, ForceMode.Impulse);
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