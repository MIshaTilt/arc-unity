using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scripts.Save.Domain;
using Scripts.Save.DTO;

namespace Scripts.Save.Repository
{
    [Serializable]
    public class EnemiesDbData 
    { 
        public string id; 
        public EnemySaveData[] enemies; 
    }

    public class PocketBaseEnemyRepository : IEnemyRepository
    {
        private readonly PocketBaseRepository<EnemiesDbData> _api;

        public PocketBaseEnemyRepository(PocketBaseConfig config)
        {
            config.SavesCollection = "enemy_saves";
            _api = new PocketBaseRepository<EnemiesDbData>(config);
        }

        public async Task<bool> SaveEnemiesAsync(string saveId, List<EntitySaveData> enemies)
        {
            var enemySaveDataList = enemies.Select(e => new EnemySaveData {
                id = e.id, enemyType = e.entityType, 
                positionX = e.positionX, positionY = e.positionY, positionZ = e.positionZ, rotationY = e.rotationY,
                currentHealth = e.currentHealth, maxHealth = e.maxHealth, isAlive = e.isAlive
            }).ToArray();

            var data = new EnemiesDbData { id = saveId, enemies = enemySaveDataList };
            
            if (await _api.ExistsAsync(data.id)) await _api.UpdateAsync(data.id, data);
            else await _api.CreateAsync(data);
            return true;
        }

        public async Task<List<EntitySaveData>> LoadEnemiesAsync(string saveId)
        {
            var data = await _api.GetByIdAsync(saveId);
            if (data?.enemies == null) return null;

            return data.enemies.Select(e => new EntitySaveData {
                id = e.id, entityType = e.enemyType,
                positionX = e.positionX, positionY = e.positionY, positionZ = e.positionZ, rotationY = e.rotationY,
                currentHealth = e.currentHealth, maxHealth = e.maxHealth, isAlive = e.isAlive
            }).ToList();
        }
    }
}