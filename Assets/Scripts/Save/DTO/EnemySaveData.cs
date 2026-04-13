namespace Scripts.Save.DTO
{
    /// <summary>
    /// Данные сохранения одного врага (вложенная структура в enemies JSON).
    /// </summary>
    [System.Serializable]
    public class EnemySaveData
    {
        public string id;
        public string enemyType;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationY;
        public float currentHealth;
        public float maxHealth;
        public bool isAlive;
    }
}
