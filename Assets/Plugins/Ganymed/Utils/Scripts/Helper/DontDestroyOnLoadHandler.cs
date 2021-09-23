using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ganymed.Utils.Helper
{
    /// <summary>
    /// Class handling DontDestroyOnLoad objects. 
    /// </summary>
    public static class DontDestroyOnLoadHandler
    {
        public static readonly List<GameObject> DontDestroyOnLoadObjects = new List<GameObject>();

        /// <summary>
        /// Set an object as DontDestroyOnLoad.
        /// </summary>
        /// <param name="gameObj"></param>
        public static void DontDestroyOnLoad(this GameObject gameObj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityEditor.SceneVisibilityManager.instance.Show(gameObj, false);
#endif
            Object.DontDestroyOnLoad(gameObj);
            DontDestroyOnLoadObjects.Add(gameObj);
        }

        /// <summary>
        /// Destroy every object set as DontDestroyOnLoad.
        /// </summary>
        public static void DestroyAll()
        {
            foreach (var go in DontDestroyOnLoadObjects.Where(go => go != null))
            {
                Object.Destroy(go);
            }

            DontDestroyOnLoadObjects.Clear();
        }
    }
}