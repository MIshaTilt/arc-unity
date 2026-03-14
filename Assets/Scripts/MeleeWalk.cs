using UnityEngine;
using UnityEngine.AI;
using Scripts;

public class MeleeWalk : MonoBehaviour
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

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

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
            }
            else
            {
                // Игрок рядом — останавливаемся и атакуем
                _agent.SetDestination(transform.position);
                PerformAttack();
            }
        }
    }

    private void PerformAttack()
    {
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
