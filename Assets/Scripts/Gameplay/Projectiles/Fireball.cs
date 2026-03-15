using UnityEngine;
using Scripts;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _lifetime = 3f;

    private void Start()
    {
        // Уничтожаем файербол через заданное время
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что столкнулись с игроком
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }
    }
}
