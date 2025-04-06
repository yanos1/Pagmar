using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpongeScene.Managers
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Music Clips")]
       
        [SerializeField] private AudioClip cutSceneMusic;

        [Header("Game Sounds")]
        [SerializeReference] private Dictionary<SoundName, AudioClip> soundDict = new();

        // -------- Sound Effects --------
        public void PlaySound(SoundName soundName, float volume = 1f)
        {
            if (!soundDict.TryGetValue(soundName, out var clip)) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public void PlaySoundOnSource(AudioSource source, SoundName soundName, float volume = 1f)
        {
            if (!soundDict.TryGetValue(soundName, out var clip)) return;
            source.Stop();
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }

        public void StopSoundOnSource(AudioSource source)
        {
            source.Stop();
        }

        public void StopAllSounds()
        {
            sfxSource.Stop();
            musicSource.Stop();
        }

        public void StopSoundIfPlaying(SoundName soundName, AudioSource source)
        {
            if (source.isPlaying && source.clip == soundDict[soundName])
            {
                source.Stop();
            }
        }
    }

    public enum SoundName
    {
        None = 0,
        WaterSplash = 1,
        GateOpenSlow = 2,
        GateOpenFast = 3,
        ButtonPressed = 4,
        ObjectGrow = 5,
        BoilingWater = 6,
        Gayser = 7,
        Shuriken = 8,
        Die = 9,
        WallBreak = 10,
        Token = 11,
        Upgrade = 12,
        ObjectDecrease = 13,
        WaterButton = 14,
        Pop = 15,
        CheckPoint = 16,
    }
}
