using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Boss
{
    public class BossDeathState : EnemyState
    {
        public BossDeathState(EnemyStateMachine sm, EnemyAI context) : base(sm, context) {}

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Agent.enabled = false;
            Context.Animator?.SetTrigger("DeathTrigger");
        }

        public override void LogicUpdate() { /* Мертв, ничего не делает */ }
    }
}