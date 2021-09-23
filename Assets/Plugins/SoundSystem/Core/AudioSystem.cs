using System;
using System.Collections.Generic;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem.Files;
using SoundSystem.SoundEvents;
using SoundSystem.Utils;
using UnityEngine;
using UnityEngine.Audio;
using Task = System.Threading.Tasks.Task;

namespace SoundSystem.Core
{
    /// <summary>
    /// The AudioSystem handles settings and stores configurations / files for audio events.
    /// Use the public methods of this class to invoke audio events during runtime.
    /// </summary>
    [HideMonoScript]
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public sealed class AudioSystem : Settings<AudioSystem>
    {
        #region --- [PUBLIC ACCESS] ---
        
        //--------------------------------------------------------------------------------------------------------------
        //----- PLAY
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Invoke the passed VFX event.
        /// </summary>
        /// <param name="id">the id of the sound event</param>
        /// <param name="delay">delay in milliseconds until the sound is played</param>
        public static void PlayVFX(VFX id, int? delay = null) => Instance.InvokeVFX(id, delay);
        
        /// <summary>
        /// Invoke the passed Music event.
        /// </summary>
        /// <param name="id"></param>
        public static void PlayMusic(Music id) => Instance.InvokeMusic(id);
        
        /// <summary>
        /// Invoke the passed Ambience event.
        /// Fade in will only be used if the file is not played as a OneShot.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fadeIn"></param>
        /// <param name="duration"></param>
        public static void PlayAmbience(Ambience id, bool fadeIn = true, float duration = 2f) 
            => Instance.InvokeAmbience(id, fadeIn, duration);
        
        //--------------------------------------------------------------------------------------------------------------
        //----- VOLUME
        //--------------------------------------------------------------------------------------------------------------

        public static float VolumeMaster
        {
            get => Instance.volumeMaster;
            set
            {
                if (value > 0 && IsMuted)
                {
                    OnAudioUnmuted?.Invoke();
                    isMuted = false;
                    cachedVolume = value;
                    PlayerPrefs.SetFloat(CachedMasterKey, cachedVolume);
                    PlayerPrefs.SetInt(IsMutedKey, 0);
                }
                Instance.volumeMaster = Mathf.Clamp(value, MinVolume, MaxVolume);
                Instance.OnMasterVolumeChanged();
            }
        }

        public static float VolumeVFX
        {
            get => Instance.volumeVFX;
            set
            {
                Instance.volumeVFX = Mathf.Clamp(value, MinVolume, MaxVolume);
                Instance.OnVFXVolumeChanged();
            }
        }

        public static float VolumeAmbience
        {
            get => Instance.volumeAmbience;
            set
            {
                Instance.volumeAmbience = Mathf.Clamp(value, MinVolume, MaxVolume);
                Instance.OnAmbienceVolumeChanged();
            }
        }

        public static float VolumeMusic
        {
            get => Instance.volumeMusic;
            set
            {
                Instance.volumeMusic = Mathf.Clamp(value, MinVolume, MaxVolume);
                Instance.OnMusicVolumeChanged();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------
        //----- MUTE / UNMUTE 
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns true if the volume of the master mixer is set to -80
        /// </summary>
        [ShowInInspector][ReadOnly][PropertyOrder(-100)]
        public static bool IsMuted => (isMuted ?? PlayerPrefs.HasKey(IsMutedKey)) && PlayerPrefs.GetInt(IsMutedKey) > 0;

        
        /// <summary>
        /// Set the volume of the master mixer to -80 and cache the current value of the volume to revert it back when
        /// un-muting the audio. 
        /// </summary>
        public static void MuteAudio()
        {
            Instance.gainGroup.audioMixer.SetFloat("Volume", MinVolume);
            OnAudioMuted?.Invoke();
            isMuted = true;
            PlayerPrefs.SetInt(IsMutedKey, 1);
        }

        /// <summary>
        /// Unmute the audio.
        /// </summary>
        public static void UnmuteAudio()
        {
            Instance.gainGroup.audioMixer.SetFloat("Volume", DefaultVolume + Gain);
            OnAudioUnmuted?.Invoke();
            isMuted = false;
            PlayerPrefs.SetInt(IsMutedKey, 0);
        }

        /// <summary>
        /// Either mute or unmute the audio depending on its current state.
        /// </summary>
        public static void ToggleAudioOnOff()
        {
            if(IsMuted)UnmuteAudio();
            else MuteAudio();
        }
        
        
        //--------------------------------------------------------------------------------------------------------------
        //----- STOP 
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Stop the passed VFX event.
        /// </summary>
        /// <param name="vfx"></param>
        public static void StopVFX(VFX vfx) => Audio.StopVFX(vfx);
        
        /// <summary>
        /// Stop the passed Ambience event.
        /// </summary>
        public static void StopAmbience(Ambience? ambience, bool fadeOut = true, float duration = 2f)
            => Audio.StopAmbience(ambience, fadeOut, duration);
        
        /// <summary>
        /// Skip the active music.
        /// </summary>
        /// <param name="fadeOut">When true, the music will be faded out slowly</param>
        public static void SkipMusic(bool fadeOut) => Audio.StopMusic(fadeOut, true);
        
        /// <summary>
        /// Stop the active music.
        /// </summary>
        /// <param name="fadeOut">When true, the music will be faded out slowly</param>
        public static void StopMusic(bool fadeOut) => Audio.StopMusic(fadeOut, false);

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INSPECTOR] ---

        [Tooltip("The tolerated lenght for clips played as one shots. If the lenght exceeds this value, a warning will be logged.")]
        [SerializeField][Range(0,10)] private float oneShotClipLenghtTolerance = 3f;


        
        [Header("Volume")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnGainValueChanged))]
#endif
        [SerializeField] [Range(0, 20)] private float gain = 0f;
        [Space]
        
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnMasterVolumeChanged))]
#endif
        [SerializeField] [Range(MinVolume,MaxVolume)] private float volumeMaster = -11f;


