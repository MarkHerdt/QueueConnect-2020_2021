using System;
using SoundSystem.Core;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class AmbienceFile : SoundFile
    {
        protected override bool enableTransition => true;
        protected override bool enableOneShot => true;
    }
}