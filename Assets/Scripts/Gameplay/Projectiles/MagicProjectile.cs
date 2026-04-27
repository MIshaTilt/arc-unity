using UnityEngine;
using Scripts;

public class MagicProjectile : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float _damage = 20f; // Значение по умолчанию, будет перезаписано
    [SerializeField] private float _lifetime = 3f;

    // Метод для передачи урона из ScriptableObject
    public void SetDamage(float newDamage)
    {
        _damage = newDamage;
    }

    private void Start()
    {
        // Уничтожаем файербол через заданное время, если он никуда не попал
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что столкнулись с игроком
        if (!other.CompareTag("Player")) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
            
            // Уничтожаем снаряд после нанесения урона
            Destroy(gameObject);
        }
    }
}