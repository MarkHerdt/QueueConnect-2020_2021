using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Enums;
using SoundSystem.Files;
using SoundSystem.SoundEvents;
using SoundSystem.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoundSystem.Core
{
    /// <summary>
    /// This class is responsible for playing audio's and managing AudioSources during runtime.
    /// Only access it from the AudioSystem.
    /// </summary>
    [ExecuteInEditMode]
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    internal class AudioPlayer : MonoSingleton<AudioPlayer>
    {
        //--------------------------------------------------------------------------------------------------------------

        #region --- [GENERAL] ---

        public AudioSource[] AudioSources => GetComponents<AudioSource>();

        /// <summary>
        /// Pool containing every AudioSource that is used for OneShots.
        /// </summary>
        private readonly Queue<AudioSource> OneShotPool = new Queue<AudioSource>();        
        
        /// <summary>
        /// Structure used to store queued audio files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private readonly struct QueuedFile<T> where T : SoundFile
        {
            public readonly T SoundFile;
            public readonly int EventID;
            
            /// <summary>
            /// Initialize a new instance of the QueuedFile struct.
            /// </summary>
            /// <param name="file">The file that is to be queued</param>
            /// <param name="eventID">The id of the event to which the queued file is to be assigned</param>
            public QueuedFile(T file, int eventID)
            {
                SoundFile = file;
                EventID = eventID;
            }
        }

        #endregion
        
        #region --- [VFX & AMBIENCE FIELDS] ---

        private Ambience lastPlayedAmbience;
        private VFX lastPlayedVFX;

        private volatile Queue<AudioSource> VFXPool = new Queue<AudioSource>();                                         
        private volatile Stack<AudioSource> AmbiencePool = new Stack<AudioSource>();

        private volatile Queue<QueuedFile<AmbienceFile>> QueuedAmbience = new Queue<QueuedFile<AmbienceFile>>();
        private volatile Queue<QueuedFile<VFXFile>> QueuedVFX = new Queue<QueuedFile<VFXFile>>();

        private static readonly Dictionary<VFX, List<AudioSource>> ActiveVFXDictionary = new Dictionary<VFX, List<AudioSource>>();
        private static readonly Dictionary<Ambience, List<AudioSource>> ActiveAmbienceDictionary = new Dictionary<Ambience, List<AudioSource>>();
        
        #endregion
        
        #region --- [MUSIC FIELDS] ---
        

        /// <summary>
        /// Returns one of two AudioSources that are dedicated for music  
        /// </summary>
        /// <returns></returns>
        private AudioSource GetNextAudioSource()
        {
            if (MusicPool.Count <= 0)
            {
                Debug.LogWarning("Warning Pool Empty!");
                return null;
            }
            var returnValue = MusicPool.Dequeue();
            MusicPool.Enqueue(returnValue);
            return returnValue;
        }
        private readonly Queue<AudioSource> MusicPool = new Queue<AudioSource>();
        
        
        /// <summary>
        /// Array containing the music files that ought to be played in a loop 
        /// </summary>
        private MusicFile[] MusicLoop = null;
        private MusicFile activeMusicFile = null;
        private AudioSource activeMusicSource = null;
        private SoundEventMusic activeMusicEvent = null;
        
        // --- Active music randomization method
        private Randomization musicRandomization;
        private int roundRobinIndex;
        private int randomIndex;

        private Coroutine FadeAudioInRoutine = null;
        private Coroutine FadeAudioOutRoutine = null;
        
        #endregion
        
        #region --- [AUDIO SOURCES] ---
        
        /// <summary>
        /// Returns an AudioSource from the OneShotPool.
        /// </summary>
        private AudioSource AudioSourceOneShot
        {
            get
            {
                if(OneShotPool.Count > 0)
                    return OneShotPool.Dequeue();
                
                Debug.LogWarning("Pool is empty. Initializing new pool");
                InitializeAudioPools();
                return OneShotPool.Dequeue();
            }
        }
        
        /// <summary>
        /// Returns an AudioSource from the VFXPool.
        /// </summary>
        private AudioSource AudioSourceVFX
        {
            get
            {
                var returnValue = VFXPool.Count > 0 ? VFXPool.Dequeue() : null;
                if (!returnValue)
                {
                    Debug.LogWarning("VFX Queue is empty. Adding additional AudioSource");
                    returnValue = gameObject.AddComponent<AudioSource>();
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Returns an AudioSource from the AmbiencePool.
        /// </summary>
        private AudioSource AudioSourceAmbience => AmbiencePool.Count > 0 ? AmbiencePool.Pop() : null;
        
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ONESHOT] ---

        /// <summary>
        /// Play a short music file that will not block an AudioSource.
        /// Clips played by this method should not be too long.
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="id"></param>
        internal void PlayOneShot(SoundFile soundFile, VFX? id = null)
        {
            if(!AudioSystem.Instance) return;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(AudioSystem.LogVFX && id != null)
                Debug.Log($"Playing VFX: [{(VFX)id}]");
#endif
            
            if (soundFile == null)
            {
                Debug.LogWarning("Sound File is null");
                return;
            }
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (AudioSystem.LogWarnings)
            {
                if (soundFile.Clip.length > AudioSystem.MaxOneShotClipLenght)
                {
                    Debug.LogWarning(
                        $"Warning length of clip exceeds optimal limit of {AudioSystem.MaxOneShotClipLenght} seconds. " +
                        $"Use {nameof(PlayVFX)}, {nameof(PlayMusic)} or {nameof(PlayAmbience)} instead of " +
                        $"{nameof(PlayOneShot)} for long audio clips.");
                }
            }
#endif
            
            var source = AudioSourceOneShot;
            source.pitch = soundFile.RandomizePitch ? Random.Range(soundFile.RandomizedPitch.x, soundFile.RandomizedPitch.y) : soundFile.Pitch;
            source.volume = soundFile.Volume;
            source.PlayOneShot(soundFile.Clip);
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (AudioSystem.LogPlayedMusic)
            {
                Debug.Log($"Now playing (OneShot): {soundFile.Clip}");     
            }
#endif
            
            OneShotPool.Enqueue(source);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [VFX] ---

        /// <summary>
        /// Play a vfx music file.
        /// </summary>
        /// <param name="eventID">The id of the associated event</param>
        /// <param name="file">The music file that is to be played</param>
        internal void PlayVFX(int eventID, VFXFile file)
        {
            if(!AudioSystem.Instance) return;
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(AudioSystem.LogVFX)
                Debug.Log($"Playing VFX: [{(VFX)eventID}]");
            #endif
            
            var audioSource = AudioSourceVFX;
            
            if (audioSource == null)
            {
                if (file.Queue)
                {
                    QueuedVFX.Enqueue(new QueuedFile<VFXFile>(file, eventID));
                    return;
                }
                if (file.ForcePlay)
                {
                    StopVFX(null);
                    audioSource = AudioSourceVFX;
                }
                else
                {
                    return;
                }
            }
            
            audioSource.volume = file.Volume;
            audioSource.pitch = file.RandomizePitch ? Random.Range(file.RandomizedPitch.x, file.RandomizedPitch.y) : file.Pitch;
            audioSource.loop = file.Loop;
            audioSource.clip = file.Clip;
            audioSource.Play();
            try
            {
                ActiveVFXDictionary[(VFX)eventID].Add(audioSource);
            }
            catch
            {
                InitInternalDictionaries();
                ActiveVFXDictionary[(VFX)eventID].Add(audioSource);
            }
            lastPlayedVFX = (VFX) eventID;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [MUSIC] ---

        /// <summary>
        /// This Method will set the passed audio event (SoundEventMusic) as the new active music event.
        /// </summary>
        /// <param name="musicEvent"></param>
        internal void PlayMusic(SoundEventMusic musicEvent)
        {
            if(!AudioSystem.Instance) return;
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (musicEvent == activeMusicEvent)
            {
                if(AudioSystem.LogWarnings && Application.isPlaying)
                    Debug.LogWarning("Warning passed event is already being played!");
                return;
            }
            if (!Application.isPlaying && musicEvent.Transition)
            {
                if(AudioSystem.LogWarnings)
                    Debug.LogWarning("Warning cannot play music with transitions while in editor!");
                return;
            }
#endif
            
            MusicLoop = musicEvent.MusicFiles.GetFiles() as MusicFile[];
            musicRandomization = musicEvent.MusicFiles.Randomization;
            var audioSource = GetNextAudioSource();
            
            PlayMusicFileFromQueue(audioSource, out var targetVolume);
            
            if (musicEvent.Transition && musicEvent.FadeIn)
            {
                if(FadeAudioInRoutine != null) StopCoroutine(FadeAudioInRoutine);
                FadeAudioInRoutine = StartCoroutine(FadeAudioIn(audioSource, targetVolume, musicEvent.FadeInDuration));
            }

            StopMusic(true);

            activeMusicEvent = musicEvent;
            activeMusicSource = audioSource;
        }

        
        /// <summary>
        /// Dequeues a music file and starts to play it on the passed AudioSource. 
        /// </summary>
        /// <param name="audioSource"></param>
        private void PlayMusicFileFromQueue(AudioSource audioSource) => PlayMusicFileFromQueue(audioSource, out var volume);
        
        /// <summary>
        /// Dequeues a music file and starts to play it on the passed AudioSource. 
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="volume">The target volume of the dequeued audio file. This param is important when fading in</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void PlayMusicFileFromQueue(AudioSource audioSource, out float volume)
        {
            switch (musicRandomization)
            {
                case Randomization.RoundRobin:
                    roundRobinIndex = (roundRobinIndex >= (MusicLoop.Length - 1)) ? 0 : roundRobinIndex += 1;
                    activeMusicFile = MusicLoop[roundRobinIndex];
                    break;
                
                case Randomization.Random:
                    var tempIndex = Random.Range(0, MusicLoop.Length);
                    if (MusicLoop.Length > 1)
                    {
                        while (tempIndex == randomIndex)
                        {
                            tempIndex = Random.Range(0, MusicLoop.Length);
                        }
                        randomIndex = tempIndex;
                    }
                    activeMusicFile = MusicLoop[tempIndex];
                    break;
                
                case Randomization.First:
                    activeMusicFile = MusicLoop[0];
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            audioSource.clip = activeMusicFile.Clip;
            audioSource.volume = activeMusicFile.Volume;
            audioSource.pitch = activeMusicFile.RandomizePitch ? Random.Range(activeMusicFile.RandomizedPitch.x, activeMusicFile.RandomizedPitch.y) : activeMusicFile.Pitch;
            audioSource.loop = AudioSystem.EnableIndividualMusicFileLooping && activeMusicFile.Loop;
            audioSource.Play();
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (AudioSystem.LogWarnings && audioSource.loop)
            {
                Debug.LogWarning("Warning: looping music will prevent other music files of the same group to be played!");
            }
            if (AudioSystem.LogPlayedMusic)
            {
                Debug.Log($"Now playing: {activeMusicFile.Clip.name}");    
            }
#endif
            volume = activeMusicFile.Volume;
        }
        
        /// <summary>
        /// Coroutine that will fade the given AudioSource in over the given duration.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="targetVolume">The volume that is to be attained after the specified duration</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private static IEnumerator FadeAudioIn(AudioSource audioSource, float targetVolume, float duration)
        {
            audioSource.volume = 0;
            audioSource.Play();
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (AudioSystem.LogMisc)
            {
                Debug.Log($"Start: Fading Audio in {audioSource.clip} | Duration: {duration}");
            }
#endif
            
            var timer = 0f;
            while (timer < duration)
            {
                yield return null;
                var deltaTime = Time.deltaTime;
                timer += deltaTime;

                audioSource.volume += targetVolume / duration * deltaTime;
                
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(AudioSystem.LogMisc)
                    Debug.Log(audioSource.volume);
#endif
                
            }

            audioSource.volume = targetVolume;
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (AudioSystem.LogMisc)
            {
                Debug.Log($"End: Fading Audio in {audioSource.clip} | Duration: {duration}");
            }
#endif
        }
        
        /// <summary>
        /// Coroutine that will fade the given AudioSource out over the given duration.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private static IEnumerator FadeAudioOut(AudioSource audioSource, float duration)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(AudioSystem.LogMisc)
                Debug.Log($"Stop Audio: {audioSource.clip.name} (fading out) (Duration){duration}");
#endif
            var startVolume = audioSource.volume;
            var timer = 0f;
            while (timer < duration)
            {
                yield return null;
                timer += Time.deltaTime;
                audioSource.volume = startVolume * (duration - timer) / duration;
            }
            
            audioSource.Stop();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(AudioSystem.LogMisc)
                Debug.Log($"Stopped Audio: {audioSource.clip.name}");
#endif
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [AMBIENCE] ---

        /// <summary>
        /// Play an ambience music file.
        /// </summary>
        /// <param name="eventID">The id of the associated event</param>
        /// <param name="file">The music file that is to be played</param>
        /// <param name="fadeIn"></param>
        /// <param name="duration"></param>
        internal void PlayAmbience(int eventID, AmbienceFile file, bool fadeIn, float duration = 2f)
        {
            var audioSource = AudioSourceAmbience;
            
            if (audioSource == null)
            {
                if (file.Queue)
                {
                    QueuedAmbience.Enqueue(new QueuedFile<AmbienceFile>(file, eventID));
                    return;
                }
                if (file.ForcePlay)
                {
                    StopAmbience(null);
                    audioSource = AudioSourceAmbience;
                }
                else
                {
                    return;
                }
            }
            
            audioSource.volume = file.Volume;
            audioSource.pitch = file.RandomizePitch ? Random.Range(file.RandomizedPitch.x, file.RandomizedPitch.y) : file.Pitch;
            audioSource.loop = file.Loop;
            audioSource.clip = file.Clip;
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(fadeIn && Application.isPlaying)
                StartCoroutine(FadeAudioIn(audioSource, file.Volume, duration));
            else 
                audioSource.Play();
#else
            if(fadeIn)
                StartCoroutine(FadeAudioIn(audioSource, file.Volume, duration));
            else 
                audioSource.Play();
#endif
            
            
            
            ActiveAmbienceDictionary[(Ambience)eventID].Add(audioSource);
            lastPlayedAmbience = (Ambience) eventID;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [UPDATE] ---

        private void OnApplicationQuit()
        {
            ActiveVFXDictionary.Clear();
            ActiveAmbienceDictionary.Clear();
        }

        private void Start()
        {
            AudioSystem.InitMixer();
            if (Application.isPlaying)
            {
                AudioSystem.PlayAmbience(Ambience.FactoryPause);
                AudioSystem.PlayMusic(Music.MainMenu);
            }
        }

        private readonly List<AudioSource> marked = new List<AudioSource>();

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if(AudioSystem.LogEditorUpdate) Debug.Log("Editor Update");
            }
#endif
            
            // --- Every VFX Audio that was playing but is not playing anymore is marked available again.
            foreach (var active in ActiveVFXDictionary)
            {
                marked.Clear();
                
                foreach (var audioSource in active.Value)
                {
                    if(!audioSource.isPlaying)
                        marked.Add(audioSource);
                }

                foreach (var audioSource in marked)
                {
                    active.Value.Remove(audioSource);
                    VFXPool.Enqueue(audioSource);
                }
            }
            foreach (var active in ActiveAmbienceDictionary)
            {
                marked.Clear();
                
                foreach (var audioSource in active.Value)
                {
                    
                    if(!audioSource) continue;
                    if(!audioSource.isPlaying)
                        marked.Add(audioSource);
                }

                foreach (var audioSource in marked)
                {
                    active.Value.Remove(audioSource);
                    AmbiencePool.Push(audioSource);
                }
            }
            
            

            // --- Music loop ---
            if (activeMusicSource != null && MusicLoop != null)
            {
                // If music is queued and the active music audio source has stopped playing this frame we add play another
                // file from the MusicLoop
                if (!activeMusicSource.isPlaying && MusicLoop.Length > 0)
                {
                    PlayMusicFileFromQueue(activeMusicSource);
                }
            }
            
            // --- Ambience queue ---
            if (AmbiencePool.Count > 0 &&  QueuedAmbience.Count > 0)
            {
                var dequeue = QueuedAmbience.Dequeue();
                PlayAmbience(dequeue.EventID, dequeue.SoundFile, false);
            }
            
            // --- VFX queue ---
            if (VFXPool.Count > 0 &&  QueuedVFX.Count > 0)
            {
                var dequeue = QueuedVFX.Dequeue();
                PlayVFX(dequeue.EventID, dequeue.SoundFile);
            }
        }
        

        
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [STOP] ---
        
        /// <summary>
        /// Stops the passed vfx event from playing. If the passed value is null, the last played vfx event
        /// will be stopped instead.
        /// </summary>
        /// <param name="key"></param>
        internal void StopVFX(VFX? key)
        {
            if (!ActiveVFXDictionary.TryGetValue(key ?? lastPlayedVFX, out var audioStack)) return;
            
            foreach (var audioSource in audioStack)
            {
                //TODO: stack => queue
                StartCoroutine(FadeAudioOut(audioSource, .1f));
                //audioSource.Stop();
                VFXPool.Enqueue(audioSource);
            }
            
            ActiveVFXDictionary[key ?? lastPlayedVFX].Clear();
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(AudioSystem.LogVFX)
                Debug.Log($"Stopping VFX: {key ?? lastPlayedVFX}");
#endif
        }

        /// <summary>
        /// Stops the passed ambience event from playing. If the passed value is null, the last played ambience event
        /// will be stopped instead.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fadeOut"></param>
        /// <param name="duration"></param>
        internal void StopAmbience(Ambience? key, bool fadeOut = false, float duration = 2f)
        {
            if (!ActiveAmbienceDictionary.TryGetValue(key ?? lastPlayedAmbience, out var audioStack)) return;

            if (!fadeOut)
            {
                foreach (var audioSource in audioStack)
                {
                    audioSource.Stop();
                    AmbiencePool.Push(audioSource);
                }

                ActiveAmbienceDictionary[key ?? lastPlayedAmbience].Clear();
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(AudioSystem.LogMisc)
                    Debug.Log($"Stopping Ambience: {key ?? lastPlayedAmbience}");
#endif                
            }
            else
            {
                foreach (var audioSource in audioStack)
                {
                    StartCoroutine(FadeAudioOut(audioSource, duration));
                }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(AudioSystem.LogMisc)
                    Debug.Log($"Stopping Ambience: {key ?? lastPlayedAmbience}");
#endif                
            }

        }
        
        
        /// <summary>
        /// Stop the currently played music.
        /// </summary>
        /// <param name="fadeOut">When true, the music will be faded out before being completely stopped</param>
        /// <param name="skip">When true, the next element in the music queue will be played.</param>
        internal void StopMusic(bool fadeOut, bool skip = true)
        {
            if (fadeOut)
            {
                if (activeMusicEvent != null && activeMusicSource != null)
                {
                    if (activeMusicEvent.Transition && activeMusicEvent.FadeOut)
                    {
                        if(FadeAudioOutRoutine != null) StopCoroutine(FadeAudioOutRoutine);
                        FadeAudioOutRoutine = StartCoroutine(FadeAudioOut(activeMusicSource, activeMusicEvent.FadeOutDuration));
                    }
                    else
                    {
                        activeMusicSource.Stop();
                    }

                    if (!skip)
                    {
                        MusicLoop = null;
                        activeMusicSource = null;
                        activeMusicEvent = null;
                    }
                }
            }
            else
            {
                if(FadeAudioInRoutine != null) StopCoroutine(FadeAudioInRoutine);
                if(FadeAudioOutRoutine != null) StopCoroutine(FadeAudioOutRoutine);
                GetNextAudioSource()?.Stop();
                GetNextAudioSource()?.Stop();
                activeMusicSource = null;
                activeMusicEvent = null;
                MusicLoop = null;
            }
        }

        
        /// <summary>
        /// Method will stop every active AudioSource.
        /// </summary>
        /// <param name="force">When enabled, the method will not use cached AudioSources.
        /// This will ensure that every AudioSource on the object is stopped</param>
        public void StopAllAudioClips(bool force = false)
        {
            if (force)
            {
                foreach (var audioSource in Instance.gameObject.GetComponents<AudioSource>())
                {
                    audioSource.Stop();
                }

                StopMusic(false);
            }
            else
            {
                foreach (var audioSource in OneShotPool)
                {
                    audioSource.Stop();
                }

                var vfx = ActiveVFXDictionary.Keys.ToArray();
                foreach (var key in vfx)
                {
                    StopVFX(key);
                }
                
                var ambience = ActiveAmbienceDictionary.Keys.ToArray();
                foreach (var key in ambience)
                {
                    StopAmbience(key);
                }
                
                StopMusic(false);
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [UNITY_EDITOR] ---
        
#if UNITY_EDITOR
        
        // Most of the member within this region are used to simulate an attenuated Update method in EditMode to enable
        // the preview of most music events / files.

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void DidReloadScripts()
        {
            EditorUpdate();
            if(Instance != null) Instance.StopAllAudioClips(true);
        }

        [UnityEditor.InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorUpdate();
            if(Instance != null) Instance.InitializeAudioPools();
        }

        private AudioPlayer() => AudioSystem.OnEditorUpdateEnabled += EditorUpdate;
        
        private static bool editorUpdate = false; 
        
        /// <summary>
        /// Editor update is only called when enabled by the AudioSystem
        /// </summary>
        [UnityEditor.InitializeOnLoadMethod]
        private static async void EditorUpdate()
        {
            // this bool will be true while the editor update is running, preventing it to run multiple times.
            
            if(editorUpdate) return;
            editorUpdate = true;
            
            if(AudioSystem.LogEditorUpdate && !Application.isPlaying)
                Debug.Log("Initialize Editor Update");
            while (!Application.isPlaying && AudioSystem.EnableEditorUpdate)
            {
                await Task.Delay(AudioSystem.EditorUpdateIntervalMS);
                if(Instance != null) Instance.Update();
            }
            editorUpdate = false;
        }

#endif

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INITIALIZATION] ---
        
        static AudioPlayer() => InitInternalDictionaries();

        protected static void InitInternalDictionaries()
        {
            foreach (VFX enumValue in Enum.GetValues(typeof(VFX)))
            {
                ActiveVFXDictionary[enumValue] = new List<AudioSource>();
            }
            foreach (Ambience enumValue in Enum.GetValues(typeof(Ambience)))
            {
                ActiveAmbienceDictionary[enumValue] = new List<AudioSource>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            AudioSystem.Instance.Initialize();
            InitializeAudioPools();
        }
        

        private void InitializeAudioPools()
        {
            if(!AudioSystem.Instance) return;


#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (AudioSystem.LogMisc)
            {
                Debug.Log("Initializing Audio Pools");      
            }
#endif

            // --- Destroy every AudioSource
            foreach (var audioSource in GetComponents<AudioSource>())
            {
                audioSource.ForceDestroy();
            }
            
            // --- Init OneShotPool
            for (var i = 0; i < AudioSystem.ChannelsOneShot; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = AudioSystem.Instance.VFXGroup;
                OneShotPool.Enqueue(audioSource);
            }
            
            // --- Init VFX Pool
            for (var i = 0; i < AudioSystem.ChannelsVFX; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = AudioSystem.Instance.VFXGroup;
                VFXPool.Enqueue(audioSource);
            }
            
            // --- Init Ambience Pool
            for (var i = 0; i < AudioSystem.ChannelsAmbience; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = AudioSystem.Instance.AmbienceGroup;
                AmbiencePool.Push(audioSource);
            }
            
            // --- Init Music Pool
            for (var i = 0; i < AudioSystem.ChannelsMusic; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = AudioSystem.Instance.MusicGroup;
                MusicPool.Enqueue(audioSource);
            }
        }
        
        #endregion
    }
}