// MeleeWalk.cs
using UnityEngine;
using Scripts.AI;

namespace Scripts
{
    public class MeleeWalk : EnemyAI
    {
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackDamage = 10f;

        protected override void ExecuteBehavior()
        {
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance <= _detectionRange)
            {
                if (distance > _attackRange)
                {
                    _agent.SetDestination(_target.position);
                    _animator?.SetFloat("Speed", 1f);
                }
                else
                {
                    _agent.SetDestination(transform.position);
                    _animator?.SetFloat("Speed", 0f);
                    PerformAttack();
                }
            }
            else
            {
                _animator?.SetFloat("Speed", 0f);
            }
        }

        private void PerformAttack()
        {
            if (_isAttacking || Time.time - _lastAttackTime < _attackCooldown) return;

            _target.GetComponent<IDamageable>()?.TakeDamage(_attackDamage);
            _lastAttackTime = Time.time;

            if (_animator != null)
            {
                _isAttacking = true;
                _animator.SetTrigger("Attack");
                Invoke(nameof(StopAttack), 0.5f);
            }
        }

        private void StopAttack() => _isAttacking = false;
    }
}