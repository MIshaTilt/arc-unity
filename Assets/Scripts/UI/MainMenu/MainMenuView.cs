using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _mainPanel;     
        [SerializeField] private GameObject _settingsPanel; 

        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _backButton;       
        
        [Header("Controls")]
        [SerializeField] private Slider _volumeSlider;

        public event Action OnPlayClicked;
        public event Action<float> OnVolumeChanged;

        private void Awake()
        {
            _playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());

            _settingsButton.onClick.AddListener(() => 
            {
                _mainPanel.SetActive(false);
                _settingsPanel.SetActive(true);
            });

             _backButton.onClick.AddListener(() => 
            {
                _settingsPanel.SetActive(false);
                _mainPanel.SetActive(true);
            });

             _volumeSlider.onValueChanged.AddListener(val => OnVolumeChanged?.Invoke(val));

            _mainPanel.SetActive(true);
            _settingsPanel.SetActive(false); 
        }

        public void SetVolumeSlider(float value) => _volumeSlider.value = value;
    }
}