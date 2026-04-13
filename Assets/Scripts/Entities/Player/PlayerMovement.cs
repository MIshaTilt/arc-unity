// PlayerMovement.cs
using UnityEngine;
using Scripts.Services;
using Scripts.MVC;
using Scripts.Save;
using Scripts.Save.DTO;

namespace Scripts
{
    public class PlayerMovement : MonoBehaviour, IEntitySaveable
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 10f;

        [Header("Look Settings")]
        [SerializeField] private float _mouseSensitivity = 1f;
        [SerializeField] private float _rotationSmoothTime = 0.05f;
        [SerializeField] private float _minVerticalAngle = -80f;
        [SerializeField] private float _maxVerticalAngle = 80f;
        [SerializeField] private Transform _cameraTransform;

        [SerializeField] private Animator _animator;
        [SerializeField] private float _groundCheckDistance = 0.2f;

        private Rigidbody _rigidbody;
        private HealthController _healthController; // Замена PlayerDamage
        private PlayerAttacks _playerAttacks; // Ссылка на атаки для получения кулдаунов
        private IInputService _inputService;

        private float _targetYRotation;
        private float _currentYRotation;
        private float _rotationVelocity;
        private float _targetXRotation;
        private float _currentXRotation;
        private bool _isGrounded;

        #region IEntitySaveable

        public string SaveId => "player";
        public string EntityType => "Player";

        /// <summary>
        /// Сериализовать состояние игрока (позиция + данные из extraData).
        /// </summary>
        public EntitySaveData CaptureState()
        {
            var healthCtrl = GetComponent<HealthController>();
            float currentHealth = healthCtrl != null ? healthCtrl.CurrentHealth : 100f;
            float maxHealth = healthCtrl != null ? healthCtrl.MaxHealth : 100f;

            var stateData = new PlayerStateData
            {
                currentHealth = currentHealth,
                maxHealth = maxHealth,
                isDead = healthCtrl != null && healthCtrl.IsDead,
                physicalCooldownTimer = _playerAttacks != null ? _playerAttacks.PhysicalCooldownTimer : 0f,
                magicCooldownTimer = _playerAttacks != null ? _playerAttacks.MagicCooldownTimer : 0f
            };

            return new EntitySaveData
            {
                id = SaveId,
                entityType = EntityType,
                positionX = transform.position.x,
                positionY = transform.position.y,
                positionZ = transform.position.z,
                rotationY = transform.eulerAngles.y,
                currentHealth = currentHealth,
                maxHealth = maxHealth,
                isAlive = healthCtrl != null && !healthCtrl.IsDead,
                extraData = JsonUtility.ToJson(stateData)
            };
        }

        /// <summary>
        /// Восстановить состояние игрока из сохранения.
        /// </summary>
        public void RestoreState(EntitySaveData data)
        {
            // Восстанавливаем позицию
            transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
            transform.rotation = Quaternion.Euler(0f, data.rotationY, 0f);

            // Сбрасываем целевые углы камеры
            _targetYRotation = data.rotationY;
            _currentYRotation = data.rotationY;

            // Восстанавливаем здоровье
            var healthCtrl = GetComponent<HealthController>();
            if (healthCtrl != null)
            {
                healthCtrl.SetHealth(data.currentHealth);
            }

            // Восстанавливаем кулдауны
            if (_playerAttacks != null && !string.IsNullOrEmpty(data.extraData))
            {
                var stateData = JsonUtility.FromJson<PlayerStateData>(data.extraData);
                _playerAttacks.SetCooldowns(stateData.physicalCooldownTimer, stateData.magicCooldownTimer);
            }

            Debug.Log("[PlayerMovement] Состояние игрока восстановлено.");
        }

        #endregion

        // DI Внедрение зависимости
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        /// <summary>
        /// Устанавливает ссылку на PlayerAttacks (для чтения кулдаунов).
        /// Вызывается из GameBootstrapper.
        /// </summary>
        public void SetPlayerAttacks(PlayerAttacks attacks)
        {
            _playerAttacks = attacks;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _healthController = GetComponent<HealthController>();

            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            _healthController.OnDeathEvent.AddListener(OnPlayerDied);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnPlayerDied()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (_healthController != null && _healthController.IsDead) return;
            if (_inputService == null) return;

            Vector2 lookInput = _inputService.LookInput;
            if (lookInput != Vector2.zero)
            {
                _targetYRotation += lookInput.x * _mouseSensitivity;
                _targetXRotation -= lookInput.y * _mouseSensitivity;
                _targetXRotation = Mathf.Clamp(_targetXRotation, _minVerticalAngle, _maxVerticalAngle);
            }

            _currentYRotation = Mathf.SmoothDamp(_currentYRotation, _targetYRotation, ref _rotationVelocity, _rotationSmoothTime);
            _currentXRotation = Mathf.Lerp(_currentXRotation, _targetXRotation, 10f * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (_healthController != null && _healthController.IsDead) return;
            if (_inputService == null) return;

            CheckGround();
            RotatePhysically();
            MovePhysically();
        }

        private void CheckGround()
        {
            Vector3 rayOrigin = _rigidbody.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, _groundCheckDistance + 0.1f);
        }

        private void RotatePhysically()
        {
            _rigidbody.MoveRotation(Quaternion.Euler(0f, _currentYRotation, 0f));
            if (_cameraTransform != null)
                _cameraTransform.localRotation = Quaternion.Euler(_currentXRotation, 0f, 0f);
        }

        private void MovePhysically()
        {
            if (!_isGrounded) return;

            Vector2 moveInput = _inputService.MoveInput;
            Vector3 forward = _rigidbody.rotation * Vector3.forward;
            Vector3 right = _rigidbody.rotation * Vector3.right;
            forward.y = 0f; right.y = 0f;

            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
            float currentSpeed = _inputService.IsSprinting ? _sprintSpeed : _moveSpeed;

            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 velocity = moveDirection * currentSpeed;
                velocity.y = _rigidbody.linearVelocity.y;
                _rigidbody.linearVelocity = velocity;
                _animator?.SetFloat("Speed", _inputService.IsSprinting ? 1f : 0.5f);
            }
            else
            {
                _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
                _animator?.SetFloat("Speed", 0f);
            }
        }
    }
}