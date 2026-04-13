namespace Scripts.Save.DTO
{
    /// <summary>
    /// Полный снимок сохранения: игрок + все враги + текущая сцена.
    /// Поле id — встроенный primary key PocketBase.
    /// </summary>
    [System.Serializable]
    public class GameSaveData
    {
        public string id;
        public string sceneName;
        public string timestamp;
        public PlayerPositionData playerPosition;
        public PlayerStateData playerState;
        public EnemySaveData[] enemies;
    }
}
