using UnityEngine;
using UnityEngine.AI;
using Scripts.MVC;
using Scripts.Save;
using Scripts.AI.StateMachine;

namespace Scripts.AI
    {[RequireComponent(typeof(NavMeshAgent), typeof(HealthController))]
    public abstract class EnemyAI : MonoBehaviour, IEntitySaveable
    {
        [Header("AI Settings")]
        public float DetectionRange = 10f;
        public float AttackRange = 2f;
        public float AttackCooldown = 1f;
        public float FleeHealthThreshold = 20f; // ХП, при котором убегает
        
        [Header("Peaceful Mode")]
        public bool IsPeacefulMode = false; // Мирный режим

        public Animator Animator;
        
        public NavMeshAgent Agent { get; private set; }
        public Transform Target { get; private set; }
        public HealthController Health { get; private set; }
        
        public EnemyStateMachine StateMachine { get; protected set; }

        public float LastAttackTime { get; set; }

        #region IEntitySaveable

        /// <summary>
        /// Уникальный ID для сохранения. Формируется из типа +_instance_id.
        /// </summary>
        public string SaveId { get; private set; }
        public string EntityType => GetType().Name;

        public EntitySaveData CaptureState()
        {
            float currentHealth = Health != null ? Health.CurrentHealth : 0f;
            float maxHealth = Health != null ? Health.MaxHealth : 100f;
            bool isAlive = Health != null && !Health.IsDead;

            return new EntitySaveData
            {
                id = SaveId,
                entityType = EntityType,
                positionX = transform.position.x,
                positionY = transform.position.y,
                positionZ = transform.position.z,
                rotationY = transform.eulerAngles.y,
                currentHealth = currentHealth,
                maxHealth = maxHealth,
                isAlive = isAlive
            };
        }

        public void RestoreState(EntitySaveData data)
        {
            // Восстанавливаем позицию
            transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
            transform.rotation = Quaternion.Euler(0f, data.rotationY, 0f);

            // Восстанавливаем здоровье
            if (Health != null)
            {
                Health.SetHealth(data.currentHealth);
            }

            // Если враг мёртв — отключаем объект
            if (!data.isAlive)
            {
                gameObject.SetActive(false);
            }

            Debug.Log($"[EnemyAI] Враг {SaveId} восстановлен. HP: {data.currentHealth}/{data.maxHealth}");
        }

        public void SetSaveId(string id)
        {
            SaveId = id;
        }

        #endregion

        public void Construct(Transform target)
        {
            Target = target;
        }

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Health = GetComponent<HealthController>();

            if (string.IsNullOrEmpty(SaveId)) SaveId = $"enemy-{GetType().Name.ToLower()}-{Mathf.RoundToInt(transform.position.x * 10)}";

            Health.OnDeathEvent.AddListener(OnDeath);
            if (Animator != null) Animator.applyRootMotion = false;

            StateMachine = new EnemyStateMachine();
        }

        protected virtual void Start()
        {
            // Наследники здесь будут инициализировать свои состояния
        }

        protected virtual void Update()
        {
            if (Health.IsDead || Target == null) return;
            
            // Делегируем работу текущему состоянию
            StateMachine.CurrentState?.LogicUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (Health.IsDead || Target == null) return;
            StateMachine.CurrentState?.PhysicsUpdate();
        }

        protected virtual void OnDeath()
        {
            Agent.isStopped = true;
            // Переводим в состояние смерти, если оно есть
            Destroy(gameObject, 7f);
        }
    }
}