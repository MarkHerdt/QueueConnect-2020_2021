#define ENABLE_WARNINGS

using System;
using UnityEngine;

namespace SoundSystem.Utils
{
    /// <summary>
    /// Abstract class for making scene persistent MonoBehaviour singletons.
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region --- [ACCESS] ---

        /// <summary>
        /// Get the active instance of the accessed type.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instanceCache != null) return instanceCache;
                
#if UNITY_EDITOR
                if (AssetImportHelper.IsImporting) return null;
#endif
                instanceCache = FindObjectOfType<T>();
                OnInstanceInitialized?.Invoke(instanceCache);
                return instanceCache;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [EVENTS] ---

        public static event Action OnInstanceDestroyed;
        public static event Action<T> OnInstanceInitialized;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [UTILITIES] ---
        
        /// <summary>
        /// Returns true if instance of type T is not null
        /// </summary>
        /// <param name="instance">null if return value is false</param>
        /// <returns></returns>
        public static bool TryGetInstance(out T instance)
        {
#if UNITY_EDITOR
            if (AssetImportHelper.IsImporting)
            {
                instance = instanceCache;
                return instanceCache != null;
            }
#endif

            if (instanceCache != null)
                instance = instanceCache;
            else
            {
                instanceCache = FindObjectOfType<T>();
                instance = instanceCache;
            }
            
            var all = FindObjectsOfType<T>();
            
            foreach (var type in all)
            {
                if(type != instanceCache)
                    DestroyImmediate(type.gameObject);
            }
        
            return instanceCache != null;
        }


        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [PRIVATE & PROTECTED FIELDS & PROPERTIES] ---

        private static T instanceCache;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INITIALIZATION] ---

        protected virtual void Awake()
        {
            if(this == null) return;
            transform.SetParent(null);

            instanceCache = this as T;
            OnInstanceInitialized?.Invoke(instanceCache);
            
            // Use the DontDestroyOnLoadHandler to keep track of every singleton we dont want to destroy on load.

            if(Application.isPlaying) DontDestroyOnLoad(Instance.gameObject);
            
#if UNITY_EDITOR
            if (AssetImportHelper.IsImporting) return;
#endif
            
            
            var otherObjectsOfType = FindObjectsOfType<T>();
            if (otherObjectsOfType.Length <= 1) return;
            
            // We destroy every object except this if we find multiple instances. 
            foreach (var obj in otherObjectsOfType)
            {
                if (obj == this) continue;
#if ENABLE_WARNINGS
                Debug.LogWarning($"Singleton: Multiple instances of the same type {GetType()} detected! " +
                                 $"Destroyed other GameObject {obj.gameObject} instance!");
#endif
                DestroyImmediate(obj.gameObject);
            }  
        }        

        #endregion

        #region --- [TERMINATION] ---

        protected virtual void OnDestroy()
        {
            try
            {
                if (instanceCache != this) return;
                instanceCache = null;
                OnInstanceDestroyed?.Invoke();
            }
            catch
            {
                return;
            }
        }

        #endregion
    }
}