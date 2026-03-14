using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class PlayerDamage : MonoBehaviour, IDamageable
    {
        [Header("Player Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private GameObject _deathEffect;

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

        public void Heal(float amount)
        {
            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            UpdateHealthBar();
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

            // Перезагрузка сцены или экран смерти
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}

