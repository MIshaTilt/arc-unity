using UnityEngine;
using Scripts.Systems.Score;
using Scripts.Services;
using Scripts.AI;
using Scripts.Systems.Enemies;

namespace Scripts.Systems.Rules
{
    public class GameRulesController
    {
        private readonly ScoreSystem _scoreSystem;
        private readonly IAudioService _audioService;
        private readonly AudioClip _victoryMusic;
        private readonly GameObject _bossPrefab;
        private readonly Transform _bossSpawnPoint;

        private readonly Transform _playerTransform;
        private readonly EnemyRegistry _enemyRegistry;

        private bool _isBossSpawned = false;
        private bool _isMusicPlayed = false;

        public GameRulesController(
            ScoreSystem scoreSystem, 
            IAudioService audioService, 
            AudioClip victoryMusic, 
            GameObject bossPrefab, 
            Transform bossSpawnPoint,
            Transform playerTransform,
            EnemyRegistry enemyRegistry)
        {
            _scoreSystem = scoreSystem;
            _audioService = audioService;
            _victoryMusic = victoryMusic;
            _bossPrefab = bossPrefab;
            _bossSpawnPoint = bossSpawnPoint;
             _playerTransform = playerTransform;
            _enemyRegistry = enemyRegistry;

            // Подписываемся на события очков
            _scoreSystem.OnKillCountChanged += CheckRules;
            _scoreSystem.OnScoreLoaded += SyncFlagsOnLoad;
        }

        private void CheckRules(int killCount)
        {
            // Реакция на 3 убийства
            if (killCount >= 3 && !_isBossSpawned)
            {
                SpawnBoss();
                _isBossSpawned = true;
            }

            // Реакция на 5 убийств
            if (killCount >= 5 && !_isMusicPlayed)
            {
                PlayVictoryMusic();
                _isMusicPlayed = true;
            }
        }

        private void SpawnBoss()
        {
            Debug.Log("<color=red>ВНИМАНИЕ! ПОЯВИЛСЯ БОСС!</color>");
            if (_bossPrefab != null && _bossSpawnPoint != null)
            {
                GameObject bossObj = GameObject.Instantiate(_bossPrefab, _bossSpawnPoint.position, _bossSpawnPoint.rotation);
                
                var bossAI = bossObj.GetComponent<EnemyAI>();
                if (bossAI != null && _playerTransform != null)
                {
                    bossAI.Construct(_playerTransform);
                    _enemyRegistry?.Register(bossAI);
                }
            }
        }

        private void PlayVictoryMusic()
        {
            Debug.Log("<color=green>ПОБЕДА! Играет музыка!</color>");
            _audioService.PlayMusic(_victoryMusic);
        }

        private void SyncFlagsOnLoad(int killCount)
        {
            if (killCount >= 3) _isBossSpawned = true;
            if (killCount >= 5) _isMusicPlayed = true;
        }

        public void Dispose()
        {
            // Отписка при завершении игры/уровня
            _scoreSystem.OnKillCountChanged -= CheckRules;
        }
    }
}