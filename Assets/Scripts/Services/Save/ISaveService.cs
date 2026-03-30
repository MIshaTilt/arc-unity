using UnityEngine;

namespace Scripts.Services
{
    public interface ISaveService
    {
        void SaveGame();
        void LoadGame();
    }

       public class PlayerPrefsSaveService : ISaveService
    {
        public void SaveGame()
        {
            Debug.Log("Клик по кнопке Сохранить.");
        }

        public void LoadGame()
        {
            Debug.Log("Клик по кнопке Загрузить.");
        }
    }
}