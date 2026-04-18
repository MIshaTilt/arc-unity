using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using Scripts.Save.DTO;

namespace Scripts.Save.Repository
{
    /// <summary>
    /// Адаптер: переводит доменные данные в формат DTO для PocketBase и наоборот.
    /// </summary>
    public class PocketBaseGameSaveRepository : IGameSaveRepository
    {
        private readonly PocketBaseRepository<GameSaveData> _apiClient;

        public PocketBaseGameSaveRepository(PocketBaseConfig config)
        {
            // Используем обобщенный репозиторий как драйвер для связи с БД
            _apiClient = new PocketBaseRepository<GameSaveData>(config);
        }

        public async Task<bool> SaveStateAsync(GameStateSnapshot snapshot)
        {
            // Маппинг: Domain -> Database DTO
            var saveData = new GameSaveData
            {
                id = snapshot.SaveId,
                sceneName = snapshot.SceneName,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Специфика БД
                playerPosition = snapshot.PlayerPosition,
                playerState = snapshot.PlayerState
            };

            // Маппинг формата врагов (EntitySaveData -> EnemySaveData)
            var enemyList = new List<EnemySaveData>();
            foreach (var enemy in snapshot.Enemies)
            {
                enemyList.Add(new EnemySaveData
                {
                    id = enemy.id,
                    enemyType = enemy.entityType,
                    positionX = enemy.positionX,
                    positionY = enemy.positionY,
                    positionZ = enemy.positionZ,
                    rotationY = enemy.rotationY,
                    currentHealth = enemy.currentHealth,
                    maxHealth = enemy.maxHealth,
                    isAlive = enemy.isAlive
                });
            }
            saveData.enemies = enemyList.ToArray();

            // Сохранение через драйвер базы данных
            bool exists = await _apiClient.ExistsAsync(saveData.id);
            if (exists)
                await _apiClient.UpdateAsync(saveData.id, saveData);
            else
                await _apiClient.CreateAsync(saveData);

            return true;
        }

        public async Task<GameStateSnapshot> LoadStateAsync(string saveId)
        {
            var saveData = await _apiClient.GetByIdAsync(saveId);
            if (saveData == null) return null;

            // Маппинг: Database DTO -> Domain
            var snapshot = new GameStateSnapshot
            {
                SaveId = saveData.id,
                SceneName = saveData.sceneName,
                PlayerPosition = saveData.playerPosition,
                PlayerState = saveData.playerState,
                Enemies = new List<EntitySaveData>()
            };

            if (saveData.enemies != null)
            {
                foreach (var enemy in saveData.enemies)
                {
                    snapshot.Enemies.Add(new EntitySaveData
                    {
                        id = enemy.id,
                        entityType = enemy.enemyType,
                        positionX = enemy.positionX,
                        positionY = enemy.positionY,
                        positionZ = enemy.positionZ,
                        rotationY = enemy.rotationY,
                        currentHealth = enemy.currentHealth,
                        maxHealth = enemy.maxHealth,
                        isAlive = enemy.isAlive
                    });
                }
            }

            return snapshot;
        }
    }
}