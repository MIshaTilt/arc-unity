using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Architecture;
using Scripts.Services;

namespace Scripts.Core
{
    public class AppBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject); // Защищаем от уничтожения

            // 1. Создаем реализации 
            IAudioService audioService = new UnityAudioService();
            ISaveService saveService = new PlayerPrefsSaveService();

            // 2. Инициализируем звук
            audioService.SetVolume(audioService.Volume);

            // 3. Регистрируем в Service Locator
            ServiceLocator.Register<IAudioService>(audioService);
            ServiceLocator.Register<ISaveService>(saveService);

            // 4. Грузим меню
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}