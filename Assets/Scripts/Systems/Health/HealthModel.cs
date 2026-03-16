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

        private float _currentHealth;
        private float _maxHealth;
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
    }
}