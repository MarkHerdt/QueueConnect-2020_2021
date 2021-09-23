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
        private static void ReflectTargetTypeRestriction(IEnumerable<PropertyInfo> propertyInfos, IEnumerable<FieldInfo> fieldInfos)
        {
            #region --- [PROPERTIES] ---

            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType.GetNullableUnderlying();

                if (propertyInfo.GetCustomAttributes(true).OfType<AllowUnsafeAttribute>().Any()) continue;

                foreach (var attribute in propertyInfo.GetCustomAttributes(true))
                {
                    var propertyAttributes = attribute.GetType().GetCustomAttributes(true).OfType<TargetTypeRestrictionAttribute>().ToArray();

                    if (propertyAttributes.Length > 0)
                    {
                        var allowedAffiliations = propertyAttributes.Aggregate(TypeAffiliations.None,
                            (current, propertyAttribute) => current | propertyAttribute.ValidTypeAffiliations);
                        
                        if (allowedAffiliations.HasFlag(propertyType.GetTypeAffiliation())) continue;
                        
                        var allowedTypes = new List<Type>();
                        var allowedInheritingTypes = new List<Type>();
                       
                        foreach (var propertyAttribute in propertyAttributes)
                        {
                            if (!propertyAttribute.Inherited)
                            {
                                foreach (var type in propertyAttribute.ValidTypes)
                                {
                                    if (!allowedTypes.Contains(type))
                                        allowedTypes.Add(type);
                                }
                            }
                            else
                            {
                                foreach (var type in propertyAttribute.ValidTypes)
                                {
                                    if (!allowedTypes.Contains(type))
                                        allowedTypes.Add(type);
                                    if (!allowedInheritingTypes.Contains(type))
                                        allowedInheritingTypes.Add(type);
                                }
                            }
                        }


                        if (allowedTypes.Contains(propertyType)) continue;
                        if (allowedInheritingTypes.Any(type =>
                            propertyType.IsSubclassOf(type) || propertyType.IsAssignableFrom(type))) continue;
                        
                        LogWarningMessage(
                            allowedAffiliations, 
                            allowedInheritingTypes, 
                            allowedTypes, 
                            propertyInfo, 
                            propertyInfo.PropertyType);
                    }
                }
            }

            #endregion

            #region --- [FIELDS] ---

            foreach (var fieldInfo in fieldInfos)
            {
                var fieldType = fieldInfo.FieldType.GetNullableUnderlying();

                if (fieldInfo.GetCustomAttributes(true).OfType<AllowUnsafeAttribute>().Any()) continue;

                foreach (var attribute in fieldInfo.GetCustomAttributes(true))
                {
                    var fieldAttributes = attribute.GetType().GetCustomAttributes(true).OfType<TargetTypeRestrictionAttribute>().ToArray();

                    if (fieldAttributes.Length > 0)
                    {
                        var allowedAffiliations = fieldAttributes.Aggregate(TypeAffiliations.None,
                            (current, propertyAttribute) => current | propertyAttribute.ValidTypeAffiliations);
                        
                        if (allowedAffiliations.HasFlag(fieldType.GetTypeAffiliation())) continue;
                        
                        var allowedTypes = new List<Type>();
                        var allowedInheritingTypes = new List<Type>();
                       
                        foreach (var propertyAttribute in fieldAttributes)
                        {
                            if (!propertyAttribute.Inherited)
                            {
                                foreach (var type in propertyAttribute.ValidTypes)
                                {
                                    if (!allowedTypes.Contains(type))
                                        allowedTypes.Add(type);
                                }
                            }
                            else
                            {
                                foreach (var type in propertyAttribute.ValidTypes)
                                {
                                    if (!allowedTypes.Contains(type))
                                        allowedTypes.Add(type);
                                    if (!allowedInheritingTypes.Contains(type))
                                        allowedInheritingTypes.Add(type);
                                }
                            }
                        }

                        if (allowedTypes.Contains(fieldType)) continue;
                        if (allowedInheritingTypes.Any(type =>
                            fieldType.IsSubclassOf(type) || fieldType.IsAssignableFrom(type))) continue;

                        LogWarningMessage(allowedAffiliations, allowedInheritingTypes, allowedTypes, fieldInfo, fieldInfo.FieldType);
                    }
                }
            }
            #endregion
        }

        private static void LogWarningMessage(
            TypeAffiliations allowedAffiliations,
            IReadOnlyCollection<Type> allowedInheritingTypes,
            IReadOnlyCollection<Type> allowedTypes,
            MemberInfo memberInfo,
            Type propertyFieldType)
        {
            var message =
                $"Warning: the {RichText.Violet}{memberInfo.MemberType}{RichText.ClearColor} " +
                $"'{RichText.Orange}{memberInfo.Name}{RichText.ClearColor}' in " +
                $"'{RichText.LightGray}{memberInfo.ReflectedType?.Namespace}.{memberInfo.ReflectedType?.Name}{RichText.ClearColor}" +
                $"{RichText.Blue}.{memberInfo.Name}{RichText.ClearColor}' " +
                $"has invalid type: {RichText.Red}'{propertyFieldType}'.{RichText.ClearColor}" +
                $"\nAllowed types for attribute are '{memberInfo.GetType().Name}': " +
                            
                $"{(allowedInheritingTypes.Count > 0? $"Inheriting types: [{RichText.Blue}" : "")}" +
                $"{string.Join(" ", allowedInheritingTypes.Select(x => $"{x.Name}, ")).RemoveFormEnd(2)}" +
                $"{(allowedInheritingTypes.Count > 0? $"{RichText.ClearColor}] " : "")}" +
                            
                $"{(allowedTypes.Count > 0? $"Types: [{RichText.Blue}" : "")}" +
                $"{string.Join(" ", allowedTypes.Select(x => $"{x.Name}, ")).RemoveFormEnd(2)}" +
                $"{(allowedTypes.Count > 0? $"{RichText.ClearColor}] " : "")}" +
                            
                $"{(allowedAffiliations != TypeAffiliations.None? $"Categories: [{RichText.Blue}" : "")}" +
                $"{allowedAffiliations}" +
                $"{(allowedAffiliations != TypeAffiliations.None? $"{RichText.ClearColor}] " : "")}" +
                            
                $"You should either validate the type of the property or use the " +
                $"{RichText.Orange}" +
                $"{nameof(AllowUnsafeAttribute).Replace(nameof(Attribute), "")}{RichText.ClearColor} " +
                $"attribute to suppress this Warning";

            Debug.LogWarning(message);    
        }
    }
}