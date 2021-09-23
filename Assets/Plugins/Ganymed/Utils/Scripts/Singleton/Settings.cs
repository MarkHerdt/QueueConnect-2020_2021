using System.IO;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Helper;
using UnityEngine;

namespace Ganymed.Utils.Singleton
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
        private static string SettingsAssetName => (typeof(T).Name.Contains("Settings") ? typeof(T).Name : $"{typeof(T).Name}Settings").Replace("Scriptable", "");

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
                if (isInitializing)
                {
                    return null;
                }

                if (instance != null) return instance;
                
                isInitializing = true;
                instance = Resources.Load(SettingsAssetName) as T;
                if (instance == null)
                {
#if UNITY_EDITOR
                    
                    if (AssetImportHelper.IsImporting)
                    {
                        isInitializing = false;
                        return null;
                    }
#endif
                    
                    Debug.LogWarning($"Cannot find {typeof(T).Name} file, creating default settings");
                        
                    instance = CreateInstance<T>();
                    instance.name = typeof(T).ToString();

#if UNITY_EDITOR
                    var filePath = instance.AssetPath().Contains("/Resources")?
                        instance.AssetPath() : $"{instance.AssetPath()}/Resources";
                    
                    const string Assets = "Assets";
                    
                    // --- Validate if the folder does exist
                    if (!UnityEditor.AssetDatabase.IsValidFolder(filePath))
                    {
                        // --- If the file path does not exit create each folder after another. 
                        var folders = filePath.Split('/');
                        var path = Assets;
                        
                        foreach (var folder in folders)
                        {
                            UnityEditor.AssetDatabase.Refresh();
                            var parent = path;
                            path = $"{path}/{folder}";
                            if (!UnityEditor.AssetDatabase.IsValidFolder(path))
                            {
                                UnityEditor.AssetDatabase.CreateFolder(parent, folder);
                            }
                        }
                        UnityEditor.AssetDatabase.Refresh();
                    }
                    
                    UnityEditor.AssetDatabase.CreateAsset(instance, $"{Assets}/{filePath}/{SettingsAssetName}.asset");
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
        
#if UNITY_EDITOR

        private void OnEnable()
        {
            Initialize();
        }

        private static async void Initialize()
        {
            var result = await TaskHelper.AwaitConditionSuccessSafe(() => !AssetImportHelper.IsImporting, 100, AwaitLogs.None, 10000);
            if(result) instance = Instance ;
        }
#endif
    }
}
