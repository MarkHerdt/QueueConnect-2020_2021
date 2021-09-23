using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ganymed.Utils.Attributes;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Singleton;
using UnityEngine;
// ReSharper disable DelegateSubtraction

namespace Ganymed.Utils.Callbacks
{
    /// <summary>
    /// class providing a variety of callbacks for unity events, messages and editor callbacks. Subscribed listener are
    /// not required to check the application state for editor callbacks.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [ScriptOrder(-10000)]
    [GameObjectHideInHierarchy]
    public class UnityEventCallbacks : MonoSingleton<UnityEventCallbacks>
#if UNITY_EDITOR
        , UnityEditor.Build.IPreprocessBuildWithReport
#endif
    {
        
        #region --- [FIELDS] ---
        
        private static readonly Dictionary<ApplicationState, Dictionary<UnityEventType, Action<UnityEventType>>> callbacks
            = new Dictionary<ApplicationState, Dictionary<UnityEventType,  Action<UnityEventType>>>();
        
        private static readonly Dictionary<ApplicationState, Dictionary<UnityEventType, Action>> actions
            = new Dictionary<ApplicationState, Dictionary<UnityEventType, Action>>();

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [GAMEOBJECT] ---

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Ganymed/UnityEventCallbacks",false, 11)]
        
        public static void CreateGameObjectInstance()
        {
            try
            {
                var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab", new[] { "Assets" });

                foreach (var guid in guids)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);

                    if (prefab.name != "UnityEventCallbacks") continue;

                    Instantiate(prefab);
                    Debug.Log("Instantiated Unity Event Callbacks Prefab");
                    break;
                }
            }
            catch
            {
                Debug.LogWarning("Failed to instantiate UnityEventCallbacks Prefab!Make sure that the corresponding prefab" +
                                 "[UnityEventCallbacks] is present within the project.");
            }
        }
        #endif
                
        public static void ValidateUnityEventCallbacks()
        {
            if (!TryGetInstance(out var instance)) {
#if UNITY_EDITOR
                CreateGameObjectInstance();
#else
                Instantiate(new GameObject("UnityEventCallbacks")).AddComponent<UnityEventCallbacks>();
                Debug.LogWarning("UnityEventCallbacks instance was not present!");
#endif
            }
        }
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [CONSTRUCTOR] ---

        static UnityEventCallbacks()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += EditorApplicationOnQuitting;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.delayCall += EditorApplicationDelayCall;
#endif
            
            foreach (ApplicationState callbackDuring in Enum.GetValues(typeof(ApplicationState)))
            {
                callbacks[callbackDuring] = new Dictionary<UnityEventType, Action<UnityEventType>>();
                actions[callbackDuring] = new Dictionary<UnityEventType, Action>();

                foreach (UnityEventType callbackType in Enum.GetValues(typeof(UnityEventType)))
                {
                    callbacks[callbackDuring][callbackType] = default;
                    actions[callbackDuring][callbackType] = default;
                }
            }
            
#if UNITY_EDITOR
            InvokeCallbacks(UnityEventType.EditorApplicationStart, 100);
