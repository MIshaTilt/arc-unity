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
        
        [Header("Events")]
        public UnityEvent OnDeathEvent; // Чтобы подписывать другие скрипты в инспекторе

        private HealthModel _healthModel;
        public bool IsDead => _healthModel != null && _healthModel.IsDead;

        private void Awake()
        {
            _healthModel = new HealthModel(_maxHealth);
            
            // Подписка View на Model
            if (_healthView != null)
            {
                _healthModel.OnHealthChanged += _healthView.UpdateHealth;
                _healthView.UpdateHealth(1f); // Инит
            }

            _healthModel.OnDamaged += PlayHitAnimation;
            _healthModel.OnDied += Die;
        }

        public void TakeDamage(float damage)
        {
            _healthModel.TakeDamage(damage);
        }

        private void PlayHitAnimation()
        {
            if (_animator != null) _animator.SetTrigger("TakeHit");
        }

        private void Die()
        {
            if (_animator != null) _animator.SetTrigger("DeathTrigger");
            if (_deathEffect != null) Instantiate(_deathEffect, transform.position, transform.rotation);
            
            OnDeathEvent?.Invoke(); // Сообщаем другим скриптам (например, контроллерам движения), что мы мертвы
        }
    }
}