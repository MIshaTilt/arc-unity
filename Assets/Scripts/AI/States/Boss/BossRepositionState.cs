using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossRepositionState : EnemyState
    {
        private BossAI _boss;
        private Vector3 _targetPos;

        public BossRepositionState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) { _boss = context as BossAI; }

        public override void Enter()
        {
            _boss.Agent.isStopped = false;
            // Выбирает точку сбоку/сзади для отхода
            _targetPos = _boss.transform.position + (_boss.transform.right * Random.Range(-5f, 5f)) - (_boss.transform.forward * 5f);
            _boss.Agent.SetDestination(_targetPos);
        }

        public override void LogicUpdate()
        {
            if (Vector3.Distance(_boss.transform.position, _targetPos) < 1f)
            {
                StateMachine.ChangeState(new BossChaseState(StateMachine, _boss));
            }
        }
    }
}