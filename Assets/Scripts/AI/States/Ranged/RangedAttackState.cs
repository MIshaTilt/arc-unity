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

            // Поворачиваемся к игроку перед выстрелом
            Vector3 lookDirection = Context.Target.position - Context.transform.position;
            lookDirection.y = 0; 
            if (lookDirection != Vector3.zero)
            {
                Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            // Стреляем, если прошел кулдаун
            if (Time.time - Context.LastAttackTime >= Context.AttackCooldown)
            {
                if (Context.CurrentWeapon != null && Context.CurrentWeapon.ProjectilePrefab != null)
                {
                    // Точка спавна: над игроком
                    Vector3 fireballPosition = Context.Target.position + Vector3.up * 2f;
                    
                    // Сохраняем ссылку на созданный объект
                    GameObject projectileObj = GameObject.Instantiate(Context.CurrentWeapon.ProjectilePrefab, fireballPosition, Quaternion.identity);
                    Debug.Log("Должен выстрелить");
                    
                    // Ищем наш скрипт снаряда и передаем ему урон из Scriptable Object
                    MagicProjectile projectileScript = projectileObj.GetComponent<MagicProjectile>();
                    if (projectileScript != null)
                    {
                        projectileScript.SetDamage(Context.CurrentWeapon.Damage);
                    }
                }

                Context.Animator?.SetTrigger("Attack");
                Context.LastAttackTime = Time.time;
            }
        }
    }
}