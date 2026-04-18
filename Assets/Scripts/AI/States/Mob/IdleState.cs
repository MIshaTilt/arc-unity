using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Mob
{
    public class IdleState : EnemyState
    {
        public IdleState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetFloat("Speed", 0f);
        }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            // Логика бегства (даже в мирном режиме, если мало ХП)
            if (Context.Health.CurrentHealth <= Context.FleeHealthThreshold && Context.Health.CurrentHealth < Context.Health.MaxHealth)
            {
                StateMachine.ChangeState(new FleeState(StateMachine, Context));
                return;
            }

            // Если не мирный режим и игрок рядом -> Агрессия
            if (!Context.IsPeacefulMode && distance <= Context.DetectionRange)
            {
                StateMachine.ChangeState(new ChaseState(StateMachine, Context));
            }
        }
    }
}