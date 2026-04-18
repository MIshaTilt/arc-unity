using UnityEngine;
using Scripts.AI.States.Boss;

namespace Scripts.AI
{
    public class BossAI : EnemyAI
    {
        [Header("Boss Settings")]
        public float HeavyAttackRange = 3f;
        public float HeavyAttackCooldown = 5f;
        public float LastHeavyAttackTime;

        // ДОП БАЛЛ: Множитель скорости атаки (если ХП < 50%, то скорость х2)
        public float AttackSpeedMultiplier => (Health.CurrentHealth < Health.MaxHealth * 0.5f) ? 2f : 1f;

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(new BossIdleState(StateMachine, this));
        }

        // Переопределяем метод получения урона, чтобы передать это в стейт-машину
        protected override void Awake()
        {
            base.Awake();
            Health.OnDeathEvent.AddListener(() => StateMachine.ChangeState(new BossDeathState(StateMachine, this)));
            
            // Чтобы босс реагировал на удары
            var healthModel = Health.GetType().GetField("_healthModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(Health);
            // Если OnDamaged доступен, подписываемся (если нет, можно добавить публичный Event в HealthController)
        }
    }
}