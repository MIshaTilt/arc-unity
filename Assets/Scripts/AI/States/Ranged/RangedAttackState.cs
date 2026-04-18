using UnityEngine;
using Scripts.AI.StateMachine;

namespace Scripts.AI.States.Ranged
{
    public class RangedAttackState : EnemyState
    {
        private RangedWalk _ranged;

        public RangedAttackState(EnemyStateMachine sm, EnemyAI context) : base(sm, context)
        {
            _ranged = context as RangedWalk;
        }

        public override void Enter()
        {
            Context.Agent.isStopped = true;
            Context.Animator?.SetFloat("Speed", 0f);
        }

        public override void LogicUpdate()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.Target.position);

            // Если игрок вышел из комфортной зоны для выстрела -> возвращаемся к позиционированию
            if (distance < _ranged.MinAttackRange || distance > _ranged.MaxAttackRange)
            {
                StateMachine.ChangeState(new RangedChaseState(StateMachine, Context));
                return;
            }

            // Поворачиваемся к игроку перед выстрелом (так как агент остановлен)
            Vector3 lookDirection = Context.Target.position - Context.transform.position;
            lookDirection.y = 0; // Игнорируем высоту
            if (lookDirection != Vector3.zero)
            {
                Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            // Стреляем, если прошел кулдаун
            if (Time.time - Context.LastAttackTime >= Context.AttackCooldown)
            {
                if (_ranged.FireballPrefab != null)
                {
                    Vector3 fireballPosition = Context.Target.position + Vector3.up * 2f;
                    GameObject.Instantiate(_ranged.FireballPrefab, fireballPosition, Quaternion.identity);
                }

                Context.Animator?.SetTrigger("Attack");
                Context.LastAttackTime = Time.time;
            }
        }
    }
}