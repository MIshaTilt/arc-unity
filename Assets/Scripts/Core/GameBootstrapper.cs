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
using Scripts.Systems.Score;
using Scripts.UI.Score;
using Scripts.Systems.Rules;  
using Scripts.Systems.Enemies;
using Scripts.Spawners;


namespace Scripts
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Global Settings")]
        [SerializeField] private InputActionAsset _inputAsset;

        [Header("Scene References")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAttacks _playerAttacks;

        [Header("Spawners & Enemy Data")]
        [SerializeField] private EnemySpawner[] _sceneSpawners;
        [SerializeField] private GameObject _meleePrefab;
        [SerializeField] private GameObject _rangedPrefab;

        [Header("UI")]
        [SerializeField] private PauseMenuView _pauseView;

        [Header("Save Settings")]
        [SerializeField] private bool _usePocketBase = false; // Переключатель: PocketBase или заглушка
        [SerializeField] private PocketBaseConfig _pocketBaseConfig;

        [Header("Event System & Rules")]
        [SerializeField] private ScoreboardView _scoreboardView;[SerializeField] private GameObject _bossPrefab;
        [SerializeField] private Transform _bossSpawnPoint;[SerializeField] private AudioClip _victoryMusic;

        private StandaloneInputService _inputService;
        private PauseMenuController _pauseController;
        private ISaveService _saveService;
        private ScoreSystem _scoreSystem;
        private GameRulesController _gameRulesController;
        private EnemyRegistry _enemyRegistry;

        [System.Obsolete]
        private void Awake()
        {
            // Инициализация сервисов
            _inputService = new StandaloneInputService(_inputAsset);
            IAudioService audioService = ServiceLocator.Get<IAudioService>();

            _scoreSystem = new ScoreSystem();

            if (_scoreboardView != null) 
                _scoreboardView.Initialize(_scoreSystem);

            // Создаем контроллер правил (Босс и Музыка)
            _gameRulesController = new GameRulesController(
                _scoreSystem, 
                audioService, 
                _victoryMusic, 
                _bossPrefab, 
                _bossSpawnPoint, 
                _playerMovement.transform,
                _enemyRegistry
            );

            // Инициализация системы сохранения

            _enemyRegistry = new EnemyRegistry(_playerMovement.transform, _meleePrefab, _rangedPrefab);

            // КЛЮЧЕВАЯ СВЯЗЬ: Любой зарегистрированный враг (спавн или загрузка) подписывается на очки
            _enemyRegistry.OnEnemyRegistered += (enemy) => 
            {
                HealthController health = enemy.GetComponent<HealthController>();
                if (health != null)
                {
                    health.OnDeathEvent.AddListener(() => _scoreSystem.AddKill());
                }
            };

            // 3. Собираем УЖЕ стоящих на сцене врагов (если есть)
            var staticEnemies = FindObjectsOfType<EnemyAI>();
            foreach(var enemy in staticEnemies)
            {
                enemy.Construct(_playerMovement.transform);
                _enemyRegistry.Register(enemy);
            }

            // 4. Запускаем Спавнеры
            foreach (var spawner in _sceneSpawners)
            {
                if (spawner != null)
                {
                    // Подписываем реестр на спавн новых врагов
                    spawner.OnEnemySpawned += _enemyRegistry.Register;
                    spawner.Initialize(_playerMovement.transform);
                }
            }

            // 5. Инициализация системы сохранения
            InitializeSaveSystem();

            _playerMovement.Construct(_inputService);
            _playerAttacks.Construct(_inputService);
            _playerMovement.SetPlayerAttacks(_playerAttacks);

            // Подписываемся на смерть игрока
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
            IPlayerSaveable playerSaveable = _playerMovement;

            if (_usePocketBase)
            {
                var metaConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                var playerConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                var enemyConfig = JsonUtility.FromJson<PocketBaseConfig>(JsonUtility.ToJson(_pocketBaseConfig));
                
                IGameMetaRepository metaRepo = new PocketBaseMetaRepository(metaConfig);
                IPlayerRepository playerRepo = new PocketBasePlayerRepository(playerConfig);
                IEnemyRepository enemyRepo = new PocketBaseEnemyRepository(enemyConfig);
                
                var saveInteractor = new SaveInteractor(
                    metaRepo, playerRepo, enemyRepo, _enemyRegistry, playerSaveable, _scoreSystem);
                    
                var loadInteractor = new LoadInteractor(
                    metaRepo, playerRepo, enemyRepo, _enemyRegistry, playerSaveable, _scoreSystem);

                
                _saveService = new PocketBaseSaveService(saveInteractor, loadInteractor);
            }
            else
            {
                _saveService = new PlayerPrefsSaveService();
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
            _gameRulesController?.Dispose();
        }
    }
}