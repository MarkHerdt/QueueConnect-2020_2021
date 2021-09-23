using Ganymed.Utils.Attributes;
using Ganymed.Utils.Callbacks;
using Ganymed.Utils.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace Ganymed.Utils.Editor
{
    [InitializeOnLoad]
    public static class HideFlagsUtility
    {
        [MenuItem("Help/Hide Flags/Show All Hidden Objects")]
        private static void ShowAll()
        {
            var allGameObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allGameObjects)
            {
                switch (go.hideFlags)
                {
                    case HideFlags.HideAndDontSave:
                        go.hideFlags = HideFlags.DontSave;
                        break;
                    case HideFlags.HideInHierarchy:
                    case HideFlags.HideInInspector:
                        go.hideFlags = HideFlags.None;
                        break;
                }
            }
            
            EditorApplication.RepaintHierarchyWindow();
        }

        static HideFlagsUtility()
        {
            UnityEventCallbacks.AddEventListener(
                Validate,
                UnityEventType.Awake);
            
            UnityEventCallbacks.AddEventListener(
                Validate,
                ApplicationState.EditMode,
                UnityEventType.OnLoad);
        }

        [MenuItem("Help/Hide Flags/Validate Hide Flags")]
        private static void ValidateHideInInspectorAttribute() => Validate(UnityEventType.Recompile);
        private static void Validate(UnityEventType origin)
        {
            var monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();
            
            foreach (var monoBehaviour in monoBehaviours)
            {
                foreach (var attribute in monoBehaviour.GetType().GetNullableUnderlying().GetCustomAttributes(true))
                {
                    switch (attribute)
                    {
                        case GameObjectHideFlagsAttribute obj:
                            var gameObject = monoBehaviour.gameObject;
                            if (gameObject.hideFlags != obj.hideFlags)
                            {
                                gameObject.hideFlags = obj.hideFlags;
                                EditorUtility.SetDirty(gameObject);
                            }
                            break;
                        
                        case HideFlagsAttribute script:
                            if (monoBehaviour.hideFlags != script.hideFlags)
                            {
                                monoBehaviour.hideFlags = script.hideFlags;    
                                EditorUtility.SetDirty(monoBehaviour);
                            }
                            break;
                    }
                }
            }
            
            EditorApplication.DirtyHierarchyWindowSorting();
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}