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
            // Запрашиваем сервис у локатора 
            IAudioService audioService = ServiceLocator.Get<IAudioService>();

            // Собираем MVC
            _controller = new MainMenuController(_mainMenuView, audioService);
        }
    }
}