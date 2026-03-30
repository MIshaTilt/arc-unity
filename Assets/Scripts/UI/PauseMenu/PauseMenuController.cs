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

        private void SaveGame() => _saveService.SaveGame();
        private void LoadGame() => _saveService.LoadGame();

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}