#if UNITY_EDITOR
        [OnValueChanged(nameof(OnVFXVolumeChanged))]
#endif
        [SerializeField] [Range(MinVolume,MaxVolume)] private float volumeVFX = -11f;
        

#if UNITY_EDITOR
        [OnValueChanged(nameof(OnAmbienceVolumeChanged))]
#endif
        [SerializeField] [Range(MinVolume,MaxVolume)] private float volumeAmbience = -11f;


#if UNITY_EDITOR
        [OnValueChanged(nameof(OnMusicVolumeChanged))]
#endif
        [SerializeField] [Range(MinVolume,MaxVolume)] private float volumeMusic = -11f;


        [Space]
        [SerializeField] private AudioMixerGroup masterGroup = null;
        [SerializeField] private AudioMixerGroup gainGroup = null;
        [SerializeField] private AudioMixerGroup vfxGroup = null;
        [SerializeField] private AudioMixerGroup ambienceGroup = null;
        [SerializeField] private AudioMixerGroup musicGroup = null;


        [Header("Channels")]
        [Tooltip("The number of AudioSources dedicated for One Shots")]
        [SerializeField] [Range(1, 20)] private int channelsOneShot = 10;
        [Tooltip("The number of AudioSources dedicated for VFX")]
        [SerializeField] [Range(1, 10)] private int channelsVFX = 3;
        [Tooltip("The number of AudioSources dedicated for Ambience")]
        [SerializeField] [Range(1, 10)] private int channelsAmbience = 3;
        [Tooltip("The number of AudioSources dedicated for Music")]
        [ShowInInspector] [ReadOnly] private const int channelsMusic = 2;

        
        [Space]
        [Tooltip("When enabled individual music files can be looped. This is should be avoided because the passed files of an music event will be looped anyway. Looping an individual file will only block every other song of the same event from playing.")]
        [SerializeField] private bool enableIndividualMusicFileLooping = true;

        
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        [Tooltip("When enabled the Update method of the AudioPlayer will be simulated during EditMode. This will enabled the preview of sound events. ")]
        [Header("Editor")]
        #if UNITY_EDITOR
        [OnValueChanged(nameof(ValidateEditorUpdate))]
        #endif
        [SerializeField] private bool enableEditorUpdate = true;
        [Tooltip("The time between Updates in EditMode (in milliseconds) if EditorUpdate is enabled.")]
        [SerializeField] [Range(1,1000)] private int editorUpdateIntervalMS = 100;
        
        [Space]
        [SerializeField] private bool logWarnings = true;
        [SerializeField] private bool logPlayedMusic = true;
        [SerializeField] private bool logVFX = true;
        [SerializeField] private bool logEditorUpdate = true;
        [Tooltip("When enabled other logs will be displayed that are do not fall into the above categories")]
        [SerializeField] private bool logMisc = true;
