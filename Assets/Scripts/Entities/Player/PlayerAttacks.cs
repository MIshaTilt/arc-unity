// PlayerAttacks.cs
using UnityEngine;
using Scripts.Services;
using Scripts.MVC;

namespace Scripts
{
    public class PlayerAttacks : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _physicalDamage = 10f;
        [SerializeField] private float _magicDamage = 25f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private GameObject _magicSlashVFX;
        [SerializeField] private Transform _magicAttackPoint;
        [SerializeField] private Animator _animator;

        private IInputService _inputService;
        private Camera _mainCamera;
        private HealthController _healthController;

        // DI Внедрение
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
            _inputService.OnPhysicalAttack += OnPhysicalAttack;
            _inputService.OnMagicAttack += OnMagicAttack;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _healthController = GetComponent<HealthController>();
        }

        private void OnDestroy()
        {
            if (_inputService != null)
            {
                _inputService.OnPhysicalAttack -= OnPhysicalAttack;
                _inputService.OnMagicAttack -= OnMagicAttack;
            }
        }

        private void OnPhysicalAttack()
        {
            if (_healthController != null && _healthController.IsDead) return;
            _animator?.SetTrigger("AttackSword");
            PerformAttack(_physicalDamage, false);
        }

        private void OnMagicAttack()
        {
            if (_healthController != null && _healthController.IsDead) return;
            _animator?.SetTrigger("AttackMagic");
            PerformAttack(_magicDamage, true);
        }

        private void PerformAttack(float damage, bool isMagic)
        {
            Vector3 rayOrigin = _mainCamera != null ? _mainCamera.transform.position : transform.position;
            Vector3 rayDirection = _mainCamera != null ? _mainCamera.transform.forward : transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _attackRange, _targetLayerMask))
            {
                hit.collider.GetComponent<IDamageable>()?.TakeDamage(damage);
                ApplyPushForce(hit.collider, hit.point, rayOrigin, isMagic);

                if (isMagic) ApplyAreaDamage(hit.point, damage * 0.5f, 3f);
            }
            else if (isMagic)
            {
                ApplyAreaDamage(rayOrigin + rayDirection * _attackRange, damage * 0.5f, 3f);
            }

            if (isMagic) SpawnMagicVFXAtSword();
        }

        // Вспомогательные методы ApplyPushForce, ApplyAreaDamage, SpawnMagicVFXAtSword остаются как были.
        private void ApplyPushForce(Collider target, Vector3 point, Vector3 origin, bool isMagic)
        {
             Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
             if (targetRigidbody != null)
             {
                 Vector3 pushDirection = (point - origin).normalized;
                 targetRigidbody.AddForceAtPosition(pushDirection * (isMagic ? 10f : 5f), point, ForceMode.Impulse);
             }
        }
        
        private void ApplyAreaDamage(Vector3 center, float damage, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, _targetLayerMask);
            foreach (Collider hitCollider in hitColliders)
            {
                hitCollider.GetComponent<IDamageable>()?.TakeDamage(damage);
            }
        }

        private void SpawnMagicVFXAtSword()
        {
            if (_magicSlashVFX == null) return;
            Transform spawnPoint = _magicAttackPoint ?? transform;
            Instantiate(_magicSlashVFX, spawnPoint.position, spawnPoint.rotation);
        }
    }
}