namespace Scripts.Save
{
    public interface IEntitySaveable
    {
        string SaveId { get; }
        string EntityType { get; }
        EntitySaveData CaptureState();
        void RestoreState(EntitySaveData data);
    }

    /// <summary>
    /// Отдельный интерфейс для игрока, оперирующий строго объектами DTO
    /// </summary>
    public interface IPlayerSaveable
    {
        Scripts.Save.DTO.PlayerPositionData CapturePosition();
        Scripts.Save.DTO.PlayerStateData CaptureState();
        void RestoreState(Scripts.Save.DTO.PlayerPositionData posData, Scripts.Save.DTO.PlayerStateData stateData);
    }

    [System.Serializable]
    public class EntitySaveData
    {
        public string id;
        public string entityType;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationY;
        public float currentHealth;
        public float maxHealth;
        public bool isAlive;
        // Поле extraData (JSON) полностью удалено!
    }
}