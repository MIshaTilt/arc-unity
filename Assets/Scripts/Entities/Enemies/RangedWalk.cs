using UnityEngine;
using Scripts.AI.States.Ranged;
using Scripts.Configs;

namespace Scripts.AI
{
    public class RangedWalk : EnemyAI
    {
        [Header("Ranged Specific Settings")]
        public float MinAttackRange = 5f;
        public float MaxAttackRange = 10f; // Оставили для стейтов
        public WeaponConfigSO[] AvailableWeapons;

        protected override void Start()
        {
            // Выбираем случайное оружие при спавне
            if (AvailableWeapons != null && AvailableWeapons.Length > 0)
            {
                CurrentWeapon = AvailableWeapons[Random.Range(0, AvailableWeapons.Length)];
                
                AttackRange = CurrentWeapon.AttackRange; 
                MaxAttackRange = CurrentWeapon.AttackRange;
                
                // ФИКС ЗДЕСЬ: Минимальная дистанция всегда в 2 раза меньше максимальной
                // (Например, если оружие бьет на 10, моб будет держать дистанцию от 5 до 10)
                MinAttackRange = MaxAttackRange * 0.5f; 
                
                AttackCooldown = CurrentWeapon.AttackCooldown;
            }

            base.Start();
            StateMachine.Initialize(new RangedIdleState(StateMachine, this));
        }
    }
}