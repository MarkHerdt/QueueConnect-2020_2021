using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Optimization;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace Ganymed.Utils.Editor
{
    [CustomEditor(typeof(OptimizationSettings), true), CanEditMultipleObjects]
    internal class OptimizationSettingsInspector : UnityEditor.Editor
    {
        private OptimizationSettings Target;
        
        private void OnEnable()
        {
            Target = (OptimizationSettings) target;
        }
    
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Optimization", GUIHelper.H1);
            GUIHelper.DrawLine();
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This is a WIP custom configuration for optimization helper.", MessageType.Info);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Hierarchy", GUIHelper.H2);
            GUIHelper.DrawLine();
            
            Target.EnableUnfoldingOnLoad = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.EnableUnfoldingOnLoad).AsLabel(), Target.GetTooltip(nameof(Target.EnableUnfoldingOnLoad))),
                Target.EnableUnfoldingOnLoad
            );
            Target.EnableSetRootOnLoad = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.EnableSetRootOnLoad).AsLabel(), Target.GetTooltip(nameof(Target.EnableSetRootOnLoad))),
                Target.EnableSetRootOnLoad
            );
            Target.EnableDestroyOnLoad = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.EnableDestroyOnLoad).AsLabel(), Target.GetTooltip(nameof(Target.EnableDestroyOnLoad))),
                Target.EnableDestroyOnLoad
            );
            
            EditorGUILayout.Space();
            Target.EnableComponentWarnings = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.EnableComponentWarnings).AsLabel(), Target.GetTooltip(nameof(Target.EnableComponentWarnings))),
                Target.EnableComponentWarnings);
            
            Target.EnableLogs = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.EnableLogs).AsLabel(), Target.GetTooltip(nameof(Target.EnableLogs))),
                Target.EnableLogs);
            
            Target.HideComponentsInInspector = EditorGUILayout.Toggle(
                new GUIContent(nameof(Target.HideComponentsInInspector).AsLabel(), Target.GetTooltip(nameof(Target.HideComponentsInInspector))),
                Target.HideComponentsInInspector);
            
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(nameof(UnfoldObjectOnLoad).AsLabel("Select\n"), GUILayout.Height(40)))
            {
                SelectionHelper.SelectComponents<UnfoldObjectOnLoad>(true, Target.EnableLogs);
            }
            
            if (GUILayout.Button(nameof(SetRootOnLoad).AsLabel("Select\n"),  GUILayout.Height(40)))
            {
                SelectionHelper.SelectComponents<SetRootOnLoad>(true, Target.EnableLogs);
            }
            
            if (GUILayout.Button(nameof(DestroyOnLoad).AsLabel("Select\n"), GUILayout.Height(40)))
            {
                SelectionHelper.SelectComponents<DestroyOnLoad>(true, Target.EnableLogs);
            }
            
            if (GUILayout.Button("Select\nAll Components", GUILayout.Height(40)))
            {
                SelectionHelper.SelectComponents<TransformOptimizationComponent>(true, Target.EnableLogs);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorUtility.SetDirty(Target);
            Target.Validate();
        }
    }
}