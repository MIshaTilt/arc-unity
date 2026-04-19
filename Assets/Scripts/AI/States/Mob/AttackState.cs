using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Mob
{
    public class AttackState : EnemyState
    {
        private bool _isAttacking;
        private float _damageDelayTimer;
        private bool _hasDamaged;
        
        private float _animationDuration = 2.0f; 

        public AttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetFloat("Speed", 0f);
            _isAttacking = false;
        }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            if (!_isAttacking)
            {
                if (distance > Context.AttackRange)
                {
                    StateMachine.ChangeState(new ChaseState(StateMachine, Context));
                    return;
                }

                if (Time.time - Context.LastAttackTime >= Context.AttackCooldown)
                {
                    StartAttack();
                }
                else
                {
                    RotateTowardsTarget();
                }
            }
            else
            {
                _damageDelayTimer -= Time.deltaTime;

                if (!_hasDamaged)
                {
                    RotateTowardsTarget(); 

                    if (_damageDelayTimer <= 0)
                    {
                        _hasDamaged = true;

                        if (distance <= Context.AttackRange + 0.5f)
                        {
                            Context.Target.GetComponent<IDamageable>()?.TakeDamage(10f);
                        }
                    }
                }

                if (Time.time - Context.LastAttackTime >= _animationDuration)
                {
                    _isAttacking = false;
                }
            }
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _hasDamaged = false;
            
            _damageDelayTimer = 0.8f; 
            
            Context.LastAttackTime = Time.time;
            Context.Animator?.SetTrigger("Attack");
        }

        private void RotateTowardsTarget()
        {
            Vector3 lookDirection = Context.Target.position - Context.transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }
        }
    }
}