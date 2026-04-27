using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Mob
{
    public class ChaseState : EnemyState
    {
        public ChaseState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

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

            if (distance <= Context.AttackRange)
            {
                StateMachine.ChangeState(new AttackState(StateMachine, Context));
            }
            else if (distance > Context.DetectionRange * 1.5f) // Потерял игрока
            {
                StateMachine.ChangeState(new IdleState(StateMachine, Context));
            }
            else
            {
                Context.Agent.SetDestination(Context.Target.position);
                Context.Animator?.SetFloat("Speed", 1f);
            }
        }
    }
}