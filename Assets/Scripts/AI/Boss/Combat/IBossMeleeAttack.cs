using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public interface IBossMeleeAttack
    {
        void ExecuteMelee(Transform boss, Transform target);
    }
}