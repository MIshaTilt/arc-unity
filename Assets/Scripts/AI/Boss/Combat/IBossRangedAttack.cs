using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public interface IBossRangedAttack
    {
        void ExecuteRanged(Transform boss, Transform target);
    }
}