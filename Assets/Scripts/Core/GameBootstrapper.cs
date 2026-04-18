using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Scripts.Services;
using Scripts.AI;
using Scripts.MVC;
using Scripts.Architecture;
using Scripts.UI.PauseMenu;
using Scripts.Save;
using Scripts.Save.DTO;
using Scripts.Save.Repository;
using Scripts.Save.Interactor;
using Scripts.Save.Domain; 

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
        [SerializeField] private string[] _enemySaveIds; // ID врагов для сохранения (задаётся в инспекторе)

        [Header("UI")]
        [SerializeField] private PauseMenuView _pauseView;

        [Header("Save Settings")]
        [SerializeField] private bool _usePocketBase = false; // Переключатель: PocketBase или заглушка
        [SerializeField] private PocketBaseConfig _pocketBaseConfig;

        private StandaloneInputService _inputService;
        private PauseMenuController _pauseController;
        private ISaveService _saveService;

        private void Awake()
        {
            // 1. Инициализация сервисов
            _inputService = new StandaloneInputService(_inputAsset);

            // 2. Инициализация системы сохранения
            InitializeSaveSystem();

            // 3. Внедрение зависимостей
            _playerMovement.Construct(_inputService);
            _playerAttacks.Construct(_inputService);
            _playerMovement.SetPlayerAttacks(_playerAttacks); // Для чтения кулдаунов

            // 4. Раздаем цели врагам и назначаем ID для сохранения
            for (int i = 0; i < _enemiesOnScene.Length; i++)
            {
                var enemy = _enemiesOnScene[i];
                if (enemy != null)
                {
                    enemy.Construct(_playerMovement.transform);

                    // Назначаем ID для сохранения (из инспектора или авто-генерация)
                    if (i < _enemySaveIds.Length && !string.IsNullOrEmpty(_enemySaveIds[i]))
                    {
                        enemy.SetSaveId(_enemySaveIds[i]);
                    }
                }
            }

            // 5. Подписываемся на смерть игрока
            HealthController playerHealth = _playerMovement.GetComponent<HealthController>();
            if (playerHealth != null)
            {
                playerHealth.OnDeathEvent.AddListener(OnPlayerDied);
            }
        }

        /// <summary>
        /// Инициализирует систему сохранения: репозиторий → интерактор → сервис.
        /// </summary>
        private void InitializeSaveSystem()
        {
            var saveableEntities = new List<IEntitySaveable>();
            foreach (var enemy in _enemiesOnScene)
            {
                if (enemy != null) saveableEntities.Add(enemy);
            }

            IPlayerSaveable playerSaveable = _playerMovement;

            if (_usePocketBase)
            {
                // Создаем конфиги для разных коллекций (у PocketBaseRepository один конфиг на инстанс)
                var metaConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                var playerConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                var enemyConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                
                // 1. Создаем гранулярные репозитории
                IGameMetaRepository metaRepo = new PocketBaseMetaRepository(metaConfig);
                IPlayerRepository playerRepo = new PocketBasePlayerRepository(playerConfig);
                IEnemyRepository enemyRepo = new PocketBaseEnemyRepository(enemyConfig);
                
                // 2. Передаем их в оркестраторы (Интеракторы)
                var saveInteractor = new SaveInteractor(
                    metaRepo, playerRepo, enemyRepo, saveableEntities, playerSaveable);
                    
                var loadInteractor = new LoadInteractor(
                    metaRepo, playerRepo, enemyRepo, saveableEntities, playerSaveable);
                
                // 3. Сервис использует Интеракторы (Фасад для UI)
                _saveService = new PocketBaseSaveService(saveInteractor, loadInteractor);

                Debug.Log($"[GameBootstrapper] Используется PocketBase: {_pocketBaseConfig.BaseUrl}");
            }
            else
            {
                _saveService = new PlayerPrefsSaveService();
                Debug.Log("[GameBootstrapper] Используется заглушка сохранения.");
            }

            ServiceLocator.Register<ISaveService>(_saveService);
        }





        // Для инициализации паузы
        private void Start()
        {
            // Собираем MVC для паузы
            _pauseController = new PauseMenuController(_pauseView, _saveService);

            // Подписываемся на ввод
            _inputService.OnPauseToggle += _pauseController.TogglePause;
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
            if (_inputService != null && _pauseController != null)
            {
                _inputService.OnPauseToggle -= _pauseController.TogglePause;
            }
            _inputService?.Dispose();
        }
    }
}