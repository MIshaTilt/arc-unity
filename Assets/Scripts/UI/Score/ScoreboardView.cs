using UnityEngine;
using TMPro;
using Scripts.Systems.Score;

namespace Scripts.UI.Score
{
    public class ScoreboardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        private ScoreSystem _scoreSystem;

        // Метод для инъекции зависимости
        public void Initialize(ScoreSystem scoreSystem)
        {
            _scoreSystem = scoreSystem;
            // Подписываемся на изменения
            _scoreSystem.OnKillCountChanged += UpdateUI;
            _scoreSystem.OnScoreLoaded += UpdateUI;
            
            // Инициализируем стартовое значение
            UpdateUI(_scoreSystem.KillCount);
        }

        private void UpdateUI(int newScore)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Убито врагов: {newScore}";
            }
        }

        private void OnDestroy()
        {
            // Обязательно отписываемся, чтобы избежать утечек памяти
            if (_scoreSystem != null)
            {
                _scoreSystem.OnKillCountChanged -= UpdateUI;
                _scoreSystem.OnScoreLoaded -= UpdateUI;
            }
        }
    }
}