#endif
        
        
        

        #endregion
        
        #region --- [SOUND EVENTS] ---

        [Title("VFX")]
        [SerializeField] private SoundEventVFX[] VFXEvents = null;
        
        [Title("Ambience")]
        [SerializeField] private SoundEventAmbience[] AmbienceEvents = null;
        
        [Title("Music")]
        [SerializeField] private SoundEventMusic[] MusicEvents = null;

        #endregion

        #region --- [PROPERTIES (PUBLIC)] ---
        
        //--------------------------------------------------------------------------------------------------------------
        //----- AUDIO MIXER
        //--------------------------------------------------------------------------------------------------------------

        public AudioMixerGroup MasterGroup => masterGroup;

        public AudioMixerGroup VFXGroup => vfxGroup;

        public AudioMixerGroup AmbienceGroup => ambienceGroup;

        public AudioMixerGroup MusicGroup => musicGroup;

        //--------------------------------------------------------------------------------------------------------------
        //----- MISC
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Additional volume gain.
        /// This value will not be serialized.
        /// </summary>
        public static float Gain
        {
            get
            {
                if (!Instance) return 10;
                return Instance.gain;
            }
            set
            {
                if (!Instance) return;
                Instance.OnGainValueChanged();
                Instance.gain = Mathf.Clamp(value, DefaultVolume, MaxVolume);
            }
        }

        /// <summary>
        /// The number of AudioSources dedicated for VFX
        /// </summary>
        public static int ChannelsVFX
        {
            get
            {
                if (!Instance) return 10;
                return Instance.channelsVFX;
            }
        }

        /// <summary>
        /// The number of AudioSources dedicated for Ambience
        /// </summary>
        public static int ChannelsAmbience
        {
            get
            {
                if (!Instance) return 10;
                return Instance.channelsAmbience;
            }
        }

        /// <summary>
        /// The number of AudioSources dedicated for Music
        /// </summary>
        public static int ChannelsMusic
        {
            get
            {
                if (!Instance) return 2;
                return channelsMusic;
            }
        }

        /// <summary>
        /// The number of AudioSources dedicated for OneShots
        /// </summary>
        public static int ChannelsOneShot
        {
            get
            {
                if (!Instance) return 10;
                return Instance.channelsOneShot;
            }
        }

        /// <summary>
        /// The tolerated lenght for clips played as one shots. If the lenght exceeds this value, a warning will be logged.
        /// </summary>
        public static float MaxOneShotClipLenght
        {
            get
            {
                if (!Instance) return 3f;
                return Instance.oneShotClipLenghtTolerance;
            }
        }

        /// <summary>
        /// When enabled individual music files can be looped. This is should be avoided because the passed files of an
        /// music event will be looped anyway. Looping an individual file will only block every other song of the same
        /// event from playing.
        /// </summary>
        public static bool EnableIndividualMusicFileLooping
        {
            get
            {
                if (!Instance) return false;
                return Instance.enableIndividualMusicFileLooping;
            }
        }


#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// When enabled the Update method of the AudioPlayer will be simulated during EditMode. This will enabled the
        /// preview of sound events. 
        /// </summary>
        public static bool EnableEditorUpdate
        {
            get
            {
                if (!Instance) return false;
                return Instance.enableEditorUpdate;
            }
        }

        /// <summary>
        /// The time between Updates in EditMode (in milliseconds) if EditorUpdate is enabled.
        /// </summary>
        public static int EditorUpdateIntervalMS
        {
            get
            {
                if(!Instance) return 1000;
                return Instance.editorUpdateIntervalMS;
            }
        }

        public static bool LogEditorUpdate
        {
            get
            {
                if (!Instance) return false;
                return Instance.logEditorUpdate;
            }
        }

        public static bool LogMisc
        {
            get
            {
                if (!Instance) return false;
                return Instance.logMisc;
            }
        }
        
        public static bool LogVFX
        {
            get
            {
                if (!Instance) return false;
                return Instance.logVFX;
            }
        }

        public static bool LogWarnings
        {
            get
            {
                if (!Instance) return false;
                return Instance.logWarnings;
            }
        }

        public static bool LogPlayedMusic
        {
            get
            {
                if (!Instance) return false;
                return Instance.logPlayedMusic;
            }
        }
