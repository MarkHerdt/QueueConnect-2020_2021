using System;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem.Core;
using SoundSystem.Files;
using UnityEngine;

namespace SoundSystem.SoundEvents
{
    [Serializable]
    public class SoundEventMusic : SoundEvent<Music>
    {
        #region --- [PROPERTIES] ---

        /// <summary>
        /// Ease the volume of the sounds in / out
        /// </summary>
        public bool Transition
        {
            get => transition && Application.isPlaying;
            set => transition = value;
        }
        
        /// <summary>
        /// Ease the volume of the sounds in 
        /// </summary>
        public bool FadeIn
        {
            get => fadeIn;
            set => fadeIn = value;
        }
        
        /// <summary>
        /// The Fade-In duration in seconds.
        /// </summary>
        public float FadeInDuration
        {
            get => fadeInDuration;
            set => fadeInDuration = value;
        }
        
        /// <summary>
        /// Ease the volume of the sounds out
        /// </summary>
        public bool FadeOut
        {
            get => fadeOut;
            set => fadeOut = value;
        }
        
        /// <summary>
        /// The Fade-Out duration in seconds.
        /// </summary>
        public float FadeOutDuration
        {
            get => fadeOutDuration;
            set => fadeOutDuration = value;
        }
        

        #endregion
        //--------------------------------------------------------------------------------------------------------------

        #region --- [IMPLEMENTATIONS] ---
        
        [FoldoutGroup("@title")]
        [HideLabel][PropertyOrder(100)][PropertySpace(SpaceBefore = 10)]
        [SerializeField] private MusicFiles musicFiles = null;
        public MusicFiles MusicFiles => musicFiles;
        

        
        [InfoBox("Music transitions will not be previewed in edit mode", InfoMessageType.Warning , "IsInEditMode")]
        [Tooltip("Ease the volume of the sounds in / out")]
        [BoxGroup("@title/Event Settings")] [SerializeField][PropertyOrder(2)]
        protected bool transition = false;
        
        [Tooltip("Ease the volume of the sounds in.")]
        [BoxGroup("@title/Event Settings")] [SerializeField] [EnableIf("@transition == true")][PropertyOrder(2)]
        protected bool fadeIn = false;
        
        [Tooltip("The fade In duration in seconds.")]
        [BoxGroup("@title/Event Settings")] [SerializeField] [EnableIf("@transition == true")] [Range(0f, 15f)] [ShowIf("@fadeIn == true")][PropertyOrder(2)]
        protected float fadeInDuration = 0f;
        
        [Tooltip("Ease the volume of the sounds out.")]
        [BoxGroup("@title/Event Settings")] [SerializeField] [EnableIf("@transition == true")][PropertyOrder(2)]
        protected bool fadeOut = false;
        
        [Tooltip("The fadeOut duration in seconds.")]
        [BoxGroup("@title/Event Settings")] [SerializeField] [EnableIf("@transition == true")] [Range(0f, 15f)] [ShowIf("@fadeOut == true")][PropertyOrder(2)]
        protected float fadeOutDuration = 0f;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [EDITOR] ---
#if UNITY_EDITOR
        
        /// <summary>
        /// Method is used to play the sound event from the inspector.
        /// </summary>
        protected override void Preview() =>  AudioSystem.PlayMusic(eventID);
#endif
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        private bool IsInEditMode() => !Application.isPlaying && transition;

    }
}