using UnityEngine;

namespace Scripts.Services
{
    public interface IAudioService
    {
        float Volume { get; }
        void SetVolume(float volume);
    }

    public class UnityAudioService : IAudioService
    {
        public float Volume => PlayerPrefs.GetFloat("GlobalVolume", 1f);

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("GlobalVolume", volume);
            PlayerPrefs.Save();
        }
    }
}