using System;
using SoundSystem.Core;
using UnityEngine;

namespace SoundSystem.Files
{
    [Serializable]
    public class VFXFile : SoundFile
    {
        protected override bool enableTransition => false;
        protected override bool enableOneShot => true;
    }
}