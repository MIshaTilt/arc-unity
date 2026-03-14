using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class PlayerAttacks : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float _physicalDamage = 10f;
        [SerializeField] private float _magicDamage = 25f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private InputActionAsset _inputAsset;

        private InputAction _physicalAttackAction;
        private InputAction _magicAttackAction;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            Debug.Log("a");

            if (_inputAsset != null)
            {
                var playerMap = _inputAsset.FindActionMap("Player");
                if (playerMap != null)
                {
                    _physicalAttackAction = playerMap.FindAction("Attack");
                    _magicAttackAction = playerMap.FindAction("MagicAttack");
                }
            }
        }

        private void OnEnable()
        {
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
            PerformPhysicalAttack();
        }

        private void OnMagicAttack(InputAction.CallbackContext context)
        {
            PerformMagicAttack();
        }

        private void PerformPhysicalAttack()
        {
            // Физическая атака - Raycast из центра экрана
            Vector3 rayOrigin = _mainCamera != null ? _mainCamera.transform.position : transform.position;
            Vector3 rayDirection = _mainCamera != null ? _mainCamera.transform.forward : transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _attackRange, _targetLayerMask))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_physicalDamage);
                }

                // Применяем физический толчок к цели
                Rigidbody targetRigidbody = hit.collider.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    Vector3 pushDirection = (hit.point - rayOrigin).normalized;
                    targetRigidbody.AddForceAtPosition(pushDirection * 5f, hit.point, ForceMode.Impulse);
                }
            }
        }

        private void PerformMagicAttack()
        {
            // Магическая атака - Raycast с возможным уроном по площади
            Vector3 rayOrigin = _mainCamera != null ? _mainCamera.transform.position : transform.position;
            Vector3 rayDirection = _mainCamera != null ? _mainCamera.transform.forward : transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _attackRange, _targetLayerMask))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_magicDamage);
                }

                // Магический взрыв - урон по области вокруг точки попадания
                ApplyAreaDamage(hit.point, _magicDamage * 0.5f, 3f);
            }
            else
            {
                // Если не попали - магический снаряд летит на максимальную дистанцию
                Vector3 impactPoint = rayOrigin + rayDirection * _attackRange;
                ApplyAreaDamage(impactPoint, _magicDamage * 0.5f, 3f);
            }
        }

        private void ApplyAreaDamage(Vector3 center, float damage, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, _targetLayerMask);

            foreach (Collider hitCollider in hitColliders)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }

                // Применяем взрывную силу
                Rigidbody targetRigidbody = hitCollider.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    Vector3 explosionForce = (hitCollider.transform.position - center).normalized;
                    targetRigidbody.AddForce(explosionForce * 10f, ForceMode.Impulse);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Получаем позицию и направление для атак
            Camera cam = _mainCamera != null ? _mainCamera : Camera.main;
            Vector3 rayOrigin = cam != null ? cam.transform.position : transform.position;
            Vector3 rayDirection = cam != null ? cam.transform.forward : transform.forward;

            // Луч атаки (красный)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * _attackRange);
            
            // Сфера в конце луча (точка попадания)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rayOrigin + rayDirection * _attackRange, 0.3f);
            
            // Позиция начала атаки (желтая сфера)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rayOrigin, 0.2f);
        }
    }

}
