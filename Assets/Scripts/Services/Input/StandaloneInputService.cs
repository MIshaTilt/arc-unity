// StandaloneInputService.cs
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Services
{
    public class StandaloneInputService : IInputService, IDisposable
    {
        private readonly InputAction _moveAction;
        private readonly InputAction _lookAction;
        private readonly InputAction _sprintAction;
        private readonly InputAction _physicalAttackAction;
        private readonly InputAction _magicAttackAction;

        public Vector2 MoveInput => _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public Vector2 LookInput => _lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool IsSprinting => _sprintAction?.ReadValue<float>() > 0.5f;

        public event Action OnPhysicalAttack;
        public event Action OnMagicAttack;

        public StandaloneInputService(InputActionAsset inputAsset)
        {
            var playerMap = inputAsset.FindActionMap("Player");
            if (playerMap == null) return;

            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
            _sprintAction = playerMap.FindAction("Sprint");
            _physicalAttackAction = playerMap.FindAction("Attack");
            _magicAttackAction = playerMap.FindAction("MagicAttack");

            Enable();

            _physicalAttackAction.performed += _ => OnPhysicalAttack?.Invoke();
            _magicAttackAction.performed += _ => OnMagicAttack?.Invoke();
        }

        private void Enable()
        {
            _moveAction?.Enable();
            _lookAction?.Enable();
            _sprintAction?.Enable();
            _physicalAttackAction?.Enable();
            _magicAttackAction?.Enable();
        }

        public void Dispose()
        {
            _moveAction?.Disable();
            _lookAction?.Disable();
            _sprintAction?.Disable();
            _physicalAttackAction?.Disable();
            _magicAttackAction?.Disable();
        }
    }
}