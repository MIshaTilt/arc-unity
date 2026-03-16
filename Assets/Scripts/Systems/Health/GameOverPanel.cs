using UnityEngine;

namespace Scripts.MVC
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;

        private void OnEnable()
        {
            _panel?.SetActive(false);
        }

        public void Show()
        {
            _panel?.SetActive(true);
        }
    }
}
