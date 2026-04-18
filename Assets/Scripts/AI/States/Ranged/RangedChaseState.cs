using UnityEngine;
using Scripts.AI.StateMachine;
using Scripts.AI.States.Mob;

namespace Scripts.AI.States.Ranged
{
    public class RangedChaseState : EnemyState
    {
        private RangedWalk _ranged;

        public RangedChaseState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) 
        {
            _ranged = context as RangedWalk;
        }

        public override void Enter()
        {
            Context.Agent.isStopped = false;
        }

        public override void LogicUpdate()
        {
            if (Context.Health.CurrentHealth <= Context.FleeHealthThreshold)
            {
                StateMachine.ChangeState(new FleeState(StateMachine, Context));
                return;
            }

            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            // Если вышли за радиус обнаружения — возвращаемся в покой
            if (distance > Context.DetectionRange * 1.5f)
            {
                StateMachine.ChangeState(new RangedIdleState(StateMachine, Context));
                return;
            }

            // Логика позиционирования
            if (distance < _ranged.MinAttackRange)
            {
                // Игрок слишком близко -> отступаем
                Vector3 direction = (Context.Target.position - Context.transform.position).normalized;
                Vector3 targetPosition = Context.Target.position - direction * _ranged.MinAttackRange;
                Context.Agent.SetDestination(targetPosition);
                Context.Animator?.SetFloat("Speed", 1f);
            }
            else if (distance > _ranged.MaxAttackRange)
            {
                // Игрок слишком далеко -> сближаемся
                Vector3 direction = (Context.Target.position - Context.transform.position).normalized;
                Vector3 targetPosition = Context.Target.position - direction * _ranged.MaxAttackRange;
                Context.Agent.SetDestination(targetPosition);
                Context.Animator?.SetFloat("Speed", 1f);
            }
            else
            {
                // Игрок в оптимальной зоне -> атакуем
                StateMachine.ChangeState(new RangedAttackState(StateMachine, Context));
            }
        }
    }
}