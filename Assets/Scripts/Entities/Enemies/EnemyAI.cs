// EnemyAI.cs (Базовый скрипт, заменяет MeleeWalk и RangedWalk, устраняя дублирование)
using UnityEngine;
using UnityEngine.AI;
using Scripts.MVC;

namespace Scripts.AI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(HealthController))]
    public abstract class EnemyAI : MonoBehaviour
    {
        [SerializeField] protected float _detectionRange = 10f;
        [SerializeField] protected float _attackCooldown = 1f;
        [SerializeField] protected Animator _animator;

        protected NavMeshAgent _agent;
        protected Transform _target;
        protected HealthController _healthController;
        protected float _lastAttackTime;
        protected bool _isAttacking;

        public void Construct(Transform target)
        {
            _target = target; // Цель передается извне
        }

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _healthController = GetComponent<HealthController>();
            
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