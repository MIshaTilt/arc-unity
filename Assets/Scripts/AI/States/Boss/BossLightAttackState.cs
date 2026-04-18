using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossLightAttackState : EnemyState
    {
        private BossAI _boss;
        private float _attackTimer = 1f;

        public BossLightAttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) { _boss = context as BossAI; }

        public override void Enter()
        {
            _boss.Agent.isStopped = true;
            _boss.Animator?.SetFloat("Speed", 0f);
            _boss.Animator?.SetFloat("AttackSpeed", _boss.AttackSpeedMultiplier); // Ускоряем анимацию
            _boss.Animator?.SetTrigger("LightAttack");
            
            _boss.Target.GetComponent<IDamageable>()?.TakeDamage(15f);
            
            // Длительность атаки уменьшается при низком ХП
            _attackTimer = 1.5f / _boss.AttackSpeedMultiplier; 
        }

        public override void LogicUpdate()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0) StateMachine.ChangeState(new BossChaseState(StateMachine, _boss));
        }
    }
}