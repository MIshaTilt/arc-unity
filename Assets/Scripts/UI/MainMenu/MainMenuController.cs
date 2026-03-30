using UnityEngine.SceneManagement;
using Scripts.Services;

namespace Scripts.UI.MainMenu
{
    public class MainMenuController
    {
        private readonly IAudioService _audioService;

        // Внедрение зависимостей 
        public MainMenuController(MainMenuView view, IAudioService audioService)
        {
            _audioService = audioService;

            // Подписка на события вьюхи
            view.OnPlayClicked += StartGame;
            view.OnVolumeChanged += ChangeVolume;

            // Устанавливаем ползунок в актуальное положение
            view.SetVolumeSlider(_audioService.Volume);
        }

        private void StartGame() => SceneManager.LoadScene("GameplayScene");
        private void ChangeVolume(float volume) => _audioService.SetVolume(volume);
    }
}