using QueueConnect.Development;
using UnityEngine;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Class to create Singletons
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Singleton : MonoBehaviour
    {
        /// <summary>
        /// Creates a temporary Singleton of the passed type <br/>
        /// Use "this" as the passed Parameter -> instance = Singleton.Temporary(this); <br/>
        /// Singleton will be destroyed when a new Scene is loaded
        /// </summary>
        /// <typeparam name="T">T must be a Component</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_ForcePassedInstance">Makes the passed Object a Singleton, even if an Instance already exists</param>
        /// <returns>Returns the Singleton or null</returns>
        public static T Temporary<T>(T _Instance, bool _ForcePassedInstance = false) where T : Component 
        {
            // When the passed Object isn't null
            if (_Instance != null)
            {
                return _Instance;
            }
            // When the passed Object is null
            var _objects = FindObjectsOfType<T>();
                
            switch (_objects.Length)
            {
                case 0:                           // When there's no instance of that Type in the Scene
                    _Instance = CreateSingleton(_Instance, HideFlags.HideAndDontSave);
                    break;
                
                case 1 when _ForcePassedInstance: // When there's one instance of that Type in the Scene
                    _Instance = CreateSingleton(_Instance, HideFlags.HideAndDontSave);
                    break;
                
                case 1:                          // When there's one instance of that Type in the Scene
                    Destroy(_Instance);
                    return _objects[0];
                
                default:                         // When there's more then one Object of that Type in the Scene
                {
                    if (_ForcePassedInstance)
                    {
                        _Instance = CreateSingleton(_Instance, HideFlags.HideAndDontSave);
                    }
                    else
                    {
                        DebugLog.Red_White_Red("More than one Object of Type ", $"{typeof(T)}", " found");
                        DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to force the passed Object to become the new Singleton");
                        
                        Destroy(_Instance);
                        return null;
                    }
                    break;
                }
            }

            return _Instance;
        }

        /// <summary>
        /// Creates a persistent Singleton of the passed type <br/>
        /// Use "this" as the passed Parameter -> instance = Singleton.Persistent(this) <br/>
        /// Singleton will not be destroyed when a new Scene is loaded
        /// </summary>
        /// <typeparam name="T">T must be a Component</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_ForcePassedInstance">Makes the passed Object a Singleton, even if an Instance already exists</param>
        /// <returns>Returns the Singleton or null</returns>
        public static T Persistent<T>(T _Instance, bool _ForcePassedInstance = false) where T : Component
        {
            // When the passed Object isn't null
            if (_Instance != null)
            {
                return _Instance;
            }
            // When the passed Object is null
            var _objects = FindObjectsOfType<T>();

            switch (_objects.Length)
            {
                case 0:                           // When there's no instance of that Type in the Scene
                    _Instance = CreateSingleton(_Instance, HideFlags.DontUnloadUnusedAsset, true);
                    break;
                
                case 1 when _ForcePassedInstance: // When there's one instance of that Type in the Scene
                    _Instance = CreateSingleton(_Instance, HideFlags.DontUnloadUnusedAsset, true);
                    break;

                case 1:                          // When there's one instance of that Type in the Scene
                    Destroy(_Instance);
                    return _objects[0];
                
                default:                         // When there's more then one Object of that Type in the Scene
                {
                    if (_ForcePassedInstance)
                    {
                        _Instance = CreateSingleton(_Instance, HideFlags.DontUnloadUnusedAsset, true);
                    }
                    else
                    {
                        DebugLog.Red_White_Red("More than one Object of Type ", $"{typeof(T)}", " found");
                        DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to force the passed Object to become the new Singleton");
                        
                        Destroy(_Instance);
                        return null;
                    }
                    break;
                }
            }

            return _Instance;
        }

        /// <summary>
        /// Creates a Singleton Object
        /// </summary>
        /// <typeparam name="T">T must be a Component</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_HideFlags">Hierarchy and Inspector visibility of the Object</param>
        /// <returns>Returns the Singleton</returns>
        private static T CreateSingleton<T>(T _Instance, HideFlags _HideFlags = HideFlags.None, bool _Persistent = false) where T : Component
        {
            // Creates a GameObject to attach the singleton to
            var _singletonObject = new GameObject {hideFlags = _HideFlags};
            // Sets the visibility of the Object
            // Adds the Component of the passed Type to the Object
            _Instance = _singletonObject.AddComponent<T>();

            if (_Persistent)
            {
                // Object won't be destroyed when a new Scene is loaded
                DontDestroyOnLoad(_Instance);
            }

            return _Instance;
        }

        /// <summary>
        /// Creates a temporary Singleton of the passed type from a ScriptableObject File in the Resources Folder <br/>
        /// Use "instance" as the passed Parameter -> instance = Singleton.Temporary(instance); <br/>
        /// Singleton will be destroyed when a new Scene is loaded
        /// </summary>
        /// <typeparam name="T">T must be a ScriptableObject</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_FilePath">Filepath from the Resources Folder, the ScriptableObject is in</param>
        /// <param name="_ForcePassedInstance">Makes the passed Object a Singleton, even if an Instance already exists</param>
        /// <returns>Returns the Singleton or null</returns>
        public static T Temporary<T>(T _Instance, string _FilePath, bool _ForcePassedInstance = false) where T : ScriptableObject
        {
            // When the passed Object isn't null
            if (_Instance != null)
            {
                // Overrides the existing Singleton
                if (_ForcePassedInstance)
                {
                    _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.HideAndDontSave);
                }
                else
                {
                    DebugLog.Red("The passed Object is not null");
                    DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to override it and create a new Singleton");
                }
            }
            // When the passed Object is null
            else
            {
                var _objects = FindObjectsOfType<T>();
                
                switch (_objects.Length)
                {
                    case 0:                           // When there's no instance of that Type in the Scene
                        _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.HideAndDontSave);
                        break;
                    
                    case 1 when _ForcePassedInstance: // When there's one instance of that Type in the Scene
                        _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.HideAndDontSave);
                        break;
                    
                    case 1:                           // When there's one instance of that Type in the Scene
                        Destroy(_Instance);
                        return _objects[0];
                    
                    default:                          // When there's more then one Object of that Type in the Scene
                    {
                        if (_ForcePassedInstance)
                        {
                            _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.HideAndDontSave);
                        }
                        else
                        {
                            DebugLog.Red_White_Red("More than one Object of Type ", $"{typeof(T)}", " found");
                            DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to force the passed Object to become the new Singleton");
                        
                            Destroy(_Instance);
                            return null;
                        }
                        break;
                    }
                }
            }

            return _Instance;
        }

        /// <summary>
        /// Creates a persistent Singleton of the passed type from a ScriptableObject File in the Resources Folder <br/>
        /// Use "instance" as the passed Parameter -> instance = Singleton.Persistent(instance); <br/>
        /// Singleton will not be destroyed when a new Scene is loaded
        /// </summary>
        /// <typeparam name="T">T must be a ScriptableObject</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_FilePath">Filepath from the Resources Folder, the ScriptableObject is in</param>
        /// <param name="_ForcePassedInstance">Makes the passed Object a Singleton, even if an Instance already exists</param>
        /// <returns>Returns the Singleton or null</returns>
        public static T Persistent<T>(T _Instance, string _FilePath, bool _ForcePassedInstance = false) where T : ScriptableObject
        {
            // When the passed Object isn't null
            if (_Instance != null)
            {
                // Overrides the existing Singleton
                if (_ForcePassedInstance)
                {
                    _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.DontUnloadUnusedAsset, true);
                }
                else
                {
                    DebugLog.Red("The passed Object is not null");
                    DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to override it and create a new Singleton");
                }
            }
            // When the passed Object is null
            else
            {
                var _objects = FindObjectsOfType<T>();
                
                switch (_objects.Length)
                {
                    case 0:                           // When there's no instance of that Type in the Scene
                        _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.DontUnloadUnusedAsset, true);
                        break;
                    
                    case 1 when _ForcePassedInstance: // When there's one instance of that Type in the Scene
                        _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.DontUnloadUnusedAsset, true);
                        break;
                    
                    case 1:                           // When there's one instance of that Type in the Scene
                        Destroy(_Instance);
                        return _objects[0];
                    
                    default:                          // When there's more then one Object of that Type in the Scene
                    {
                        if (_ForcePassedInstance)
                        {
                            _Instance = LoadScriptableObject(_Instance, _FilePath, HideFlags.DontUnloadUnusedAsset, true);
                        }
                        else
                        {
                            DebugLog.Red_White_Red("More than one Object of Type ", $"{typeof(T)}", " found");
                            DebugLog.Red($"Set \"{nameof(_ForcePassedInstance)}\" to \"true\", to force the passed Object to become the new Singleton");
                        
                            Destroy(_Instance);
                            return null;
                        }
                        break;
                    }
                }
            }

            return _Instance;
        }

        /// <summary>
        /// Loads a ScriptableObject File from the Resources Folder
        /// </summary>
        /// <typeparam name="T">T must be a ScriptableObject</typeparam>
        /// <param name="_Instance">Object to make a Singleton of</param>
        /// <param name="_FilePath">Filepath from the Resources Folder, the ScriptableObject is in</param>
        /// <param name="_HideFlags">Hierarchy and Inspector visibility of the Object</param>
        /// <param name="_Persistent">If true, the Object wont be destroyed when a new Scene is loaded</param>
        /// <returns>Returns the Singleton</returns>
        private static T LoadScriptableObject<T>(T _Instance, string _FilePath, HideFlags _HideFlags = HideFlags.None, bool _Persistent = false) where T : ScriptableObject
        {
            // Loads the ScriptableObject
            _Instance = Resources.Load<T>(_FilePath);

            // When the File couldn't be found
            if (_Instance == null)
            {
                DebugLog.Red_White_Red("The ScriptableObject ", $"{typeof(T)}", "couldn't be found in the Resources Folder");
                return null;
            }

            // Sets the visibility of the Object
            _Instance.hideFlags = _HideFlags;

            if (_Persistent)
            {
                // Object won't be destroyed when a new Scene is loaded
                DontDestroyOnLoad(_Instance);
            }

            return _Instance;
        }
    }
}