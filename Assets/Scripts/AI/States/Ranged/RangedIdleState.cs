using UnityEngine;
using Scripts.AI.StateMachine;
using Scripts.AI.States.Mob; // Для доступа к FleeState

namespace Scripts.AI.States.Ranged
{
    public class RangedIdleState : EnemyState
    {
        public RangedIdleState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetFloat("Speed", 0f);
        }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            // Если мало ХП - убегаем (переиспользуем FleeState от милишника)
            if (Context.Health.CurrentHealth <= Context.FleeHealthThreshold && Context.Health.CurrentHealth < Context.Health.MaxHealth)
            {
                StateMachine.ChangeState(new FleeState(StateMachine, Context));
                return;
            }

            // МИРНЫЙ РЕЖИМ: если не мирный режим и игрок рядом -> Агримся
            if (!Context.IsPeacefulMode && distance <= Context.DetectionRange)
            {
                StateMachine.ChangeState(new RangedChaseState(StateMachine, Context));
            }
        }
    }
}