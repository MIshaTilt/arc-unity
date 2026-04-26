using UnityEngine;

namespace Scripts.Services
{
    public interface IAudioService
    {
        float Volume { get; }
        void SetVolume(float volume);
        void PlayMusic(AudioClip clip); // Добавили метод
    }


    public class UnityAudioService : IAudioService
    {
        private AudioSource _musicSource;

        public float Volume => PlayerPrefs.GetFloat("GlobalVolume", 1f);

        public UnityAudioService()
        {
            // Создаем невидимый объект для проигрывания звуков
            GameObject audioObj = new GameObject("GlobalAudioSource");
            GameObject.DontDestroyOnLoad(audioObj);
            _musicSource = audioObj.AddComponent<AudioSource>();
            _musicSource.volume = Volume;
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            _musicSource.volume = volume;
            PlayerPrefs.SetFloat("GlobalVolume", volume);
            PlayerPrefs.Save();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip != null)
            {
                _musicSource.clip = clip;
                _musicSource.Play();
            }
        }
    }

}