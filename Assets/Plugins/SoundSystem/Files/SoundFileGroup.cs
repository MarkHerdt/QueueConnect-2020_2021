using System;
using Sirenix.OdinInspector;
using SoundSystem.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoundSystem.Files
{
    /// <summary>
    /// base class for groups of sound files either for VFX, Music or Ambience
    /// </summary>
    [Serializable]
    public abstract class SoundFileGroup
    {
        [EnumToggleButtons]
        [SerializeField] private Randomization randomization = Randomization.RoundRobin;

        [FoldoutGroup("Edit All", Expanded = false)] [Button] [HorizontalGroup("Edit All/Misc", LabelWidth = 40)]
        private void ApplySettings() => Validate();
        
        [FoldoutGroup("Edit All", Expanded = false)] [Button("$buttonName")] [HorizontalGroup("Edit All/Misc", LabelWidth = 40)]
        private void OpenClose() => Toggle();
        
        //--------------------------------------------------------------------------------------------------------------
        //--- SOUND FILE SETTINGS ---
        //--------------------------------------------------------------------------------------------------------------
        
        [Tooltip("The pitch of the clip")]
        [FoldoutGroup("Edit All")] [SerializeField] [Range(-3f, 3f)]
        protected float pitch = 1f;
        
        [Tooltip("Randomize the pitch of the clip")]
        [FoldoutGroup("Edit All")] [SerializeField] 
        protected bool randomizePitch = false;
        
        [Tooltip("The range of the pitch of the clip")]
        [FoldoutGroup("Edit All")] [SerializeField][ShowIf("@randomizePitch == true")]
        [MinMaxSlider(-3f, 3f)] protected Vector2 randomizedPitch = new Vector2(.5f,1.5f);
        
        [Tooltip("The volume of the clip")]
        [FoldoutGroup("Edit All")] [SerializeField] [Range(0f, 1f)]
        protected float volume = 1f;
        
        [Tooltip("When enabled the file will be played as a (non AudioSource blocking) One Shot")]
        [FoldoutGroup("Edit All")] [SerializeField] [EnableIf("@enableOneShot == true")]
        protected bool playOneShot = true;

        [Tooltip("When enabled the file will be looped")]
        [FoldoutGroup("Edit All")] [SerializeField] [ShowIf("@playOneShot == false")]
        protected bool loop = false;
        
        [Tooltip("When enabled the file will be queued if no audioSource is available at the time.")]
        [FoldoutGroup("Edit All")] [SerializeField] [ShowIf("@playOneShot == false")]
        protected bool queue = false;
        
        [Tooltip("When enabled the file will forced to be played. Caution! This will stop another file.")]
        [FoldoutGroup("Edit All")] [SerializeField] [ShowIf("@playOneShot == false")]
        protected bool forcePlay = false;
        
        //--------------------------------------------------------------------------------------------------------------

        protected abstract bool enableOneShot { get; }
        protected abstract void Validate();

        private string buttonName
        {
            get
            {
                if(soundFiles.Length <= 0) return "Open";
                return soundFiles[0].edit ? "Collapse" : "Open";
            }
        }

        public Randomization Randomization => randomization;
        
        /// <summary>
        /// Returns every sound file of the target group
        /// </summary>
        /// <returns></returns>
        public SoundFile[] GetFiles() => soundFiles;
        protected abstract SoundFile[] soundFiles { get; }
        
        private void Toggle()
        {
            if(soundFiles.Length <= 0) return;
            var state = soundFiles[0].edit;
            foreach (var file in soundFiles)
            {
                file.edit = !state;
            }
        }

       
        /// <summary>
        /// Returns a random file form the SoundFile pool.
        /// </summary>
        /// <returns></returns>
        public virtual SoundFile GetSoundFile()
        {
            if (soundFiles.Length <= 0) return null;
            switch (randomization)
            {
                case Randomization.RoundRobin:
                    index = (index >= (soundFiles.Length - 1)) ? 0 : index += 1;
                    return soundFiles[index];
                
                case Randomization.Random:
                    return soundFiles[Random.Range(0, soundFiles.Length)];

                case Randomization.First:
                    return soundFiles[0];
                
                default:
                    return soundFiles[Random.Range(0, soundFiles.Length)];
            }
        }

        private int index = 0;
    }
}