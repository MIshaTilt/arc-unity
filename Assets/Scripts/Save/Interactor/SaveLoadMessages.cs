namespace Scripts.Save.Interactor
{
    // Запросы к интеракторам
    public class SaveGameRequest
    {
        public string SaveId;
    }

    public class LoadGameRequest
    {
        public string SaveId;
    }

    // Ответы от интеракторов
    public class SaveGameResponse
    {
        public bool Success;
        public string Message;
    }

    public class LoadGameResponse
    {
        public bool Success;
        public string Message;
    }
}