using UnityEngine;
using UnityEngine.UI;
using Scripts;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Animator _animator;
    [Tooltip("Ссылка на AI скрипт (MeleeWalk или RangedWalk)")]
    [SerializeField] private MonoBehaviour _enemyAI;

    private float _currentHealth;
    private bool _isDead;

    private void Start()
    {
        _currentHealth = _maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        if (_animator != null && _currentHealth > 0f)
        {
            _animator.SetTrigger("TakeHit");
        }
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
        _isDead = true;

        // Сообщаем AI скрипту о смерти
        if (_enemyAI is MeleeWalk melee)
        {
            melee.Die();
        }
        else if (_enemyAI is RangedWalk ranged)
        {
            ranged.Die();
        }

        if (_animator != null)
        {
            _animator.SetTrigger("DeathTrigger");
        }

        // Уничтожаем объект через 3 секунды (после проигрывания анимации смерти)
        Destroy(gameObject, 7f);
    }
}
