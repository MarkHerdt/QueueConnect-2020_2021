using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ganymed.Utils.Editor
{
    public static class SelectionHelper
    {
        private static readonly List<GameObject> selectionCache = new List<GameObject>();
    
        public static void SelectComponents<T>(bool validateHideFlags = true, bool logSelection = true) where T : Component
        {
            selectionCache.Clear();
            if (validateHideFlags)
            {
                foreach (var component in FindObjectsOfTypeAll<T>(true))
                {
                    component.hideFlags = HideFlags.None;
                    selectionCache.Add(component.gameObject);
                }    
            }
            else
            {
                foreach (var component in FindObjectsOfTypeAll<T>(true))
                {
                    selectionCache.Add(component.gameObject);
                }  
            }
        
            
            if(logSelection)
                Debug.Log($"Selected {selectionCache.Count} Objects of type {typeof(T).Name}");
            
            Selection.objects = selectionCache.ToArray();
            selectionCache.Clear();
        }
        
        /// <summary>
        /// Use this method to get all loaded objects of some type, including inactive objects. 
        /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindObjectsOfTypeAll<T>(bool includeInactive)
        {
            var results = new List<T>();
            for(var i = 0; i< SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    foreach (var go in allGameObjects)
                    {
                        results.AddRange(go.GetComponentsInChildren<T>(includeInactive));
                    }
                }
            }
            return results;
        }
    }
}
