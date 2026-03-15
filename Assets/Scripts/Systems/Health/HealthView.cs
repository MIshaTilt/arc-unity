// HealthView.cs (Представление - только UI)
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.MVC
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;

        public void UpdateHealth(float normalizedHealth)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = normalizedHealth;
            }
        }
    }
}