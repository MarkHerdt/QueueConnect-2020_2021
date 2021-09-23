using System;
using SoundSystem.Core;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class MusicFile : SoundFile
    {
        protected override bool enableTransition => true;
        protected override bool enableOneShot => false;

        protected MusicFile()
        {
            playOneShot = false;
        }
    }
}