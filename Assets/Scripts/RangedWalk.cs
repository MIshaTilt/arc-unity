using UnityEngine;
using UnityEngine.AI;
using Scripts;

public class RangedWalk : MonoBehaviour
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

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _player = player.transform;
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
            // Если слишком близко — отходим
            if (_distanceToPlayer < _minAttackRange)
            {
                MoveAwayFromPlayer();
            }
            // Если слишком далеко — приближаемся
            else if (_distanceToPlayer > _maxAttackRange)
            {
                MoveTowardsPlayer();
            }
            // Если в оптимальной зоне — стоим и атакуем
            else
            {
                _agent.SetDestination(transform.position);
                PerformAttack();
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
