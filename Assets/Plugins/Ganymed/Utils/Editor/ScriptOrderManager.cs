using System;
using Ganymed.Utils.Attributes;
using UnityEditor;
using UnityEngine;

namespace Ganymed.Utils.Editor
{
    [InitializeOnLoad]
    public static class ScriptOrderManager
    {
        static ScriptOrderManager()
        {
            foreach (var monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.GetClass() == null) continue;
                foreach (var attribute in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrder)))
                {
                    var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    var newOrder = ((ScriptOrder) attribute).order;
                    
                    if (currentOrder == newOrder) continue;
                    
                    Debug.Log($"Setting [ExecutionOrder] of {monoScript.name} from {currentOrder} to {newOrder}");
                    MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
    }
}