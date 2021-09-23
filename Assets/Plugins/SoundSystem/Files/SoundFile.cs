using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SoundSystem.Files
{
    /// <summary>
    /// Base class for custom sound files.
    /// </summary>
    [Serializable]
    public abstract class SoundFile
    {
        #region --- [PROPERTIES] ---
        public AudioClip Clip => clip;
        
        /// <summary>
        /// Value between -3 and 3 
        /// </summary>
        public float Pitch
        {
            get => pitch;
            set => pitch = Mathf.Clamp(value, -3f, 3f);
        }
        
        /// <summary>
        /// Value between -3 and 3 
        /// </summary>
        public bool RandomizePitch
        {
            get => randomizePitch;
            set => randomizePitch = value;
        }
        
        public Vector2 RandomizedPitch
        {
            get => randomizedPitch;
            set => randomizedPitch = new Vector2(Mathf.Clamp(value.x, -3f, 3f), Mathf.Clamp(value.y, -3f, 3f));
        }

        /// <summary>
        /// Value between 0 and 1 
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp(value, 0f, 1f);
        }
        
        /// <summary>
        /// When enabled the clip will be played as a non blocking one shot.
        /// </summary>
        public bool PlayOneShot
        {
            get => playOneShot;
            set => playOneShot = value;
        }
        
        public bool Loop
        {
            get => loop;
            set => loop = value;
        }
        
        /// <summary>
        /// When enabled the clip will be queued if another clip is currently playing.
        /// </summary>
        public bool Queue
        {
            get => queue;
            set => queue = value;
        }
        
        /// <summary>
        /// When enabled the clip will be played event if another clip is already being played.
        /// </summary>
        public bool ForcePlay
        {
            get => forcePlay;
            set => forcePlay = value;
        }

        #endregion
        
        [Required] [SerializeField] [HorizontalGroup("File", LabelWidth = 30)]
        private AudioClip clip = null;

        [SerializeField] [HorizontalGroup("File", LabelWidth = 30)]
        internal bool edit = false;

        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [Range(-3f, 3f)]
        private float pitch = 1f;
        
        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField]
        private bool randomizePitch = false;
        
        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [ShowIf("@randomizePitch == true")]
        [MinMaxSlider(-3f, 3f)] private Vector2 randomizedPitch = new Vector2(.5f,1.5f);
        
        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [Range(0f, 1f)]
        protected float volume = 1f;
        
        [ShowIfGroup("Settings", Condition = "edit")] [EnableIf("@enableOneShot == true")]
        [SerializeField] private protected bool playOneShot = true;

        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [ShowIf("@playOneShot == false")]
        private bool loop = false;
        
        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [ShowIf("@playOneShot == false")]
        private bool queue = false;
        
        [ShowIfGroup("Settings", Condition = "edit")] [SerializeField] [ShowIf("@playOneShot == false")]
        private bool forcePlay = false;
        
        
        //--------------------------------------------------------------------------------------------------------------

        protected abstract bool enableTransition { get; }
        protected abstract bool enableOneShot { get; }
    }
}