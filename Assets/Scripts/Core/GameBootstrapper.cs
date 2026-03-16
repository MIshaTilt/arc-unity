using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Добавлено для работы со сценами
using Scripts.Services;
using Scripts.AI;
using Scripts.MVC; // Добавлено для доступа к HealthController

namespace Scripts
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Global Settings")]
        [SerializeField] private InputActionAsset _inputAsset;
        
        [Header("Scene References")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAttacks _playerAttacks;
        [SerializeField] private EnemyAI[] _enemiesOnScene;

        private StandaloneInputService _inputService;

        private void Awake()
        {
            // 1. Инициализация сервисов
            _inputService = new StandaloneInputService(_inputAsset);

            // 2. Внедрение зависимостей
            _playerMovement.Construct(_inputService);
            _playerAttacks.Construct(_inputService);

            // 3. Раздаем цели врагам
            foreach (var enemy in _enemiesOnScene)
            {
                if (enemy != null)
                {
                    enemy.Construct(_playerMovement.transform);
                }
            }

            // 4. Подписываемся на смерть игрока для перезагрузки сцены
            HealthController playerHealth = _playerMovement.GetComponent<HealthController>();
            if (playerHealth != null)
            {
                // Слушаем событие смерти из HealthController
                playerHealth.OnDeathEvent.AddListener(OnPlayerDied);
            }
        }

        private void OnPlayerDied()
        {
            Debug.Log("Игрок умер. Игра остановлена.");
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            _inputService?.Dispose();
        }
    }
}