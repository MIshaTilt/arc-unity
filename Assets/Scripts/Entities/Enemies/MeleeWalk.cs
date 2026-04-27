using UnityEngine;
using Scripts.AI.States.Mob;
using Scripts.Configs;

namespace Scripts.AI
{
    public class MeleeWalk : EnemyAI
    {
        public WeaponConfigSO[] AvailableWeapons; // Перетащи сюда SwordConfig и AxeConfig в инспекторе

        protected override void Start()
        {
            // Выбираем случайное оружие при спавне
            if (AvailableWeapons != null && AvailableWeapons.Length > 0)
            {
                CurrentWeapon = AvailableWeapons[Random.Range(0, AvailableWeapons.Length)];
                AttackRange = CurrentWeapon.AttackRange; // Обновляем параметры ИИ
                AttackCooldown = CurrentWeapon.AttackCooldown;

                EquipWeapon();
            }
            
            base.Start();
            StateMachine.Initialize(new IdleState(StateMachine, this));
        }

    }
}