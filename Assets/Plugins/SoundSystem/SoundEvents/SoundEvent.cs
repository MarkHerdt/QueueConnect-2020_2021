using System;
using Sirenix.OdinInspector;
using SoundSystem.Core;
using UnityEngine;

namespace SoundSystem.SoundEvents
{
    /// <summary>
    /// Base class for Sound Events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class SoundEvent<T> where T : unmanaged
    {
        #region --- [INSPECTOR] ---
        
        [FoldoutGroup("@title")]
        [PropertyOrder(0)]
        [HideLabel, PropertySpace(SpaceAfter = 10)]
        [SerializeField] protected T eventID = (T)(object)0;
        
        private string title => eventID.ToString();
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [DEBUG] ---
#if UNITY_EDITOR
        [Button][PropertyOrder(1)]
        [HorizontalGroup("@title/Misc", LabelWidth = 40)]
        protected abstract void Preview();

        [Button][PropertyOrder(1)]
        [HorizontalGroup("@title/Misc", LabelWidth = 40)]
        private void Stop() => AudioPlayer.Instance.StopAllAudioClips();

        [Button][PropertyOrder(1)]
        [HorizontalGroup("@title/Misc", LabelWidth = 40)]
        private void Save() => AudioSystem.Init();
#endif
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [PROPERTIES] ---

        /// <summary>
        /// Unique identifier. This id is the value of the events enum type and is used as the key to access the event.
        /// </summary>
        public int ID => (int)(object)eventID;

        #endregion
        
    }
}