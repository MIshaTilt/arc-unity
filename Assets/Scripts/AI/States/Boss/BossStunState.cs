using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossStunState : EnemyState
    {
        private float _stunDuration = 2f;

        public BossStunState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetTrigger("Stun");
        }

        public override void LogicUpdate()
        {
            _stunDuration -= Time.deltaTime;
            if (_stunDuration <= 0) StateMachine.ChangeState(new BossChaseState(StateMachine, Context));
        }
    }
}