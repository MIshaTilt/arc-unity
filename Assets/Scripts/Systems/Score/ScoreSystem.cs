using System;

namespace Scripts.Systems.Score
{
    public class ScoreSystem
    {
        public int KillCount { get; private set; }

        public event Action<int> OnKillCountChanged;
        public event Action<int> OnScoreLoaded;

        public void AddKill()
        {
            KillCount++;
            OnKillCountChanged?.Invoke(KillCount);
        }

        // Загружаем счет без триггера победных действий
        public void LoadScore(int count)
        {
            KillCount = count;
            OnScoreLoaded?.Invoke(KillCount);
        }
    }
}