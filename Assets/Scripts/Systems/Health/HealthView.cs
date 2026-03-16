// HealthView.cs (Представление - только UI)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scripts.MVC
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;

        public void UpdateHealth(float normalizedHealth, float currentHealth)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = normalizedHealth;
            }

            if (_healthText != null)
            {
                _healthText.text = Mathf.CeilToInt(currentHealth).ToString();
            }
        }
    }
}