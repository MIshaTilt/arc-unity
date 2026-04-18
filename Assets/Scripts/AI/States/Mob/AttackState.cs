using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Mob
{
    public class AttackState : EnemyState
    {
        public AttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetFloat("Speed", 0f);
        }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            // Если игрок отошел -> снова гонимся
            if (distance > Context.AttackRange)
            {
                StateMachine.ChangeState(new ChaseState(StateMachine, Context));
                return;
            }

            // Логика атаки
            if (Time.time - Context.LastAttackTime >= Context.AttackCooldown)
            {
                Context.Animator?.SetTrigger("Attack");
                Context.Target.GetComponent<IDamageable>()?.TakeDamage(10f); // Урон
                Context.LastAttackTime = Time.time;
            }
        }
    }
}