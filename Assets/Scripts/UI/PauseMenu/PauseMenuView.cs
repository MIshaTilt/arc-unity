using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.PauseMenu
{
    public class PauseMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        public event Action OnMainMenuClicked;
        public event Action OnSaveClicked;
        public event Action OnLoadClicked;

        private void Awake()
        {
            _mainMenuButton.onClick.AddListener(() => OnMainMenuClicked?.Invoke());
            _saveButton.onClick.AddListener(() => OnSaveClicked?.Invoke());
            _loadButton.onClick.AddListener(() => OnLoadClicked?.Invoke());
            TogglePanel(false);
        }

        public void TogglePanel(bool isVisible) => _pausePanel.SetActive(isVisible);
    }
}