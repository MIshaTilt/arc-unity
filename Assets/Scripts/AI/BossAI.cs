using UnityEngine;
using Scripts.AI.States.Boss;
using Scripts.AI.Boss.Combat;


namespace Scripts.AI
{
    public class BossAI : EnemyAI
    {
        [Header("Boss Settings")]
        public float HeavyAttackRange = 3f;
        public float HeavyAttackCooldown = 5f;
        public float LastHeavyAttackTime;

        [Header("VFX Prefabs (Optional)")]
        public GameObject FireMeleeVFX;
        public GameObject FireRangedVFX;
        
        public GameObject IceMeleeVFX;
        public GameObject IceRangedVFX;
        
        public GameObject EarthMeleeVFX;
        public GameObject EarthRangedVFX;
        
        public GameObject AetherMeleeVFX;
        public GameObject AetherRangedVFX;

        public IBossMeleeAttack CurrentMeleeAttack { get; private set; }
        public IBossRangedAttack CurrentHeavyAttack { get; private set; }


        // ДОП БАЛЛ: Множитель скорости атаки (если ХП < 50%, то скорость х2)
        public float AttackSpeedMultiplier => (Health.CurrentHealth < Health.MaxHealth * 0.5f) ? 2f : 1f;

        protected override void Start()
        {
            // Выбираем случайную стихию при рождении
            IBossElementFactory elementFactory = GetRandomElementFactory();
            
            // Собираем атаки (Оружия)
            CurrentMeleeAttack = elementFactory.CreateMeleeAttack();
            CurrentHeavyAttack = elementFactory.CreateRangedAttack();

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

        private IBossElementFactory GetRandomElementFactory()
        {
            int rand = Random.Range(0, 4);
            switch (rand)
            {
                case 0: return new IceBossFactory(IceMeleeVFX, IceRangedVFX);
                case 1: return new FireBossFactory(FireMeleeVFX, FireRangedVFX);
                case 2: return new EarthBossFactory(EarthMeleeVFX, EarthRangedVFX);
                default: return new AetherBossFactory(AetherMeleeVFX, AetherRangedVFX);
            }
        }

    }
}