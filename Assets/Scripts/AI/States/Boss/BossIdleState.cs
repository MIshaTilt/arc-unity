using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossIdleState : EnemyState
    {
        private BossAI _boss;
        public BossIdleState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) { _boss = context as BossAI; }

        public override void LogicUpdate()
        {
            bool isHit = _boss.Health.CurrentHealth < _boss.Health.MaxHealth; // Был ли нанесен урон
            float distance = Vector3.Distance(_boss.transform.position, _boss.Target.position);

            // МИРНЫЙ РЕЖИМ БОССА: агрится только если ударили ИЛИ если не мирный режим и игрок близко
            if (isHit || (!_boss.IsPeacefulMode && distance <= _boss.DetectionRange))
            {
                StateMachine.ChangeState(new BossChaseState(StateMachine, _boss));
            }
        }
    }
}