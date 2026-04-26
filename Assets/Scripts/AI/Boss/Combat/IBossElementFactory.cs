using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public interface IBossElementFactory
    {
        IBossMeleeAttack CreateMeleeAttack();
        IBossRangedAttack CreateRangedAttack();
    }
}