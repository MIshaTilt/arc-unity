using UnityEngine;
using Scripts.AI.States.Ranged;

namespace Scripts.AI
{
    public class RangedWalk : EnemyAI
    {
        [Header("Ranged Specific Settings")]
        public float MinAttackRange = 5f;
        public float MaxAttackRange = 10f;
        public GameObject FireballPrefab;

        protected override void Start()
        {
            base.Start();
            // Инициализируем машину состояний стрелка
            StateMachine.Initialize(new RangedIdleState(StateMachine, this));
        }
    }
}