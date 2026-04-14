// PlayerAttacks.cs
using UnityEngine;
using UnityEngine.UI;
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

        [Header("Cooldowns")]
        [SerializeField] private float _physicalCooldown = 1.5f;
        [SerializeField] private float _magicCooldown = 5f;
        [SerializeField] private Image _magicCooldownIndicator;

        private IInputService _inputService;
        private Camera _mainCamera;
        private HealthController _healthController;

        private float _physicalCooldownTimer;
        private float _magicCooldownTimer;
        private bool _isPhysicalReady => _physicalCooldownTimer <= 0f;
        private bool _isMagicReady => _magicCooldownTimer <= 0f;

        /// <summary>
        /// Публичный доступ к таймерам кулдаунов (для сохранения).
        /// </summary>
        public float PhysicalCooldownTimer => _physicalCooldownTimer;
        public float MagicCooldownTimer => _magicCooldownTimer;

        // DI Внедрение
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
            _inputService.OnPhysicalAttack += OnPhysicalAttack;
            _inputService.OnMagicAttack += OnMagicAttack;
        }

        /// <summary>
        /// Устанавливает таймеры кулдаунов (для загрузки из сохранения).
        /// </summary>
        public void SetCooldowns(float physicalTimer, float magicTimer)
        {
            _physicalCooldownTimer = physicalTimer;
            _magicCooldownTimer = magicTimer;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _healthController = GetComponent<HealthController>();
        }

        private void Update()
        {
            if (_physicalCooldownTimer > 0f)
                _physicalCooldownTimer -= Time.deltaTime;

            if (_magicCooldownTimer > 0f)
            {
                _magicCooldownTimer -= Time.deltaTime;
                UpdateMagicIndicator();
            }
        }

        private void UpdateMagicIndicator()
        {
            if (_magicCooldownIndicator != null)
            {
                _magicCooldownIndicator.color = _isMagicReady ? Color.green : Color.red;
            }
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
            if (!_isPhysicalReady) return;

            _physicalCooldownTimer = _physicalCooldown;
            _animator?.SetTrigger("AttackSword");
            PerformAttack(_physicalDamage, false);
        }

        private void OnMagicAttack()
        {
            if (_healthController != null && _healthController.IsDead) return;
            if (!_isMagicReady) return;

            _magicCooldownTimer = _magicCooldown;
            _animator?.SetTrigger("AttackMagic");
            PerformAttack(_magicDamage, true);
        }

        private void PerformAttack(float damage, bool isMagic)
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = transform.forward;

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