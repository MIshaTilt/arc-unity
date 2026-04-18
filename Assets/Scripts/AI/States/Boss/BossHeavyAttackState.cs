using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossHeavyAttackState : EnemyState
    {
        private BossAI _boss;
        private float _attackTimer = 2f;

        public BossHeavyAttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) { _boss = context as BossAI; }

        public override void Enter()
        {
            _boss.Agent.isStopped = true;
            _boss.Animator?.SetFloat("AttackSpeed", _boss.AttackSpeedMultiplier);
            _boss.Animator?.SetTrigger("HeavyAttack");
            
            _boss.Target.GetComponent<IDamageable>()?.TakeDamage(35f);
            _boss.LastHeavyAttackTime = Time.time;

            _attackTimer = 2.5f / _boss.AttackSpeedMultiplier;
        }

        public override void LogicUpdate()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0) StateMachine.ChangeState(new BossRepositionState(StateMachine, _boss)); // После сильной атаки - отпрыгивает
        }
    }
}