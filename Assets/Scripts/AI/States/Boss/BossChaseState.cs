using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossChaseState : EnemyState
    {
        private BossAI _boss;
        public BossChaseState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) { _boss = context as BossAI; }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(_boss.transform.position, _boss.Target.position);

            // Если подошли вплотную - решаем какую атаку делать
            if (distance <= _boss.AttackRange)
            {
                if (Time.time - _boss.LastHeavyAttackTime >= _boss.HeavyAttackCooldown / _boss.AttackSpeedMultiplier)
                    StateMachine.ChangeState(new BossHeavyAttackState(StateMachine, _boss));
                else
                    StateMachine.ChangeState(new BossLightAttackState(StateMachine, _boss));
            }
            else
            {
                _boss.Agent.isStopped = false;
                _boss.Agent.SetDestination(_boss.Target.position);
                _boss.Animator?.SetFloat("Speed", 1f * _boss.AttackSpeedMultiplier); // Если злой - бежит быстрее
            }
        }
    }
}