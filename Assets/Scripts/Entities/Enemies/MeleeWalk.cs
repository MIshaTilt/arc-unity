using Scripts.AI.States.Mob;

namespace Scripts.AI
{
    public class MeleeWalk : EnemyAI
    {
        protected override void Start()
        {
            base.Start();
            // Инициализируем автомат состоянием Покоя
            StateMachine.Initialize(new IdleState(StateMachine, this));
        }
    }
}