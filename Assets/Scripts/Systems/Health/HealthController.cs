// HealthController.cs (Контроллер - висит на игроке и врагах)
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.MVC
{
    public class HealthController : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private HealthView _healthView; // Ссылка на View
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _deathEffect;
        [SerializeField] private GameOverPanel _gameOverPanel;

        [Header("Events")]
        public UnityEvent OnDeathEvent; // Чтобы подписывать другие скрипты в инспекторе

        private HealthModel _healthModel;
        public bool IsDead => _healthModel != null && _healthModel.IsDead;
        public float CurrentHealth => _healthModel != null ? _healthModel.CurrentHealth : 0f;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _healthModel = new HealthModel(_maxHealth);

            // Подписка View на Model
            if (_healthView != null)
            {
                _healthModel.OnHealthChanged += _healthView.UpdateHealth;
                _healthView.UpdateHealth(1f, _maxHealth); // Инит
            }

            _healthModel.OnDamaged += PlayHitAnimation;
            _healthModel.OnDied += Die;
        }

        public void TakeDamage(float damage)
        {
            _healthModel.TakeDamage(damage);
        }

        /// <summary>
        /// Устанавливает текущее здоровье (для загрузки из сохранения).
        /// </summary>
        public void SetHealth(float health)
        {
            if (_healthModel == null) return;

            // Если персонаж мёртв — воскрешаем только если health > 0
            if (_healthModel.IsDead && health > 0)
            {
                _healthModel.Resurrect();
                _healthModel.SetHealth(health);
            }
            else if (!_healthModel.IsDead)
            {
                // Если жив — просто ставим HP
                _healthModel.SetHealth(health);
            }
            // Если мёртв и health <= 0 — ничего не делаем, остаётся мёртвым

            // Обновляем UI (используем MaxHealth из модели, а не из сериализованного поля)
            float normalizedHealth = _healthModel.MaxHealth > 0 
                ? _healthModel.CurrentHealth / _healthModel.MaxHealth 
                : 0f;
            _healthView?.UpdateHealth(normalizedHealth, _healthModel.CurrentHealth);
        }

        private void PlayHitAnimation()
        {
            if (_animator != null) _animator.SetTrigger("TakeHit");
        }

        private void Die()
        {
            if (_animator != null) _animator.SetTrigger("DeathTrigger");
            if (_deathEffect != null) Instantiate(_deathEffect, transform.position, transform.rotation);

            _gameOverPanel?.Show();
            OnDeathEvent?.Invoke(); // Сообщаем другим скриптам (например, контроллерам движения), что мы мертвы
        }
    }
}