#endif
        }
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ADD EVENTLISTENER (CALLBACK)] ---

        /// <summary>
        /// Subscribe a delegate to a callback. (defined context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="removePreviousListener">insure that old listener are removed before adding new</param>
        /// <param name="callbackDuring">the context in which the listener is allowed to be called</param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action<UnityEventType> listener, bool removePreviousListener, ApplicationState callbackDuring,
            params UnityEventType[] callbackTypes)
        {
            if (removePreviousListener)
                RemoveEventListener(listener, callbackDuring, callbackTypes);
            
            foreach (var type in callbackTypes) {
                callbacks[callbackDuring][type] += listener;
            }
        }     
        
     
        /// <summary>
        /// Subscribe a listener to a callback. (defined context) previous listener will be removed by default.
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="callbackDuring">the context in which the listener is allowed to be called</param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action<UnityEventType> listener, ApplicationState callbackDuring,
            params UnityEventType[] callbackTypes)
        {
            RemoveEventListener(listener, callbackDuring, callbackTypes);
            
            foreach (var type in callbackTypes) {
                callbacks[callbackDuring][type] += listener;
            }
        }


        /// <summary>
        /// Subscribe a delegate to a callback. Listener are called during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="unityEventType"></param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action<UnityEventType> listener, UnityEventType unityEventType,
            params UnityEventType[] callbackTypes)
        {
            foreach (var type in callbackTypes) {
                callbacks[ApplicationState.EditAndPlayMode][type] += listener;
            }
        }


        /// <summary>
        /// Subscribe a delegate to a callback. Listener are called during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="removePreviousListener"></param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action<UnityEventType> listener, bool removePreviousListener,  params UnityEventType[] callbackTypes)
        {
            if(removePreviousListener)
                RemoveEventListener(listener,callbackTypes);
            
            foreach (var type in callbackTypes) {
                callbacks[ApplicationState.EditAndPlayMode][type] += listener;
            }
        }

        #endregion

        #region --- [REMOVE EVENTLISTENER (CALLBACK)] ---

        /// <summary>
        /// Remove a delegate from a callback. (defined context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="applicationState">the context from which you would like to remove the listener from</param>
        /// <param name="callbackTypes">the callback event types you would like to remove the listener from</param>
        public static void RemoveEventListener(Action<UnityEventType> listener, ApplicationState applicationState,
            params UnityEventType[] callbackTypes)
        {
            if (callbackTypes.Length > 0)
            {
                foreach (var type in callbackTypes)
                {
                    callbacks[applicationState][type] -= listener;
                }
            }
            else
            {
                foreach (UnityEventType type in Enum.GetValues(typeof(UnityEventType)))
                {
                    callbacks[applicationState][type] -= listener;
                }
            }
        }       
        
        
        /// <summary>
        /// Remove a listener from a callback. Callbacks are invoked during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="callbackTypes">the callback event types you would like to remove the listener from</param>
        public static void RemoveEventListener(Action<UnityEventType> listener, params UnityEventType[] callbackTypes)
        {
            if (callbackTypes.Length > 0)
            {
                foreach (var type in callbackTypes)
                {
                    callbacks[ApplicationState.EditAndPlayMode][type] -= listener;
                    callbacks[ApplicationState.EditMode][type] -= listener;
                    callbacks[ApplicationState.PlayMode][type] -= listener;
                }
            }
            else
            {
                foreach (UnityEventType type in Enum.GetValues(typeof(UnityEventType)))
                {
                    callbacks[ApplicationState.EditAndPlayMode][type] -= listener;
                    callbacks[ApplicationState.EditMode][type] -= listener;
                    callbacks[ApplicationState.PlayMode][type] -= listener;
                }
            }
        } 

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [ADD EVENTLISTENER (ACTIONS)] ---

        /// <summary>
        /// Subscribe a delegate to a callback. (defined context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="removePreviousListener">insure that old listener are removed before adding a new</param>
        /// <param name="callbackDuring">the context in which the listener is allowed to be called</param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action listener, bool removePreviousListener, ApplicationState callbackDuring,
            params UnityEventType[] callbackTypes)
        {
            if (removePreviousListener)
                RemoveEventListener(listener, callbackDuring, callbackTypes);
            
            foreach (var type in callbackTypes) {
                actions[callbackDuring][type] += listener;
            }
        }     
        
     
        /// <summary>
        /// Subscribe a delegate to a callback. (defined context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="callbackDuring">the context in which the listener is allowed to be called</param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action listener, ApplicationState callbackDuring,
            params UnityEventType[] callbackTypes)
        {
            foreach (var type in callbackTypes) {
                actions[callbackDuring][type] += listener;
            }
        }
      
        
        /// <summary>
        /// Subscribe a delegate to a callback. Listener are called during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action listener, params UnityEventType[] callbackTypes)
        {
            foreach (var type in callbackTypes) {
                actions[ApplicationState.EditAndPlayMode][type] += listener;
            }
        }


        /// <summary>
        /// Subscribe a delegate to a callback. Listener are called during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="removePreviousListener"></param>
        /// <param name="callbackTypes">the callback event types you would like to bind the listener to</param>
        public static void AddEventListener(Action listener, bool removePreviousListener,  params UnityEventType[] callbackTypes)
        {
            if(removePreviousListener)
                RemoveEventListener(listener,callbackTypes);
            
            foreach (var type in callbackTypes) {
                actions[ApplicationState.EditAndPlayMode][type] += listener;
            }
        }

        #endregion

        #region --- [REMOVE EVENTLISTENER (CLEAN)] ---

        /// <summary>
        /// Remove a delegate from a callback. (defined context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="applicationState">the context from which you would like to remove the listener from</param>
        /// <param name="callbackTypes">the callback event types you would like to remove the listener from</param>
        public static void RemoveEventListener(Action listener, ApplicationState applicationState,
            params UnityEventType[] callbackTypes)
        {
            if (callbackTypes.Length > 0)
            {
                foreach (var type in callbackTypes)
                {
                    actions[applicationState][type] -= listener;
                }
            }
            else
            {
                foreach (UnityEventType type in Enum.GetValues(typeof(UnityEventType)))
                {
                    actions[applicationState][type] -= listener;
                }
            }
        }       
        
        
        /// <summary>
        /// Remove a delegate from a callback. Listener are called during Edit and Playmode. (every context)
        /// </summary>
        /// <param name="listener">the delegate listening</param>
        /// <param name="callbackTypes">the callback event types you would like to remove the listener from</param>
        public static void RemoveEventListener(Action listener, params UnityEventType[] callbackTypes)
        {
            if (callbackTypes.Length > 0)
            {
                foreach (var type in callbackTypes)
                {
                    actions[ApplicationState.EditAndPlayMode][type] -= listener;
                    actions[ApplicationState.EditMode][type] -= listener;
                    actions[ApplicationState.PlayMode][type] -= listener;
                }
            }
            else
            {
                foreach (UnityEventType type in Enum.GetValues(typeof(UnityEventType)))
                {
                    actions[ApplicationState.EditAndPlayMode][type] -= listener;
                    actions[ApplicationState.EditMode][type] -= listener;
                    actions[ApplicationState.PlayMode][type] -= listener;
                }
            }
        } 

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [CALLBACK AND METHOD INVOKATION] ---

