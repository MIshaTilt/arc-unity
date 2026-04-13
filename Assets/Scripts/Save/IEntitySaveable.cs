namespace Scripts.Save
{
    /// <summary>
    /// Интерфейс для сущностей, которые могут быть сохранены/загружены.
    /// Реализуется в PlayerMovement, EnemyAI и т.д.
    /// </summary>
    public interface IEntitySaveable
    {
        /// <summary>
        /// Уникальный идентификатор сущности (для сопоставления при загрузке).
        /// </summary>
        string SaveId { get; }

        /// <summary>
        /// Тип сущности (для создания правильного префаба при загрузке).
        /// </summary>
        string EntityType { get; }

        /// <summary>
        /// Сериализовать текущее состояние сущности в DTO.
        /// </summary>
        EntitySaveData CaptureState();

        /// <summary>
        /// Восстановить состояние сущности из DTO.
        /// </summary>
        void RestoreState(EntitySaveData data);
    }

    /// <summary>
    /// Универсальный DTO для любой сохраняемой сущности.
    /// Поле id — уникальный идентификатор сущности (не PK таблицы).
    /// </summary>
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
        // Дополнительные поля (кулдауны, мана и т.д.) — в наследниках или через JSON-расширение.
        public string extraData; // JSON-строка для специфичных данных
    }
}
