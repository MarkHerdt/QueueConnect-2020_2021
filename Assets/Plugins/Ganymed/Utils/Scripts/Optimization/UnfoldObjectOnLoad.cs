using System.Linq;
using Ganymed.Utils.ExtensionMethods;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ganymed.Utils.Optimization
{
    /// <summary>
    /// Component that will delete its gameObject and unpack its children.
    /// </summary>
    [AddComponentMenu("Optimization/Unfold Object OnLoad", 100)]
    public sealed class UnfoldObjectOnLoad : TransformOptimizationComponent
    {
        #region --- [INSPECTOR] ---
        
        [Tooltip("If enabled, the parent transform of the children of this GameObject will automatically be set to the " +
                 "target transform at the beginning of the game. Afterwards this GameObject will be destroyed.")]
        [HideInInspector] [SerializeField] internal bool enable = true;
        
        [Tooltip("The new transform to which the children of the gameObject should be assigned to. Null if root")]
        [HideInInspector] [SerializeField] internal Parent parent = Parent.Parent;
        
        [Tooltip("Custom target transform.")]
        [HideInInspector] [SerializeField] public Transform customParentTransform = null;

        [Tooltip("if enabled, this GameObject will be destroyed even if other components are present.")]
        [SerializeField] [HideInInspector] internal bool deleteUnfoldObjectsWithComponents = false;

        #endregion

        #region --- [FIELDS] ---

        private Transform gameObjectTransform;
        private Transform parentTransform = null;

        #endregion
        
        internal override void Initialize(InitializationEvents? @event)
        {
            if(@event!= null && @event != executeOn)return;
            if(!enable || !OptimizationSettings.Instance.EnableUnfoldingOnLoad) return;
            
            if (!Application.isPlaying && !enableInEditMode)
            {
                if(OptimizationSettings.Instance.EnableComponentWarnings)
                    Debug.LogWarning("Application must be running to unfold objects");
                return;
            }
            
            gameObjectTransform = transform;
            switch (parent)
            {
                case Parent.Root:
                    parentTransform = null;
                    break;
                case Parent.Parent:
                    parentTransform = gameObjectTransform.parent;
                    break;
                case Parent.Custom:
                    parentTransform = customParentTransform;
                    break;
                default:
                    parentTransform = null;
                    break;
            }


            while (transform.childCount > 0)
            {
                gameObjectTransform.GetChild(0).SetParent(parentTransform);         
            }

            var num = GetComponents<Component>().Count(
                component => component.GetType() != typeof(Transform) && component.GetType() != typeof(UnfoldObjectOnLoad));
        
            if (num > 0)
            {
                if (!deleteUnfoldObjectsWithComponents)
                {
                    if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                        $"Warning! GameObject flagged for deletion contained {num} Component/s and was NOT destroyed!");
                    return;
                }
                if(OptimizationSettings.Instance.EnableComponentWarnings) Debug.LogWarning(
                    $"Warning! GameObject flagged for deletion contained {num} Component/s and WAS destroyed!");
                
                DestroySafe(gameObject);
            }
            else
            {
                DestroySafe(gameObject);
            }
        }

        private static void DestroySafe(Object obj)
        {
            if(Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }
    }

    #region --- [EDITOR] ---

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UnfoldObjectOnLoad), true), UnityEditor.CanEditMultipleObjects]
    internal class UnfoldObjectEditor : UnityEditor.Editor
    {
        private UnfoldObjectOnLoad Target;

        private void OnEnable()
        {
            Target = (UnfoldObjectOnLoad) target;
        }

        public override void OnInspectorGUI()
        {
            if (!OptimizationSettings.Instance.EnableUnfoldingOnLoad)
            {
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.HelpBox(
                    $"Automatic GameObject unfolding is disabled. It can be enabled in the {nameof(OptimizationSettings)}.",
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
            
                Target.deleteUnfoldObjectsWithComponents = UnityEditor.EditorGUILayout.Toggle(
                    new GUIContent("Delete Objects with Components", Target.GetTooltip(nameof(Target.deleteUnfoldObjectsWithComponents))),
                    Target.deleteUnfoldObjectsWithComponents);
            
                Target.enableInEditMode = UnityEditor.EditorGUILayout.Toggle(
                    new GUIContent("Enable In Edit-Mode", Target.GetTooltip(nameof(Target.enableInEditMode))),
                    Target.enableInEditMode);
            
            
                UnityEditor.EditorGUILayout.Space();
                if (!Target.enableInEditMode) return;
                if (GUILayout.Button("Unfold (Warning: This GameObject will be destroyed!)"))
                {
                    Target.Initialize(null);
                }
            }
            UnityEditor.EditorUtility.SetDirty(Target);
        }
    }
#endif

    #endregion
}
