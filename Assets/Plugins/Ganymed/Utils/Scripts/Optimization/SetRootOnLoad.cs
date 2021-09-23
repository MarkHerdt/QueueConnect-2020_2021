using Ganymed.Utils.ExtensionMethods;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEngine;

namespace Ganymed.Utils.Optimization
{
    /// <summary>
    /// Component will set the parent of its gameObject during Awake, OnEnable or Start
    /// </summary>
    [AddComponentMenu("Optimization/Set Root OnLoad", 100)]
    public sealed class SetRootOnLoad : TransformOptimizationComponent
    {
        
         #region --- [FIELDS] ---
        [Tooltip("If enabled, The parent transform of this gameObject will automatically be set to the target at the start of the game")]
        [HideInInspector] [SerializeField] internal bool enable = true;
        
        [Tooltip("The new transform to which the gameObject should be assigned to. Null if root")]
        [HideInInspector] [SerializeField] internal Parent parent = Parent.Root;
        
        [Tooltip("Custom target transform")]
        [HideInInspector] [SerializeField] public Transform customParentTransform = null;
        
        #endregion


        internal override void Initialize(InitializationEvents? @event)
        {
            if(@event!= null && @event != executeOn)return;
            if(!enable || !OptimizationSettings.Instance.EnableSetRootOnLoad) return;
            
            if (!Application.isPlaying && !enableInEditMode)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    "Application must be running if you want to automatically set the root of the GameObject");
                return;
            }
            
            switch (parent)
            {
                case Parent.Root:
                    transform.SetParent(null);
                    break;
                case Parent.Parent:
                    transform.SetParent(transform.parent);
                    break;
                case Parent.Custom:
                    transform.SetParent(customParentTransform);
                    break;
                default:
                    transform.SetParent(null);
                    break;
            }

            if (Application.isPlaying)
            {
                Destroy(this);
            }
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SetRootOnLoad), true), UnityEditor.CanEditMultipleObjects]
    internal class SetRootOnLoadInspector : UnityEditor.Editor
    {
        private SetRootOnLoad Target;
        private UnityEditor.SerializedObject serializableObject;
        private UnityEditor.SerializedProperty serializableProperty;

        private void OnEnable()
        {
            Target = (SetRootOnLoad) target;
            serializableObject = new UnityEditor.SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            if (!OptimizationSettings.Instance.EnableSetRootOnLoad)
            {
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.HelpBox(
                    $"Automatic transform rooting of GameObjects is disabled. It can be enabled in the {nameof(OptimizationSettings)}.",
                    UnityEditor.MessageType.Info);
                if(GUILayout.Button("Edit Settings", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    OptimizationSettings.EditSettings();
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
                UnityEditor.EditorGUILayout.Space();
            }
            
            Target.enable = UnityEditor.EditorGUILayout.Toggle(
                new GUIContent("Enable", Target.GetTooltip(nameof(Target.enable))),
                Target.enable);
            
            if (Target.enable)
            {
                UnityEditor.EditorGUILayout.Space();
                
                Target.parent = (Parent) UnityEditor.EditorGUILayout.EnumPopup(
                    new GUIContent("Target Parent Transform", Target.GetTooltip(nameof(Target.parent))),
                    Target.parent);
                
                if (Target.parent == Parent.Custom)
                {
                    Target.customParentTransform =
                        UnityEditor.EditorGUILayout.ObjectField(
                            new GUIContent("Transform", Target.GetTooltip(nameof(Target.customParentTransform))),
                            Target.customParentTransform,
                            typeof(Transform), true) as Transform;
                }
                
                Target.executeOn = (InitializationEvents) UnityEditor.EditorGUILayout.EnumPopup(
                    new GUIContent("Execute On", Target.GetTooltip(nameof(Target.executeOn))),
                    Target.executeOn);
                
                UnityEditor.EditorGUILayout.Space();
                
                Target.enableInEditMode = UnityEditor.EditorGUILayout.Toggle(
                    new GUIContent(nameof(Target.enableInEditMode).AsLabel(), Target.GetTooltip(nameof(Target.enableInEditMode))),
                    Target.enableInEditMode);
                
                UnityEditor.EditorGUILayout.Space();
            
                if (!Target.enableInEditMode) return;
                if (GUILayout.Button("SetParent"))
                {
                    Target.Initialize(null);
                }
            }
            
            UnityEditor.EditorUtility.SetDirty(Target);
        }
    }
#endif
}
