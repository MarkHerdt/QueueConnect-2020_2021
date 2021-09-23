using System;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem.Core;
using SoundSystem.Files;
using UnityEngine;

namespace SoundSystem.SoundEvents
{
    [Serializable]
    public class SoundEventAmbience : SoundEvent<Ambience>
    {
        //--------------------------------------------------------------------------------------------------------------

        #region --- [IMPLEMENTATIONS] ---

        [FoldoutGroup("@title")]
        [ListDrawerSettings(Expanded = true)][PropertyOrder(100)][PropertySpace(SpaceBefore = 10)]
        [SerializeField] private AmbienceFiles[] ambienceFiles = null;

        public SoundFileGroup[] AmbienceFiles => ambienceFiles;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [EDITOR] ---
#if UNITY_EDITOR
        
        /// <summary>
        /// Method is used to play the sound event from the inspector.
        /// </summary>
        protected override void Preview() => AudioSystem.PlayAmbience(eventID);

#endif
        #endregion
    }
}