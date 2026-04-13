using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Services;

namespace Scripts.UI.PauseMenu
{
    public class PauseMenuController
    {
        private readonly PauseMenuView _view;
        private readonly ISaveService _saveService;
        private bool _isPaused;

        public PauseMenuController(PauseMenuView view, ISaveService saveService)
        {
            _view = view;
            _saveService = saveService;

            _view.OnMainMenuClicked += GoToMainMenu;
            _view.OnSaveClicked += SaveGame;
            _view.OnLoadClicked += LoadGame;
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            _view.TogglePanel(_isPaused);
            Time.timeScale = _isPaused ? 0f : 1f;

            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
        }

        private async void SaveGame()
        {
            Debug.Log("[PauseMenuController] Запуск сохранения...");
            bool success = await _saveService.SaveGameAsync();
            Debug.Log(success ? "[PauseMenuController] Сохранение успешно." : "[PauseMenuController] Сохранение провалено!");
        }

        private async void LoadGame()
        {
            Debug.Log("[PauseMenuController] Запуск загрузки...");
            string saveId = _saveService.GetLastSaveId();
            var saveData = await _saveService.LoadGameAsync(saveId);
            Debug.Log(saveData != null ? "[PauseMenuController] Загрузка успешна." : "[PauseMenuController] Сохранение не найдено!");
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}