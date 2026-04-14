// HealthModel.cs (Модель - только данные)
using System;
using UnityEngine;

namespace Scripts.MVC
{
    public class HealthModel
    {
        public event Action<float, float> OnHealthChanged; // (normalized, current)
        public event Action OnDied;
        public event Action OnDamaged;
        public event Action OnHealed;
        public event Action OnResurrected;

        private float _currentHealth;
        private float _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= 0;

        public HealthModel(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0f);

            OnDamaged?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth / _maxHealth, _currentHealth);

            if (IsDead) OnDied?.Invoke();
        }

        /// <summary>
        /// Устанавливает здоровье напрямую (для загрузки из сохранения).
        /// </summary>
        public void SetHealth(float health)
        {
            _currentHealth = Mathf.Clamp(health, 0f, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth / _maxHealth, _currentHealth);
        }

        /// <summary>
        /// Воскрешает персонажа с полным здоровьем.
        /// </summary>
        public void Resurrect()
        {
            _currentHealth = _maxHealth;
            OnResurrected?.Invoke();
            OnHealthChanged?.Invoke(1f, _currentHealth);
        }

        /// <summary>
        /// Восстанавливает здоровье на величину (лечение).
        /// </summary>
        public void Heal(float amount)
        {
            if (IsDead) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth / _maxHealth, _currentHealth);
        }
    }
}