using UnityEngine;
using Scripts.Architecture;
using Scripts.Services;
using Scripts.UI.MainMenu;

namespace Scripts.Core
{
    public class MainMenuBootstrapper : MonoBehaviour
    {
        [SerializeField] private MainMenuView _mainMenuView;
        
        private MainMenuController _controller;

        private void Start()
        {
            IAudioService audioService = ServiceLocator.Get<IAudioService>();
            IGameSessionService sessionService = ServiceLocator.Get<IGameSessionService>();

            // Собираем MVC
            _controller = new MainMenuController(_mainMenuView, audioService, sessionService);
        }
    }
}