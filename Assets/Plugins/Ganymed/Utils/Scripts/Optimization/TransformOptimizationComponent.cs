using System;
using Ganymed.Utils.Attributes;
using UnityEngine;

namespace Ganymed.Utils.Optimization
{
    [DisallowMultipleComponent]
    [ScriptOrder(-5000)]
    public abstract class TransformOptimizationComponent : MonoBehaviour
    {
        [Tooltip("Determine at which point during the initialization script is executed.")]
        [HideInInspector] [SerializeField] public InitializationEvents executeOn;
        
        [Tooltip("Allow manual script is execution in edit mode.")]
        [SerializeField] [HideInInspector] internal bool enableInEditMode = false;
        
        #region --- [MISC] ---
        
        protected TransformOptimizationComponent() => OptimizationSettings.AddOptimizationComponent(this);
        
        #endregion

        internal abstract void Initialize(InitializationEvents? @event);

        private void Awake() => Initialize(InitializationEvents.Awake);
        private void OnEnable() => Initialize(InitializationEvents.OnEnable);
        private void Start() => Initialize(InitializationEvents.Start);
    }
}