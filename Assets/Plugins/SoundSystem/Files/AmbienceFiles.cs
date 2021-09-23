using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class AmbienceFiles : SoundFileGroup
    {
        [ListDrawerSettings(Expanded = true)]
        [SerializeField] private AmbienceFile[] ambienceFiles = null;

        protected override bool enableOneShot => true;
        
        protected override void Validate()
        {
            foreach (var file in ambienceFiles)
            {
                file.Pitch = pitch;
                file.Volume = volume;
                file.PlayOneShot = playOneShot;
                file.Loop = loop;
                file.Queue = queue;
                file.ForcePlay = forcePlay;
            }
        }

        protected override SoundFile[] soundFiles => ambienceFiles;
    }
}