using UnityEngine;
using Scripts.MVC; // Пространство имен, где лежит HealthController

namespace Scripts.AI
{
    public class RangedWalk : EnemyAI
    {
        [Header("Ranged Specific Settings")]
        [SerializeField] private float _minAttackRange = 5f;
        [SerializeField] private float _maxAttackRange = 10f;
        [SerializeField] private GameObject _fireballPrefab;

        // Метод ExecuteBehavior вызывается из базового класса EnemyAI в Update(),
        // но только если враг жив и цель (_target) существует.
        protected override void ExecuteBehavior()
        {
            float distanceToTarget = Vector3.Distance(transform.position, _target.position);

            // Если игрок в радиусе обнаружения
            if (distanceToTarget <= _detectionRange)
            {
                // Если слишком близко — отходим
                if (distanceToTarget < _minAttackRange)
                {
                    MoveAwayFromTarget();
                    _animator?.SetFloat("Speed", 1f);
                }
                // Если слишком далеко — приближаемся
                else if (distanceToTarget > _maxAttackRange)
                {
                    MoveTowardsTarget();
                    _animator?.SetFloat("Speed", 1f);
                }
                // Если в оптимальной зоне — стоим и атакуем
                else
                {
                    _agent.SetDestination(transform.position); // Остановка
                    _animator?.SetFloat("Speed", 0f);
                    PerformAttack();
                }
            }
            else
            {
                // Игрок вне радиуса — стоим на месте
                _animator?.SetFloat("Speed", 0f);
            }
        }

        private void MoveTowardsTarget()
        {
            // Идем к точке, находящейся на границе максимального радиуса атаки
            Vector3 direction = (_target.position - transform.position).normalized;
            Vector3 targetPosition = _target.position - direction * _maxAttackRange;
            _agent.SetDestination(targetPosition);
        }

        private void MoveAwayFromTarget()
        {
            // Отходим на границу минимального радиуса атаки (отступаем)
            Vector3 direction = (_target.position - transform.position).normalized;
            Vector3 targetPosition = _target.position - direction * _minAttackRange;
            _agent.SetDestination(targetPosition);
        }

        private void PerformAttack()
        {
            // Проверяем состояние атаки и кулдаун (переменные унаследованы от EnemyAI)
            if (_isAttacking || Time.time - _lastAttackTime < _attackCooldown)
            {
                return;
            }

            if (_fireballPrefab == null) return;

            // Логика из оригинала: спавн файербола над игроком
            Vector3 fireballPosition = _target.position + Vector3.up * 2f;
            Instantiate(_fireballPrefab, fireballPosition, Quaternion.identity);

            _lastAttackTime = Time.time;

            if (_animator != null)
            {
                _isAttacking = true;
                _animator.SetTrigger("Attack");
                
                // Сбрасываем флаг атаки через полсекунды
                Invoke(nameof(StopAttack), 0.5f);
            }
        }

        private void StopAttack()
        {
            _isAttacking = false;
        }

        // Отрисовка зон в редакторе Unity для удобства настройки
        private void OnDrawGizmosSelected()
        {
            // Радиус обнаружения (желтый) - переменная из базового класса
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
}