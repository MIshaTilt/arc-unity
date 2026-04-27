using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class AetherBossFactory : IBossElementFactory
    {
        private GameObject _meleeVFX;
        private GameObject _rangedVFX;

        // Конструктор принимает префабы
        public AetherBossFactory(GameObject meleeVFX, GameObject rangedVFX)
        {
            _meleeVFX = meleeVFX;
            _rangedVFX = rangedVFX;
        }

        public IBossMeleeAttack CreateMeleeAttack() => new AetherMeleeAttack(_meleeVFX);
        public IBossRangedAttack CreateRangedAttack() => new AetherRangedAttack(_rangedVFX);
    }
}