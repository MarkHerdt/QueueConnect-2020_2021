using Ganymed.Utils.ExtensionMethods;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEngine;

namespace Ganymed.Utils.Optimization
{
    /// <summary>
    /// Component will destroy the gameObject during Awake, OnEnable or Start
    /// </summary>
    [AddComponentMenu("Optimization/Destroy OnLoad", 100)]
    public sealed class DestroyOnLoad : TransformOptimizationComponent
    {
        [HideInInspector] [SerializeField] internal bool enable = true;
        [HideInInspector] [SerializeField] internal bool deleteObjectsWithComponents = true;
        [HideInInspector] [SerializeField] internal bool deleteObjectsWithChildren = true;
        
        internal override void Initialize(InitializationEvents? @event)
        {
            if(@event != executeOn && @event != null) return;
            
            if (!enable)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning: Component must be enabled if you want to destroy the GameObject! [{gameObject.name}]");
                return;
            }
            
            if (!OptimizationSettings.Instance.EnableDestroyOnLoad)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning: DestroyOnLoad must be enabled in the Hierarchy-Optimization-Configuration if you want to destroy the GameObject! [{gameObject.name}]");
                return;
            }
            
            if (!Application.isPlaying && !enableInEditMode)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning: Application must be running if you want to automatically destroy the GameObject! [{gameObject.name}]");
                return;
            }

            if (transform.childCount > 0 && !deleteObjectsWithChildren)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning: Cannot destroy GameObject with children! [{gameObject.name}]");
                return;
            }
            
            
            if (transform.GetComponents<Component>().Length > 2 && !deleteObjectsWithComponents)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning: Cannot destroy GameObject with components! [{gameObject.name}]");
                return;
            }
            
            
            if(@event == executeOn || @event == null)
                DestroySafe(gameObject);
        }
        
        private static void DestroySafe(Object obj)
        {
            if(Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        
        
        
        static void DestroyComponentsWithTag()
        {
            
        }
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DestroyOnLoad), true), UnityEditor.CanEditMultipleObjects]
    internal class DestroyOnLoadInspector : UnityEditor.Editor
    {
        private DestroyOnLoad Target;

        private void OnEnable()
        {
            Target = (DestroyOnLoad) target;
        }

        public override void OnInspectorGUI()
        {
            if (!OptimizationSettings.Instance.EnableDestroyOnLoad)
            {
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.HelpBox(
                    $"Automatic DestroyOnLoad is disabled. It can be enabled in the {nameof(OptimizationSettings)}.",
                    UnityEditor.MessageType.Info);
                if(GUILayout.Button("Edit Settings", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    OptimizationSettings.EditSettings();
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
                UnityEditor.EditorGUILayout.Space();
            }
            
            Target.enable = UnityEditor.EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.enable).AsLabel(), Target.GetTooltip(nameof(Target.enable))),
                Target.enable);
            
            Target.executeOn = (InitializationEvents) UnityEditor.EditorGUILayout.EnumPopup(
                new GUIContent(nameof(Target.executeOn).AsLabel(), Target.GetTooltip(nameof(Target.executeOn))),
                Target.executeOn);
            
            UnityEditor.EditorGUILayout.Space();
            
            Target.deleteObjectsWithChildren = UnityEditor.EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.deleteObjectsWithChildren).AsLabel(), Target.GetTooltip(nameof(Target.deleteObjectsWithChildren))),
                Target.deleteObjectsWithChildren);
            
            Target.deleteObjectsWithComponents = UnityEditor.EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.deleteObjectsWithComponents).AsLabel(), Target.GetTooltip(nameof(Target.deleteObjectsWithComponents))),
                Target.deleteObjectsWithComponents);
            
            UnityEditor.EditorGUILayout.Space();
            
            
            Target.enableInEditMode = UnityEditor.EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.enableInEditMode).AsLabel(), Target.GetTooltip(nameof(Target.enableInEditMode))),
                Target.enableInEditMode);

            if (Target.enableInEditMode)
            {
                if (GUILayout.Button("Destroy"))
                {
                    Target.Initialize(null);
                }
            }
        }
    }
#endif
}