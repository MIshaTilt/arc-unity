using UnityEngine;

namespace Scripts.AI.Boss.Combat
{
    public class FireBossFactory : IBossElementFactory
    {
        private GameObject _meleeVFX;
        private GameObject _rangedVFX;

        // Конструктор принимает префабы
        public FireBossFactory(GameObject meleeVFX, GameObject rangedVFX)
        {
            _meleeVFX = meleeVFX;
            _rangedVFX = rangedVFX;
        }

        // И передает их в атаки при создании
        public IBossMeleeAttack CreateMeleeAttack() => new FireMeleeAttack(_meleeVFX);
        public IBossRangedAttack CreateRangedAttack() => new FireRangedAttack(_rangedVFX);
    }

}