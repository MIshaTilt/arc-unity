using UnityEngine;
using UnityEngine.UI;
using Scripts;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private Slider _healthSlider;

    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0f);

        UpdateHealthBar();

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _currentHealth / _maxHealth;
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
