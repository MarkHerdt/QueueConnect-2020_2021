using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class VFXFiles : SoundFileGroup
    {
        [ListDrawerSettings(Expanded = true)]
        [SerializeField] private VFXFile[] vfxFiles = null;

        protected override bool enableOneShot => true;
        
        protected override void Validate()
        {
            foreach (var file in vfxFiles)
            {
                file.Pitch = pitch;
                file.RandomizePitch = randomizePitch;
                file.RandomizedPitch = randomizedPitch;
                file.Volume = volume;
                file.PlayOneShot = playOneShot;
                file.Loop = loop;
                file.Queue = queue;
                file.ForcePlay = forcePlay;
            }
        }

        protected override SoundFile[] soundFiles => vfxFiles;
    }
}