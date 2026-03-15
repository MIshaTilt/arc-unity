// PlayerMovement.cs
using UnityEngine;
using Scripts.Services;
using Scripts.MVC;

namespace Scripts
{
    public class PlayerMovement : MonoBehaviour
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
        private IInputService _inputService;

        private float _targetYRotation;
        private float _currentYRotation;
        private float _rotationVelocity;
        private float _targetXRotation;
        private float _currentXRotation;
        private bool _isGrounded;

        // DI Внедрение зависимости
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _healthController = GetComponent<HealthController>();

            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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