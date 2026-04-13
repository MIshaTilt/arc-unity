// EnemyAI.cs (Базовый скрипт, заменяет MeleeWalk и RangedWalk, устраняя дублирование)
using UnityEngine;
using UnityEngine.AI;
using Scripts.MVC;
using Scripts.Save;

namespace Scripts.AI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(HealthController))]
    public abstract class EnemyAI : MonoBehaviour, IEntitySaveable
    {
        [SerializeField] protected float _detectionRange = 10f;
        [SerializeField] protected float _attackCooldown = 1f;
        [SerializeField] protected Animator _animator;

        protected NavMeshAgent _agent;
        protected Transform _target;
        protected HealthController _healthController;
        protected float _lastAttackTime;
        protected bool _isAttacking;

        #region IEntitySaveable

        /// <summary>
        /// Уникальный ID для сохранения. Формируется из типа +_instance_id.
        /// </summary>
        public string SaveId { get; private set; }
        public string EntityType => GetType().Name;

        public EntitySaveData CaptureState()
        {
            float currentHealth = _healthController != null ? _healthController.CurrentHealth : 0f;
            float maxHealth = _healthController != null ? _healthController.MaxHealth : 100f;
            bool isAlive = _healthController != null && !_healthController.IsDead;

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
            if (_healthController != null)
            {
                _healthController.SetHealth(data.currentHealth);
            }

            // Если враг мёртв — отключаем объект
            if (!data.isAlive)
            {
                gameObject.SetActive(false);
            }

            Debug.Log($"[EnemyAI] Враг {SaveId} восстановлен. HP: {data.currentHealth}/{data.maxHealth}");
        }

        #endregion

        public void Construct(Transform target)
        {
            _target = target; // Цель передается извне
        }

        /// <summary>
        /// Устанавливает уникальный ID для сохранения.
        /// Вызывается из GameBootstrapper при инициализации.
        /// </summary>
        public void SetSaveId(string id)
        {
            SaveId = id;
        }

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _healthController = GetComponent<HealthController>();

            // Генерируем SaveId автоматически, если не установлен
            if (string.IsNullOrEmpty(SaveId))
            {
                SaveId = $"{GetType().Name}_{GetHashCode()}";
            }

            // Отключаем ИИ при смерти (Слушаем событие от HealthController)
            _healthController.OnDeathEvent.AddListener(OnDeath);

            if (_animator != null) _animator.applyRootMotion = false;
        }

        protected virtual void Update()
        {
            if (_healthController.IsDead || _target == null) return;
            ExecuteBehavior();
        }

        protected abstract void ExecuteBehavior();

        protected void OnDeath()
        {
            _agent.isStopped = true;
            Destroy(gameObject, 7f);
        }
    }
}