#endif
        
        #endregion
        
        #region --- [PROPERTIES (PRIVATE)] ---
        
        private static AudioPlayer Audio => audioPlayer ? audioPlayer : (audioPlayer = AudioPlayer.Instance);
        private static AudioPlayer audioPlayer = null;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [DELEGATES] ---

        public delegate void VFXDelegate(VFX vfx);
        public delegate void AmbienceDelegate(Ambience ambience);
        public delegate void MusicDelegate(Music music);

        #endregion
        
        #region --- [CSHARP EVENTS] ---

        public static event Action OnAudioMuted;
        public static event Action OnAudioUnmuted;

        public static event VFXDelegate OnVFXEventInvoked;
        public static event AmbienceDelegate OnAmbienceEventInvoked;
        public static event MusicDelegate OnMusicEventInvoked;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [FIELDS] ---

        private readonly Dictionary<int, SoundEventVFX> SoundEventsVFX = new Dictionary<int, SoundEventVFX>();
        private readonly Dictionary<int, SoundEventMusic> SoundEventsMusic = new Dictionary<int, SoundEventMusic>();
        private readonly Dictionary<int, SoundEventAmbience> SoundEventsAmbience = new Dictionary<int, SoundEventAmbience>();
        
        private static bool? isMuted = null;
        private static float cachedVolume;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [PLAY VFX/AMBIENCE/MUSIC] ---

        private async void InvokeVFX(VFX sound, int? delay = null)
        {
            OnVFXEventInvoked?.Invoke(sound);
            if (delay is int millisecondsDelay)
            {
                await Task.Delay(millisecondsDelay);
            }
            
            if (SoundEventsVFX.TryGetValue((int)sound, out var vfxEvent))
            {
                foreach (var vfxGroup in vfxEvent.VFXFiles)
                {
                    if (!(vfxGroup.GetSoundFile() is VFXFile vfxFile))
                    {
#if DEVELOPMENT_BUILD
                        if(logWarnings)
                            Debug.LogWarning($"Warning! missing sound for event: {sound}!");
#endif
                        return;
                    }
                    if (vfxFile.PlayOneShot)
                    {
                        Audio.PlayOneShot(vfxFile, sound);
                    }
                    else
                    {
                        Audio.PlayVFX((int)sound, vfxFile);
                    }
                }
                return;
            }
#if DEVELOPMENT_BUILD
            if(logWarnings) 
                Debug.LogWarning($"Warning! no sound event for {sound} implemented!");
#endif
        }
        
        
        private void InvokeAmbience(Ambience sound, bool fadeIn, float duration)
        {
            OnAmbienceEventInvoked?.Invoke(sound);
            if (SoundEventsAmbience.TryGetValue((int)sound, out var eventAmbience))
            {
                foreach (var fileGroup in eventAmbience.AmbienceFiles)
                {
                    if (!(fileGroup.GetSoundFile() is AmbienceFile ambienceFile))
                    {
#if DEVELOPMENT_BUILD
                        if(logWarnings)
                            Debug.LogWarning($"Warning! missing sound for event: {sound}!");
#endif
                        return;
                    }
                    if (ambienceFile.PlayOneShot)
                    {
                        Audio.PlayOneShot(ambienceFile);
                    }
                    else
                    {
                        Audio.PlayAmbience((int)sound, ambienceFile, fadeIn, duration);
                    }
                }
                return;
            }
#if DEVELOPMENT_BUILD
            if(logWarnings)
                Debug.LogWarning($"Warning! no sound event for {sound} implemented!");
#endif
        }
        
        
        private void InvokeMusic(Music sound)
        {
            OnMusicEventInvoked?.Invoke(sound);
            if (SoundEventsMusic.TryGetValue((int) sound, out var eventMusic))
            {
                Audio.PlayMusic(eventMusic);
            }
            else
            {
                Debug.Log($"{sound} is not present in the {nameof(SoundEventsMusic)} Dictionary");
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [INITIALIZE] ---

        
        public void Initialize()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR 
            if (LogMisc)
            {
                Debug.Log("Initialize");
            }
#endif
            SoundEventsVFX.Clear();
            SoundEventsMusic.Clear();
            SoundEventsAmbience.Clear();
            
            foreach (var vfxEvent in VFXEvents)
            {
                if (SoundEventsVFX.ContainsKey(vfxEvent.ID))
                {
                    Debug.LogWarning($"{(VFX)vfxEvent.ID} IS ALREADY PRESENT IN THE DICTIONARY!");
                    continue;
                }
                SoundEventsVFX.Add(vfxEvent.ID, vfxEvent);
            }
            foreach (var musicEvent in MusicEvents)
            {
                if (SoundEventsMusic.ContainsKey(musicEvent.ID))
                {
                    Debug.LogWarning($"{(Music)musicEvent.ID} IS ALREADY PRESENT IN THE DICTIONARY!");
                    continue;
                }
                SoundEventsMusic.Add(musicEvent.ID, musicEvent);
            }
            foreach (var ambienceEvent in AmbienceEvents)
            {
                if (SoundEventsAmbience.ContainsKey(ambienceEvent.ID))
                {
                    Debug.LogWarning($"{(Ambience)ambienceEvent.ID} IS ALREADY PRESENT IN THE DICTIONARY!");
                    continue;
                }
                SoundEventsAmbience.Add(ambienceEvent.ID, ambienceEvent);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [VOLUME] ---
        
        internal static void InitMixer()
        {
            if(!Instance) return;
            Instance.InitializeMixer();
        }

        private const string CachedMasterKey = "CachedMasterKey";
        private const string IsMutedKey = "IsMutedKey";
        
        private const string GainKey = "GainVolume";
        
        private const string MasterKey = "MasterVolume";
        private const string VFXKey = "VFXVolume";
        private const string AmbienceKey = "AmbienceVolume";
        private const string MusicKey = "MusicVolume";

        private const float MinVolume = -80f;
        private const float MaxVolume = 0f;
        private const float DefaultVolume = 0f;
        
        private void InitializeMixer()
        {
            if(PlayerPrefs.HasKey(MasterKey))
            {
                volumeMaster = PlayerPrefs.GetFloat(MasterKey);
            }
            if(PlayerPrefs.HasKey(VFXKey))
            {
                volumeVFX = PlayerPrefs.GetFloat(VFXKey);
            }
            if(PlayerPrefs.HasKey(AmbienceKey))
            {
                volumeAmbience = PlayerPrefs.GetFloat(AmbienceKey);
            }
            if(PlayerPrefs.HasKey(MusicKey))
            {
                volumeMusic = PlayerPrefs.GetFloat(MusicKey);
            }

            OnMasterVolumeChanged();
            OnVFXVolumeChanged();
            OnAmbienceVolumeChanged();
            OnMusicVolumeChanged();
            LoadGainFromPrefs();
            OnGainValueChanged();
            
            if (IsMuted)
            {
                MuteAudio();
            }
        }

        private void LoadGainFromPrefs()
        {
            gain = PlayerPrefs.HasKey(GainKey)? PlayerPrefs.GetFloat(GainKey) : gain;
        }
        
        private void OnGainValueChanged()
        {
            PlayerPrefs.SetFloat(GainKey, gain);
            if (!IsMuted)
            {
                gainGroup.audioMixer.SetFloat("Volume", DefaultVolume + Gain);
            }
        }

        private void OnMasterVolumeChanged()
        {
            if(!masterGroup) return;
            
            masterGroup.audioMixer.SetFloat("volume_master", volumeMaster);
            PlayerPrefs.SetFloat(MasterKey, volumeMaster);
        }

        private void OnVFXVolumeChanged()
        {
            if(!vfxGroup) return;
            
            vfxGroup.audioMixer.SetFloat("volume_vfx", volumeVFX);
            PlayerPrefs.SetFloat(VFXKey, volumeVFX);
        }

        private void OnAmbienceVolumeChanged()
        {
            if(!ambienceGroup) return;
            
            ambienceGroup.audioMixer.SetFloat("volume_ambience", volumeAmbience);
            PlayerPrefs.SetFloat(AmbienceKey, volumeAmbience);
        }

        private void OnMusicVolumeChanged()
        {
            if(!musicGroup) return;
            
            musicGroup.audioMixer.SetFloat("volume_music", volumeMusic);
            PlayerPrefs.SetFloat(MusicKey, volumeMusic);
        }
        

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [EDITOR] ---


#if UNITY_EDITOR
        [UnityEditor.MenuItem("Sound/Edit Settings", priority = 0)]
        public static void EditSettings() => SelectObject(Instance);
        
        [UnityEditor.InitializeOnLoadMethod]
        internal static void Init()
        {
            try
            {
                if (Instance == null) return;
                Instance.Initialize();
            }
            catch (Exception exception)
            {
                if(LogWarnings)
                    Debug.LogWarning(exception);
            }
        }
        
        private void ValidateEditorUpdate()
        {
            if(enableEditorUpdate)
                OnEditorUpdateEnabled?.Invoke();
        }
        
        internal static event Action OnEditorUpdateEnabled;
#endif

        #endregion

    }
}
