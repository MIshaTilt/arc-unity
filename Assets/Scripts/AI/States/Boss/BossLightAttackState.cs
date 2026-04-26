using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossLightAttackState : EnemyState
    {
        private BossAI _boss;
        private float _attackTimer;
        private float _damageDelayTimer;
        private bool _hasDamaged;

        public BossLightAttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) 
        { 
            _boss = context as BossAI; 
        }

        public override void Enter()
        {
            _boss.Agent.isStopped = true;
            _boss.Animator?.SetFloat("AttackSpeed", _boss.AttackSpeedMultiplier);
            _boss.Animator?.SetTrigger("LightAttack");
            
            _hasDamaged = false;
            
            _attackTimer = 2f / _boss.AttackSpeedMultiplier; 
            
            _damageDelayTimer = 0.8f / _boss.AttackSpeedMultiplier; 
        }

        public override void LogicUpdate()
        {
            _attackTimer -= Time.deltaTime;
            _damageDelayTimer -= Time.deltaTime;

            if (!_hasDamaged)
            {
                Vector3 lookDirection = _boss.Target.position - _boss.transform.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
                }
            }

            if (!_hasDamaged && _damageDelayTimer <= 0)
            {
                _hasDamaged = true;

                if (Vector3.Distance(_boss.transform.position, _boss.Target.position) <= _boss.AttackRange + 0.5f)
                {
                    _boss.CurrentMeleeAttack.ExecuteMelee(_boss.transform, _boss.Target);
                }
            }

            if (_attackTimer <= 0) 
            {
                StateMachine.ChangeState(new BossChaseState(StateMachine, _boss));
            }
        }
    }
}