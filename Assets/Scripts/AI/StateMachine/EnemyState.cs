namespace Scripts.AI.StateMachine
{
    public abstract class EnemyState
    {
        protected EnemyStateMachine StateMachine;
        protected EnemyAI Context; // Ссылка на самого врага для доступа к компонентам

        public EnemyState(EnemyStateMachine stateMachine, EnemyAI context)
        {
            StateMachine = stateMachine;
            Context = context;
        }

        // Вызывается при входе в состояние
        public virtual void Enter() { }
        
        // Вызывается при выходе
        public virtual void Exit() { }
        
        // Заменяет обычный Update
        public virtual void LogicUpdate() { }
        
        // Заменяет FixedUpdate
        public virtual void PhysicsUpdate() { }
    }
}