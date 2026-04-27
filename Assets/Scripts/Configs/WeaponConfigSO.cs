using UnityEngine;

namespace Scripts.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig")]
    public class WeaponConfigSO : ScriptableObject
    {
        public string WeaponName;
        public float Damage;
        public float AttackRange;
        public float AttackCooldown;
        
        [Header("Visuals")]
        public GameObject WeaponPrefab; // Префаб оружия (чтобы вложить в руку)
        public GameObject ProjectilePrefab; // Для дальнего боя
    }
}