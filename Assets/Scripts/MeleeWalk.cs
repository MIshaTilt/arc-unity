using UnityEngine;
using UnityEngine.AI;
using Scripts;

public class MeleeWalk : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackCooldown = 1f;

    private NavMeshAgent _agent;
    private Transform _player;
    private float _distanceToPlayer;
    private float _lastAttackTime;
    private IDamageable _playerDamageable;
    [SerializeField] Animator _animator;
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
            _playerDamageable = player.GetComponent<IDamageable>();
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
            // Если игрок дальше радиуса атаки — преследуем
            if (_distanceToPlayer > _attackRange)
            {
                _agent.SetDestination(_player.position);
                
                // Анимация движения
                if (_animator != null)
                {
                    _animator.SetFloat("Speed", 1f);
                }
            }
            else
            {
                // Игрок рядом — останавливаемся и атакуем
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

    private void PerformAttack()
    {
        if (_isAttacking)
        {
            return;
        }

        if (_playerDamageable == null)
        {
            _playerDamageable = _player.GetComponent<IDamageable>();
            return;
        }

        // Проверяем кулдаун
        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return;
        }

        // Наносим урон
        _playerDamageable.TakeDamage(_attackDamage);
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
        if (_isDead) return;

        if (_animator != null)
        {
            _animator.SetTrigger("TakeHit");
        }
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        if (_animator != null)
        {
            _animator.SetTrigger("DeathTrigger");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения (желтый)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // Радиус атаки (красный)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
