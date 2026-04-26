using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class IceBossFactory : IBossElementFactory
    {
        private GameObject _meleeVFX;
        private GameObject _rangedVFX;

        // Конструктор принимает префабы
        public IceBossFactory(GameObject meleeVFX, GameObject rangedVFX)
        {
            _meleeVFX = meleeVFX;
            _rangedVFX = rangedVFX;
        }

        public IBossMeleeAttack CreateMeleeAttack() => new IceMeleeAttack(_meleeVFX);
        public IBossRangedAttack CreateRangedAttack() => new IceRangedAttack(_rangedVFX);
    }
}