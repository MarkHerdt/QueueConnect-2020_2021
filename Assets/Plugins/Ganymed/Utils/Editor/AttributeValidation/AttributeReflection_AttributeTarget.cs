using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ganymed.Utils.Attributes;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Helper;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEngine;

namespace Ganymed.Utils.Editor.AttributeValidation
{
    internal static partial class AttributeReflection
    {
        private static void ReflectAttributeTarget(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                foreach (var inspected in type.GetNullableUnderlying().GetCustomAttributes())
                {
                    foreach (var attribute in inspected.GetType().GetNullableUnderlying().GetCustomAttributes())
                    {
                        if (!(attribute is AttributeTargetAttribute targetAttribute)) continue;

                        // Continue if we have an attribute of type AttributeTargetAttribute
                        
                        foreach (var validTypes in targetAttribute.PermittedTypes)
                        {
                            if(type == validTypes) continue;
                            if (targetAttribute.Inherited && (type.IsAssignableFrom(validTypes) || type.IsSubclassOf(validTypes)))
                                continue;

                            var viableType =
                                targetAttribute.PermittedTypes.Aggregate(string.Empty, (current, viable)
                                    => current + $"{viable} |").RemoveFormEnd(2);

                            var message = $"Warning: the {RichText.Violet}" +
                                          $"{nameof(Attribute)}" +
                                          $"{RichText.ClearColor} " +
                                          $"{RichText.Orange}" +
                                          $"{inspected.GetType().Name}" +
                                          $"{RichText.ClearColor} in " +
                                          $"{RichText.LightGray}{type.Namespace}{RichText.ClearColor}" +
                                          $"{(type.Namespace.IsNullOrWhiteSpace() ? "" : ".")}" +
                                          $"{RichText.Blue}{type.Name}{RichText.ClearColor} " +
                                          $"is only a valid attribute for the following types" +
                                          $"{(targetAttribute.Inherited? " or subclasses of" : "")}" +
                                          $": " +
                                          $"{RichText.Orange}'{viableType}'{RichText.ClearColor}\n";
                        
                            Debug.LogWarning(message);    
                        }
                    }
                }
            }
        }
    }
}