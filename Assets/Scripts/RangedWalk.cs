using UnityEngine;
using UnityEngine.AI;
using Scripts;
using System.Linq.Expressions;

public class RangedWalk : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private float _minAttackRange = 5f;
    [SerializeField] private float _maxAttackRange = 10f;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private GameObject _fireballPrefab;

    private NavMeshAgent _agent;
    private Transform _player;
    private float _distanceToPlayer;
    private Vector3 _optimalPosition;
    private float _lastAttackTime;
    [SerializeField] private Animator _animator;
    private bool _isDead;
    private bool _isAttacking;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        //_animator = GetComponent<Animator>();
        _agent.speed = _moveSpeed;

        // Отключаем Root Motion
        if (_animator != null)
        {
            _animator.applyRootMotion = false;
        }

        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _player = player.transform;
        }
    }

    private void Update()
    {
        // Если умер — блокируем всё
        if (_isDead)
        {
            return;
        }

        if (_player == null)
        {
            return;
        }

        _distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Если игрок в радиусе обнаружения
        if (_distanceToPlayer <= _detectionRange)
        {
            // Если слишком близко — отходим
            if (_distanceToPlayer < _minAttackRange)
            {
                MoveAwayFromPlayer();
                
                // Анимация движения
                if (_animator != null)
                {
                    _animator.SetFloat("Speed", 1f);
                }
            }
            // Если слишком далеко — приближаемся
            else if (_distanceToPlayer > _maxAttackRange)
            {
                MoveTowardsPlayer();
                
                // Анимация движения
                if (_animator != null)
                {
                    _animator.SetFloat("Speed", 1f);
                }
            }
            // Если в оптимальной зоне — стоим и атакуем
            else
            {
                _agent.SetDestination(transform.position);
                
                // Анимация idle
                if (_animator != null)
                {
                    _animator.SetFloat("Speed", 0f);
                }
                
                PerformAttack();
            }
        }
        else
        {
            // Игрок вне радиуса — idle
            if (_animator != null)
            {
                _animator.SetFloat("Speed", 0f);
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        Vector3 targetPosition = _player.position - direction * _maxAttackRange;
        _agent.SetDestination(targetPosition);
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        Vector3 targetPosition = _player.position - direction * _minAttackRange;
        _agent.SetDestination(targetPosition);
    }

    private void PerformAttack()
    {
        if (_isAttacking)
        {
            return;
        }

        // Проверяем кулдаун
        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return;
        }

        if (_fireballPrefab == null || _player == null)
        {
            return;
        }

        // Создаем файербол прямо над игроком (на высоте 2 метра)
        Vector3 fireballPosition = _player.position + Vector3.up * 2f;
        Instantiate(_fireballPrefab, fireballPosition, Quaternion.identity);

        _lastAttackTime = Time.time;

        // Анимация атаки
        if (_animator != null && !_isAttacking)
        {
            _isAttacking = true;
            _animator.SetTrigger("Attack");
            Invoke(nameof(StopAttack), 0.5f);
        }
    }

    private void StopAttack()
    {
        _isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Ahhh hit");
        if (_isDead) return;

        if (_animator != null)
        {
            Debug.Log("TakeHit");
            
        }
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        if (_animator != null)
        {
            Debug.Log("DeathTrigger");
            _animator.SetTrigger("DeathTrigger");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения (желтый)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // Минимальная дистанция атаки (оранжевый)
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, _minAttackRange);

        // Максимальная дистанция атаки (зеленый)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _maxAttackRange);
    }
}
