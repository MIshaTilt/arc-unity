namespace Scripts.Save.Repository
{
    /// <summary>
    /// Конфигурация PocketBase сервера.
    /// </summary>
    [System.Serializable]
    public class PocketBaseConfig
    {
        public string BaseUrl = "http://localhost:8090";
        public string AdminEmail = "admin@example.com";
        public string AdminPassword = "change_me";
        public string SavesCollection = "game_saves";
        public string AuthCollection = "users";
    }
}