#if UNITY_EDITOR
                
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnLoad()
            => InvokeCallbacks(UnityEventType.OnLoad);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoadRuntimeMethod()
            => InvokeCallbacks(UnityEventType.BeforeSceneLoad);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoadRuntimeMethod()
            => InvokeCallbacks(UnityEventType.AfterSceneLoad);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AssembliesLoadedRuntimeMethod()
            => InvokeCallbacks(UnityEventType.AfterAssembliesLoaded);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistrationRuntimeMethod()
            => InvokeCallbacks(UnityEventType.SubsystemRegistration);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreenRuntimeMethod()
            => InvokeCallbacks(UnityEventType.BeforeSplashScreen);

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnRecompile()
            => InvokeCallbacks(UnityEventType.Recompile);
        
        private static void EditorApplicationDelayCall()
            => InvokeCallbacks(UnityEventType.InspectorUpdate);
        
#endif
        protected override void Awake()
        {
            base.Awake();
            InvokeCallbacks(UnityEventType.Awake);
        }
        
        private void Start() => InvokeCallbacks(UnityEventType.Start);

        private void Update() => InvokeCallbacks(UnityEventType.Update);
        private void LateUpdate() => InvokeCallbacks(UnityEventType.LateUpdate);
        private void FixedUpdate() => InvokeCallbacks(UnityEventType.FixedUpdate);
        private void OnApplicationQuit() => InvokeCallbacks(UnityEventType.ApplicationQuit);
        private void OnEnable() => InvokeCallbacks(UnityEventType.OnEnable);
        private void OnDisable() => InvokeCallbacks(UnityEventType.OnDisable);
        
        
#if UNITY_EDITOR
        private static void EditorApplicationOnQuitting() => InvokeCallbacks(UnityEventType.EditorApplicationQuit);
        
        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            InvokeCallbacks(state.GetUnityEventType());
            InvokeCallbacks(UnityEventType.TransitionEditPlayMode);
        }
#endif
        

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [INVOKE] ---

        /// <summary>
        /// Invoke callbacks depending on the context
        /// </summary>
        /// <param name="eventType">the type of the source event</param>
        /// <param name="delayInMilliseconds"></param>
        private static async void InvokeCallbacks(UnityEventType eventType, int delayInMilliseconds)
        {
            await Task.Delay(delayInMilliseconds);
            InvokeCallbacks(eventType);
        }
        
        /// <summary>
        /// Invoke callbacks depending on the context
        /// </summary>
        /// <param name="eventType">the type of the source event</param>
        private static void InvokeCallbacks(UnityEventType eventType)
        {
            if (Application.isPlaying)
            {
                callbacks[ApplicationState.PlayMode][eventType]?.Invoke(eventType);
                actions[ApplicationState.PlayMode][eventType]?.Invoke(); 
            }
            else
            {
                callbacks[ApplicationState.EditMode][eventType]?.Invoke(eventType);
                actions[ApplicationState.EditMode][eventType]?.Invoke();
            }
            callbacks[ApplicationState.EditAndPlayMode][eventType]?.Invoke(eventType);
            actions[ApplicationState.EditAndPlayMode][eventType]?.Invoke();
        }

        #endregion

#if UNITY_EDITOR
        public int callbackOrder => int.MinValue;
        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
            => InvokeCallbacks(UnityEventType.PreProcessorBuild);
#endif
    }
}
