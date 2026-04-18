using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Mob
{
    public class FleeState : EnemyState
    {
        public FleeState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = false;
        }

        public override void LogicUpdate()
        {
            // Бежим в противоположную сторону от игрока
            Vector3 directionAway = (Context.transform.position - Context.Target.position).normalized;
            Vector3 fleePosition = Context.transform.position + directionAway * 5f;
            
            Context.Agent.SetDestination(fleePosition);
            Context.Animator?.SetFloat("Speed", 1f);

            // Если отбежали достаточно далеко и здоровье не убавляется, можно остановиться
            if (Vector3.Distance(Context.transform.position, Context.Target.position) > Context.DetectionRange * 2f)
            {
                StateMachine.ChangeState(new IdleState(StateMachine, Context));
            }
        }
    }
}