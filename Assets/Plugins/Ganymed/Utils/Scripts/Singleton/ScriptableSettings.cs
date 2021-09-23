using UnityEditor;
using UnityEngine;

namespace Ganymed.Utils.Singleton
{
    /// <summary>
    /// Base class for singleton instances for setting files.
    /// </summary>
    public abstract class ScriptableSettings : ScriptableObject
    {
        /// <summary>
        /// The filepath in which the settings file will be stored.
        /// </summary>
        /// <returns></returns>
        public virtual string AssetPath() => "Settings";

        #region --- [EDITOR SELECTION] ---
        
#if UNITY_EDITOR
        /// <summary>
        /// Method will select the object instance.
        /// </summary>
        /// <param name="target"></param>
        protected internal static void SelectObject(Object target)
        {
            Selection.activeObject = target;
#if UNITY_2018_2_OR_NEWER
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
#else
            UnityEditor.EditorApplication.ExecuteMenuItem("Window/Inspector");
#endif
        }
#endif
        #endregion
    }
}