using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class IceMeleeAttack : IBossMeleeAttack
    {
        private GameObject _vfxPrefab;

        public IceMeleeAttack(GameObject vfxPrefab)
        {
            _vfxPrefab = vfxPrefab;
        }

        public void ExecuteMelee(Transform boss, Transform target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(15f);
            Debug.Log("<color=cyan>ЛЕДЯНОЙ УДАР!</color> Звук разбивающегося стекла. Игрок замерзает! (Урон: 15)");

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