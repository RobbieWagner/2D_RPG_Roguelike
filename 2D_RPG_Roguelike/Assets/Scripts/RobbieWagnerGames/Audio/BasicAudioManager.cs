using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace RobbieWagnerGames.Common
{
    public enum AudioSourceName
    {
        UINav = 0,
        UISelect = 1,
        UIExit = 2,
        Bounce = 3,
        PointGain = 4,
        LeafGain = 5,
        FlowerGain = 6,
        Purchase = 7,
        Music = 8,
        UIFail = 9
    }

    public class BasicAudioManager : MonoBehaviour
    {
        [SerializeField][SerializedDictionary("source type", "source")] private SerializedDictionary<AudioSourceName, AudioSource> audioSources;

        public static BasicAudioManager Instance {get; private set;}

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        public void PlayAudioSource(AudioSourceName name)
        {
            if(audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                audioSources[name].Play();
            }
        }

        public void StopAudioSource(AudioSourceName name)
        {
            if(audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                audioSources[name].Stop();
            }
        }
    }
}