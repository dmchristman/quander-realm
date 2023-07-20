using UnityEngine;

namespace Wrapper
{
    [System.Serializable]
    public class Audio
    {
        public string name;
        public AudioType type;
        public AudioClip clip;

        [Range(0f, 1f)] 
        public float volume = 1f;
        
        [HideInInspector]
        public AudioSource source;
    }
}