using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class MusicFiles : SoundFileGroup
    {
        [ListDrawerSettings(Expanded = true)]
        [SerializeField] private MusicFile[] musicFiles = null;


        protected override bool enableOneShot => false;

        protected override void Validate()
        {
            foreach (var file in musicFiles)
            {
                file.Pitch = pitch;
                file.RandomizePitch = randomizePitch;
                file.RandomizedPitch = randomizedPitch;
                file.Volume = volume;
                file.PlayOneShot = false;
                file.Loop = loop;
                file.Queue = queue;
                file.ForcePlay = forcePlay;
            }
        }

        protected override SoundFile[] soundFiles => musicFiles;

        protected MusicFiles()
        {
            playOneShot = false;
        }
    }
}