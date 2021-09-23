#define ENABLE_WARNINGS
//#define ENABLE_CREATION

using System.IO;
using UnityEngine;

namespace SoundSystem.Utils
{
    /// <summary>
    /// Generic base class for singleton instances of setting files.
    /// Do not refactor or alter contents of this class if you are not 100% sure about what you're doing!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Settings<T> : ScriptableSettings where T : ScriptableSettings
    {
        
        /// <summary>
        /// The name of the asset file. We use the name of the type to to keep this class generic.
        /// For this reason it is recommended to use fitting and meaningful names for setting classes!
        /// </summary>
        private static string SettingsAssetName => typeof(T).Name.Contains("Settings") ? typeof(T).Name : $"{typeof(T).Name}Settings";

        // private backfield cache of the instance.
        private static T instance = null;                   
        
        // ReSharper disable once StaticMemberInGenericType
        // This field is required for multithreading environments. 
        /// <summary>
        /// This field is important if we are initializing or accessing the instance in a multi-threaded environment.
        /// </summary>
        private static volatile bool isInitializing = false;
        

        /// <summary>
        /// The singleton instance of the related type.
        /// A new instance of the accessed type will be created if none can be found. 
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                
                if (isInitializing)
                {
                    return null;
                }
                
                isInitializing = true;
                instance = Resources.Load(SettingsAssetName) as T;
                if (instance == null)
                {
#if ENABLE_CREATION
#if UNITY_EDITOR

                    if (AssetImportHelper.IsImporting)
                    {
                        isInitializing = false;
                        return null;
                    }
#endif
   
#if ENABLE_WARNINGS
                    Debug.LogWarning($"Cannot find {typeof(T).Name} file, creating default settings");
#endif

                    instance = CreateInstance<T>();
                    instance.name = typeof(T).ToString();

#if UNITY_EDITOR
                     if (!Directory.Exists($"{instance.FilePath()}/Resources"))
                     {
                         Directory.CreateDirectory(Path.GetDirectoryName($"{instance.FilePath()}/Resources") ?? "Assets/Resources");
                         UnityEditor.AssetDatabase.CreateFolder(instance.FilePath(), "Resources");
                     }
                        
                    UnityEditor.AssetDatabase.CreateAsset(instance, $"{instance.FilePath()}/Resources/{SettingsAssetName}.asset");
#endif
#endif    
                }
                isInitializing = false;
                return instance;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
