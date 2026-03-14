using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 10f;

        [Header("Look Settings")]
        [SerializeField] private float _mouseSensitivity = 1f;
        [Tooltip("Время сглаживания поворота. Чем больше, тем плавнее (и более 'вязко')")]
        [SerializeField] private float _rotationSmoothTime = 0.05f;
        [SerializeField] private float _minVerticalAngle = -80f;
        [SerializeField] private float _maxVerticalAngle = 80f;
        [Tooltip("Ссылка на камеру Cinemachine для вертикального поворота")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputAsset;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private InputAction _physicalAttackAction;
        private InputAction _magicAttackAction;
        private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;

        private Vector2 _currentMoveInput;
        private Vector2 _currentLookInput;

        // Переменные для сглаживания (горизонтальный поворот)
        private float _targetYRotation;
        private float _currentYRotation;
        private float _rotationVelocity;

        // Вертикальный поворот капсулы (для Cinemachine)
        private float _targetXRotation;
        private float _currentXRotation;

        // Проверка земли
        private bool _isGrounded;
        [SerializeField] private float _groundCheckDistance = 0.2f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // Блокируем и прячем курсор мыши для правильной работы камеры
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_inputAsset != null)
            {
                var playerMap = _inputAsset.FindActionMap("Player");
                if (playerMap != null)
                {
                    _moveAction = playerMap.FindAction("Move");
                    _lookAction = playerMap.FindAction("Look");
                    _sprintAction = playerMap.FindAction("Sprint");
                    _physicalAttackAction = playerMap.FindAction("Attack");
                    _magicAttackAction = playerMap.FindAction("MagicAttack");
                }
            }
        }

        private void OnEnable()
        {
            if (_moveAction != null) _moveAction.Enable();
            if (_lookAction != null) _lookAction.Enable();
            if (_sprintAction != null) _sprintAction.Enable();
            if (_physicalAttackAction != null)
            {
                _physicalAttackAction.Enable();
                _physicalAttackAction.performed += OnPhysicalAttack;
            }
            if (_magicAttackAction != null)
            {
                _magicAttackAction.Enable();
                _magicAttackAction.performed += OnMagicAttack;
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null) _moveAction.Disable();
            if (_lookAction != null) _lookAction.Disable();
            if (_sprintAction != null) _sprintAction.Disable();
            if (_physicalAttackAction != null)
            {
                _physicalAttackAction.Disable();
                _physicalAttackAction.performed -= OnPhysicalAttack;
            }
            if (_magicAttackAction != null)
            {
                _magicAttackAction.Disable();
                _magicAttackAction.performed -= OnMagicAttack;
            }
        }

        private void OnPhysicalAttack(InputAction.CallbackContext context)
        {
            _animator.SetTrigger("AttackSword");
        }

        private void OnMagicAttack(InputAction.CallbackContext context)
        {
            _animator.SetTrigger("AttackMagic");
        }

        private void Update()
        {
            if (_moveAction != null)
                _currentMoveInput = _moveAction.ReadValue<Vector2>();

            if (_lookAction != null)
                _currentLookInput = _lookAction.ReadValue<Vector2>();

            // 1. Считаем ЦЕЛЕВОЙ угол поворота (куда мы хотим повернуться)
            if (_currentLookInput != Vector2.zero)
            {
                _targetYRotation += _currentLookInput.x * _mouseSensitivity;
                _targetXRotation -= _currentLookInput.y * _mouseSensitivity;

                // Ограничиваем вертикальный угол
                _targetXRotation = Mathf.Clamp(_targetXRotation, _minVerticalAngle, _maxVerticalAngle);
            }

            // 2. ПЛАВНО меняем ТЕКУЩИЙ угол в сторону целевого каждый кадр
            _currentYRotation = Mathf.SmoothDamp(
                _currentYRotation,
                _targetYRotation,
                ref _rotationVelocity,
                _rotationSmoothTime
            );

            // Для вертикального угла используем простую интерполяцию
            _currentXRotation = Mathf.Lerp(_currentXRotation, _targetXRotation, 10f * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            CheckGround();
            RotatePhysically();
            MovePhysically();
        }

        private void CheckGround()
        {
            // Raycast вниз от центра Rigidbody для проверки земли
            RaycastHit hit;
            Vector3 rayOrigin = _rigidbody.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, _groundCheckDistance + 0.1f);
        }

        private void RotatePhysically()
        {
            // Горизонтальный поворот капсулы через Rigidbody (для физики)
            Quaternion targetRotation = Quaternion.Euler(0f, _currentYRotation, 0f);
            _rigidbody.MoveRotation(targetRotation);
            
            // Вертикальный поворот камеры (не капсулы!)
            if (_cameraTransform != null)
            {
                _cameraTransform.localRotation = Quaternion.Euler(_currentXRotation, 0f, 0f);
            }
        }

        private void MovePhysically()
        {
            // Движение работает только на земле
            if (!_isGrounded)
            {
                return;
            }

            Vector3 forward = _rigidbody.rotation * Vector3.forward;
            Vector3 right = _rigidbody.rotation * Vector3.right;
            forward.y = 0f;
            right.y = 0f;

            Vector3 moveDirection = (forward * _currentMoveInput.y + right * _currentMoveInput.x).normalized;

            // Определяем текущую скорость: бег или ходьба
            bool isSprinting = _sprintAction != null && _sprintAction.ReadValue<float>() > 0.5f;
            float currentSpeed = isSprinting ? _sprintSpeed : _moveSpeed;

            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 velocity = moveDirection * currentSpeed;
                velocity.y = _rigidbody.linearVelocity.y;
                _rigidbody.linearVelocity = velocity;

                // Анимация движения
                _animator.SetFloat("Speed", isSprinting ? 1f : 0.5f);
            }
            else
            {
                Vector3 velocity = _rigidbody.linearVelocity;
                velocity.x = 0f;
                velocity.z = 0f;
                _rigidbody.linearVelocity = velocity;

                // Анимация idle
                _animator.SetFloat("Speed", 0f);
            }
        }
    }
}