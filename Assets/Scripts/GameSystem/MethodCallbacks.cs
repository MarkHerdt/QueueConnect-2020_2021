using System;
using System.Collections.Generic;
using System.Linq;
using QueueConnect.Development;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace QueueConnect.GameSystem
{
    public enum Context
    {
        Awake,
        OnEnable
    }
    
    [HideMonoScript]
    [CreateAssetMenu(menuName = "MethodCallbacks", fileName = "MethodCallbacks_SOB")]  
    public class MethodCallbacks : ScriptableObject
    {
        [Serializable]
        private  class MethodsCallback : SerializableDictionaryBase<Context, List<Callback>> { } 
        
        #region Inspector Fields 
            [Tooltip("Every Method that is currently subscribed")]
            [ShowInInspector] public Dictionary<Context, List<Callback>> methods = new Dictionary<Context, List<Callback>>();
        #endregion

        #region Privates
            public delegate void Callback();     
        #endregion

        // #region Properties
        //     /// <summary>
        //     /// Every Method that is currently subscribed
        //     /// </summary>
        //     private static MethodsCallback Methods { get; } = new MethodsCallback();
        // #endregion
        
        // static MethodCallbacks()
        // {
        //     foreach (var _enum in Enum.GetValues(typeof(Context)).Cast<Context>())
        //     {
        //         if (!Methods.ContainsKey(_enum))
        //         {
        //             Methods.Add(_enum, new List<Callback>());
        //         }
        //     }
        // }
        
        // /// <summary>
        // /// Subscribes a Method to the specified Callback
        // /// </summary>
        // /// <param name="_Context">When the subscribed Method should be Invoked</param>
        // /// <param name="_Callback">Method to subscribe</param>
        // public static void Subscribe(Context _Context, Callback _Callback)
        // {
        //     if (_Callback.Target.GetType().IsSubclassOf(typeof(MonoBehaviour)))
        //     {
        //         // Checks if the GameObject in in the Scene
        //         if (((MonoBehaviour)_Callback.Target).gameObject.scene.IsValid() && !Methods[_Context].Contains(_Callback))
        //         {
        //             Methods[_Context].Add(_Callback);
        //         }   
        //     }
        //     else if (_Callback.Target.GetType().IsSubclassOf(typeof(ScriptableObject)))
        //     {
        //         if (!Methods[_Context].Contains(_Callback))
        //         {
        //             Methods[_Context].Add(_Callback);
        //         }   
        //     }
        // }
            
        // /// <summary>
        // /// Unsubscribes a Method from the specified Callback
        // /// </summary>
        // /// <param name="_Context">What callback is the Method currently subscribed to</param>
        // /// <param name="_Callback">Method to unsubscribe</param>
        // public static void UnSubscribe(Context _Context, Callback _Callback) 
        // {
        //     if (Methods[_Context].Contains(_Callback)) 
        //     {
        //         Methods[_Context].Remove(_Callback);   
        //     }
        // }

        private void OnValidate()
        {
            //DebugLog.White($"{methods.Values.Count}");  
            
            // DebugLog.White($"{Methods.Values.Count}"); 
            
            // foreach (var _enum in Enum.GetValues(typeof(Context)).Cast<Context>())
            // {
            //     if (!methods.ContainsKey(_enum))
            //     {
            //         methods.Add(_enum, new List<Callback>()); 
            //     }
            // }
            // foreach (var _callback in Methods)
            // {
            //     foreach (var _method in _callback.Value)
            //     {
            //         if (!methods[_callback.Key].Contains(_method))
            //         {
            //             DebugLog.Red("!methods");
            //             methods[_callback.Key].Add(_method);
            //         }
            //     }
            // }
        }
    }   
}