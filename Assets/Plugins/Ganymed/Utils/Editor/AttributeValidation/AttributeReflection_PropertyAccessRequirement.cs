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
        /// <summary>
        /// Method checks integrity of 'PropertyAccessRequirementAttribute' instances present within the passes propertyInfos.
        /// </summary>
        /// <param name="propertyInfos"></param>
        private static void ReflectPropertyAccessRequirement(IEnumerable<PropertyInfo> propertyInfos)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                foreach (var attribute in propertyInfo.GetCustomAttributes(true))
                {
                    var attributes = attribute.GetType().GetCustomAttributes(true).OfType<PropertyAccessRequirementAttribute>();
                    
                    foreach (var propertyRestrictionAttribute in attributes)
                    {
                        var canRead = propertyInfo.CanRead;
                        var canWrite = propertyInfo.CanWrite;
                        
                        // --- if a getter && setter are required
                        if (propertyRestrictionAttribute.RequiresReadAndWrite && !canRead && !canWrite)
                        {
                            var message =
                                $"Warning: the {RichText.Violet}Property{RichText.ClearColor} " +
                                $"'{RichText.Orange}{propertyInfo.Name}{RichText.ClearColor}' in " +
                                $"'{RichText.LightGray}{propertyInfo.DeclaringType?.Namespace}" +
                                $"{(propertyInfo.DeclaringType?.Namespace.IsNullOrWhiteSpace() ?? false? "" : ".")}" +
                                $"{RichText.ClearColor}{RichText.LightGray}{propertyInfo.DeclaringType?.Name}." +
                                $"{RichText.Blue}{propertyInfo.Name}'{RichText.ClearColor}{RichText.ClearColor} " +
                                $"requires {RichText.Red}read and write accessibility. {RichText.ClearColor}(Getter/Setter)\n";
                            Debug.LogWarning(message);    
                        }
                        // --- if a getter is required
                        if (propertyRestrictionAttribute.RequiresWrite && !canWrite)
                        {
                            var message =
                                $"Warning: the {RichText.Violet}Property{RichText.ClearColor} " +
                                $"'{RichText.Orange}{propertyInfo.Name}{RichText.ClearColor}' in " +
                                $"'{RichText.LightGray}{propertyInfo.DeclaringType?.Namespace}" +
                                $"{(propertyInfo.DeclaringType?.Namespace.IsNullOrWhiteSpace() ?? false? "" : ".")}" +
                                $"{RichText.ClearColor}{RichText.LightGray}{propertyInfo.DeclaringType?.Name}." +
                                $"{RichText.Blue}{propertyInfo.Name}'{RichText.ClearColor}{RichText.ClearColor} " +
                                $"requires {RichText.Red}write accessibility. {RichText.ClearColor}(Setter)\n";
                            Debug.LogWarning(message);    
                        }
                        // --- if a setter is required
                        if (propertyRestrictionAttribute.RequiresRead && !canRead)
                        {
                            var message =
                                $"Warning: the {RichText.Violet}Property{RichText.ClearColor} " +
                                $"'{RichText.Orange}{propertyInfo.Name}{RichText.ClearColor}' in " +
                                $"'{RichText.LightGray}{propertyInfo.DeclaringType?.Namespace}" +
                                $"{(propertyInfo.DeclaringType?.Namespace.IsNullOrWhiteSpace() ?? false? "" : ".")}" +
                                $"{RichText.ClearColor}{RichText.LightGray}{propertyInfo.DeclaringType?.Name}." +
                                $"{RichText.Blue}{propertyInfo.Name}'{RichText.ClearColor}{RichText.ClearColor} " +
                                $"requires {RichText.Red}read accessibility. {RichText.ClearColor}(Getter)\n";
                            Debug.LogWarning(message);    
                        }
                    }
                }
            }
        }
    }
}