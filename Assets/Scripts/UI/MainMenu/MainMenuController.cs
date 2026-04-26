using UnityEngine.SceneManagement;
using Scripts.Services;

namespace Scripts.UI.MainMenu
{
    public class MainMenuController
    {
        private readonly IAudioService _audioService;
        private readonly IGameSessionService _sessionService; // НОВОЕ

        public MainMenuController(MainMenuView view, IAudioService audioService, IGameSessionService sessionService)
        {
            _audioService = audioService;
            _sessionService = sessionService;

            view.OnPlayClicked += () => StartGame(false);
            view.OnPlayPeacefulClicked += () => StartGame(true);
            
            view.OnVolumeChanged += ChangeVolume;
            view.SetVolumeSlider(_audioService.Volume);
        }

        private void StartGame(bool isPeaceful)
        {
            _sessionService.IsPeacefulMode = isPeaceful;
            SceneManager.LoadScene("GameplayScene");
        }

        private void ChangeVolume(float volume) => _audioService.SetVolume(volume);
    }
}