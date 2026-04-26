namespace Scripts.Services
{
    public interface IGameSessionService
    {
        bool IsPeacefulMode { get; set; }
    }

    public class GameSessionService : IGameSessionService
    {
        public bool IsPeacefulMode { get; set; } = false;
    }
}