using System.Collections.Generic;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Singleton;
using UnityEngine;

namespace Ganymed.Utils.Optimization
{
    public sealed class OptimizationSettings : Settings<OptimizationSettings>
    {
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Ganymed/Edit Optimization Settings", priority = 40)]
        public static void EditSettings()
        {
            SelectObject(Instance);
        }
#endif
        
        #region --- [FIELDS (HIERARCHY)] ---
        
        [SerializeField] [HideInInspector] private bool enableUnfoldingOnLoad = true;
        [SerializeField] [HideInInspector] private bool enableSetRootOnLoad = true;
        [SerializeField] [HideInInspector] private bool enableDestroyOnLoad = true;
        
        [Tooltip("When enabled, allows warning messages send by individual components.")]
        [SerializeField] [HideInInspector] private bool enableComponentWarnings = false;
        [Tooltip("When enabled, allows notification logs.")]
        [SerializeField] [HideInInspector] private bool enableLogs = true;

        [SerializeField] [HideInInspector] private bool hideComponentsInInspector = false;
        [SerializeField] [HideInInspector] private bool hideComponentsInInspectorCache = false;
        
        #endregion

        #region --- [PROPERTIES] ---

        public bool EnableUnfoldingOnLoad
        {
            get => enableUnfoldingOnLoad;
            set => enableUnfoldingOnLoad = value;
        }
        
        public bool EnableSetRootOnLoad
        {
            get => enableSetRootOnLoad;
            set => enableSetRootOnLoad = value;
        }
        
        public bool EnableDestroyOnLoad
        {
            get => enableDestroyOnLoad;
            set => enableDestroyOnLoad = value;
        }
        
        public bool EnableComponentWarnings
        {
            get => enableComponentWarnings;
            set => enableComponentWarnings = value;
        }
        
        public bool EnableLogs
        {
            get => enableLogs;
            set => enableLogs = value;
        }
        
        public bool HideComponentsInInspector
        {
            get => hideComponentsInInspector;
            set => hideComponentsInInspector = value;
        }
        
        public bool HideComponentsInInspectorCache
        {
            get => hideComponentsInInspectorCache;
            set => hideComponentsInInspectorCache = value;
        }
        
        #endregion

        #region --- [VALIDATE] ---

        public void Validate()
        {
            if (hideComponentsInInspector != hideComponentsInInspectorCache)
            {

                var value = hideComponentsInInspector ? HideFlags.HideInInspector : HideFlags.None;
                foreach (var obj in optimizationComponents)
                {
                    obj.hideFlags = value;
                }
            }

            hideComponentsInInspectorCache = hideComponentsInInspector;
        }
        
        

        #endregion
        
        #region --- [STATIC] ---

        public static readonly List<TransformOptimizationComponent> optimizationComponents = new List<TransformOptimizationComponent>();

        public static void AddOptimizationComponent(TransformOptimizationComponent component)
        {
            optimizationComponents.AddIfNotInList(component);
        }
        
        public static void RemoveOptimizationComponent(TransformOptimizationComponent component)
        {
            try
            {
                optimizationComponents.Remove(component);
            }
            catch
            {
                // ignored
            }
        }
        
        #endregion
    }
}