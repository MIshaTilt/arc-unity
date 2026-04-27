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

            if (distance > Context.DetectionRange * 1.5f)
            {
                StateMachine.ChangeState(new RangedIdleState(StateMachine, Context));
                return;
            }

            float optimalDistance =  _ranged.MaxAttackRange;

            if (distance < _ranged.MinAttackRange)
            {
                // Отступаем на идеальную дистанцию
                Vector3 direction = (Context.transform.position - Context.Target.position).normalized; // Вектор ОТ игрока
                Vector3 targetPosition = Context.Target.position + direction * optimalDistance;
                
                Context.Agent.SetDestination(targetPosition);
                Context.Animator?.SetFloat("Speed", 1f);
            }
            else if (distance > _ranged.MaxAttackRange)
            {
                // Сближаемся на идеальную дистанцию
                Vector3 direction = (Context.transform.position - Context.Target.position).normalized; 
                Vector3 targetPosition = Context.Target.position + direction * optimalDistance;
                
                Context.Agent.SetDestination(targetPosition);
                Context.Animator?.SetFloat("Speed", 1f);
            }
            else
            {
                // Дистанция в пределах нормы (от Min до Max) -> атакуем!
                Context.Agent.isStopped = true; // Тормозим агента перед атакой
                StateMachine.ChangeState(new RangedAttackState(StateMachine, Context));
            }
        }

    }
}