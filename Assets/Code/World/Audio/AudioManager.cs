using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Wrapper
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private GameObject audioSourceContainer;
        [Space(15)]
        [SerializeField] private Audio[] audioArray;

        private Audio nowPlaying;

        #region Setup

        private void Awake()
        {
            GenerateAudioSources();
        }

        private void OnEnable()
        {
            Events.PlayMusic += PlayMusic;
            Events.StopMusic += StopMusic;
            Events.PlaySound += PlayOneShot;
        }

        private void OnDisable()
        {
            Events.PlayMusic -= PlayMusic;
            Events.StopMusic -= StopMusic;
            Events.PlaySound -= PlayOneShot;
        }

        private void GenerateAudioSources()
        {
            foreach (Audio audio in audioArray)
            {
                audio.source = audioSourceContainer.AddComponent<AudioSource>();

                audio.source.clip = audio.clip;
                audio.source.playOnAwake = false;
                audio.source.volume = audio.volume;
                audio.source.loop = audio.type == AudioType.Music;
            }
        }

        #endregion

        #region Play/Stop

        private void PlayMusic(string name)
        {
            StopMusic();

            Audio audio = GetAudio(name);
            if (audio == null)
            {
                Debug.LogFormat("Could not find audio by the name {0}", name);
                return;
            }

            nowPlaying = audio;
            try 
            {
                audio.source.Play();
            }
            catch (Exception) { }
        }

        private void StopMusic()
        {
            if (nowPlaying == null)
                return;

            nowPlaying.source.Stop();
            nowPlaying = null;
        }

        private void PlayOneShot(string name)
        {
            Audio audio = GetAudio(name);
            if (audio == null)
            {
                Debug.LogFormat("Could not find audio by the name {0}", name);
                return;
            }

            try
            {
                audio.source.PlayOneShot(audio.clip);
            }
            catch (Exception) { }
        }

        #endregion

        private Audio GetAudio(string name)
        {
            return Array.Find(audioArray, audio => Format(audio.name) == Format(name));
        }

        private string Format(string input)
        {
            return input.Trim();
        }
    }
}
