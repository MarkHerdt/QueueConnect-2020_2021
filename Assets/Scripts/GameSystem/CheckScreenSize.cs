using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Checks if the Screen size has changed
    /// </summary>
    [ExecuteInEditMode][HideMonoScript]
    public class CheckScreenSize :  UIBehaviour
    {
        /// <summary>
        /// Is fired when the Screen size changes
        /// </summary>
        public static event Action OnScreenSizeChanged;
        
        protected override void OnRectTransformDimensionsChange ()
        {
            OnScreenSizeChanged?.Invoke();
        }
    }   
}