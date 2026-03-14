using UnityEngine;
using Scripts;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private GameObject _deathEffect;

    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0f);
        Debug.Log("TakeDamage");

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_deathEffect != null)
        {
            Instantiate(_deathEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
