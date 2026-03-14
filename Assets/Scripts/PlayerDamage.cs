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
        [SerializeField] private Animator _animator;

        private float _currentHealth;
        private bool _isDead;

        public bool IsDead => _isDead;

        private void Start()
        {
            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            if (_currentHealth > 0f)
            {
                _animator.SetTrigger("TakeHit");
            }

            UpdateHealthBar();

            if (_currentHealth <= 0f && enabled == true)
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
            _isDead = true;

            if (_deathEffect != null)
            {
                Instantiate(_deathEffect, transform.position, transform.rotation);
            }
            Debug.Log("Die");
            _animator.SetTrigger("DeathTrigger");

            // Отключаем управление и коллайдеры
            enabled = false;

            // Перезагрузка сцены через 3 секунды
            Invoke(nameof(ReloadScene), 5f);
        }

        private void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}

