using System;
using System.Collections.Generic;
using System.Reflection;
using Ganymed.Utils.Attributes;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Helper;
using JetBrains.Annotations;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using UnityEngine;

namespace Ganymed.Utils.Editor.AttributeValidation
{
    internal static partial class AttributeReflection
    {
        private static void ReflectValidBindingFlagsAttribute(IEnumerable<MemberInfo> memberInfos)
        {
            foreach (var memberInfo in memberInfos)

                switch (memberInfo)
                {
                    case MethodInfo methodInfo:
                        foreach (var inspected in methodInfo.GetBaseDefinition().GetCustomAttributes())
                        foreach (var attribute in inspected.GetType().GetCustomAttributes())
                            if (attribute is RequiredAccessAttribute validBindingFlagsAttribute)
                            {
                                string requiredPublic = null;
                                string requiredStatic = null;

                                if (validBindingFlagsAttribute.PublicRequiredOrNull != null)
                                {
                                    var _public = (bool) validBindingFlagsAttribute.PublicRequiredOrNull;
                                    if (_public != methodInfo.IsPublic)
                                        requiredPublic = $"{(_public ? "public" : "private")}";
                                }

                                if (validBindingFlagsAttribute.StaticRequiredOrNull != null)
                                {
                                    var _static = (bool) validBindingFlagsAttribute.StaticRequiredOrNull;
                                    if (_static != methodInfo.IsStatic)
                                        requiredStatic = $"{(_static ? "static" : "non static")}";
                                }

                                if (requiredPublic != null || requiredStatic != null)
                                    LogValidBindingFlagsWarning(
                                        AttributeTargets.Method,
                                        requiredPublic,
                                        requiredStatic,
                                        inspected,
                                        memberInfo);
                            }

                        break;


                    case FieldInfo fieldInfo:
                        foreach (var inspected in fieldInfo.GetCustomAttributes())
                        foreach (var attribute in inspected.GetType().GetCustomAttributes())
                            if (attribute is RequiredAccessAttribute validBindingFlagsAttribute)
                            {
                                string requiredPublic = null;
                                string requiredStatic = null;

                                if (validBindingFlagsAttribute.PublicRequiredOrNull != null)
                                {
                                    var _public = (bool) validBindingFlagsAttribute.PublicRequiredOrNull;
                                    if (_public != fieldInfo.IsPublic)
                                        requiredPublic = $"{(_public ? "public" : "private")}";
                                }

                                if (validBindingFlagsAttribute.StaticRequiredOrNull != null)
                                {
                                    var _static = (bool) validBindingFlagsAttribute.StaticRequiredOrNull;
                                    if (_static != fieldInfo.IsStatic)
                                        requiredStatic = $"{(_static ? "static" : "non static")}";
                                }

                                if (requiredPublic != null || requiredStatic != null)
                                    LogValidBindingFlagsWarning(
                                        AttributeTargets.Field,
                                        requiredPublic,
                                        requiredStatic,
                                        inspected,
                                        memberInfo);
                            }

                        break;


                    case PropertyInfo propertyInfo:
                        foreach (var inspected in propertyInfo.GetBaseDefinition().GetCustomAttributes())
                        foreach (var attribute in inspected.GetType().GetCustomAttributes())
                            if (attribute is RequiredAccessAttribute validBindingFlagsAttribute)
                            {
                                string requiredPublic = null;
                                string requiredStatic = null;

                                if (validBindingFlagsAttribute.PublicRequiredOrNull != null)
                                {
                                    var _public = (bool) validBindingFlagsAttribute.PublicRequiredOrNull;
                                    if (_public != propertyInfo.GetAccessors(true)[0].IsPublic)
                                        requiredPublic = $"{(_public ? "public" : "private")}";
                                }

                                if (validBindingFlagsAttribute.StaticRequiredOrNull != null)
                                {
                                    var _static = (bool) validBindingFlagsAttribute.StaticRequiredOrNull;
                                    if (_static != propertyInfo.GetAccessors(true)[0].IsStatic)
                                        requiredStatic = $"{(_static ? "static" : "non static")}";
                                }

                                if (requiredPublic != null || requiredStatic != null)
                                    LogValidBindingFlagsWarning(
                                        AttributeTargets.Property,
                                        requiredPublic,
                                        requiredStatic,
                                        inspected,
                                        memberInfo);
                            }

                        break;


                    case ConstructorInfo constructorInfo:
                        foreach (var inspected in constructorInfo.GetCustomAttributes())
                        foreach (var attribute in inspected.GetType().GetCustomAttributes())
                            if (attribute is RequiredAccessAttribute validBindingFlagsAttribute)
                            {
                                string requiredPublic = null;
                                string requiredStatic = null;

                                if (validBindingFlagsAttribute.PublicRequiredOrNull != null)
                                {
                                    var _public = (bool) validBindingFlagsAttribute.PublicRequiredOrNull;
                                    if (_public != constructorInfo.IsPublic)
                                        requiredPublic = $"{(_public ? "public" : "private")}";
                                }

                                if (validBindingFlagsAttribute.StaticRequiredOrNull != null)
                                {
                                    var _static = (bool) validBindingFlagsAttribute.StaticRequiredOrNull;
                                    if (_static != constructorInfo.IsStatic)
                                        requiredStatic = $"{(_static ? "static" : "non static")}";
                                }

                                if (requiredPublic != null || requiredStatic != null)
                                    LogValidBindingFlagsWarning(
                                        AttributeTargets.Constructor,
                                        requiredPublic,
                                        requiredStatic,
                                        inspected,
                                        memberInfo);
                            }

                        break;
                }
        }

        private static void LogValidBindingFlagsWarning(
            AttributeTargets target,
            [CanBeNull] string requiredPublic,
            [CanBeNull] string requiredStatic,
            Attribute origin,
            MemberInfo memberInfo)
        {
            var message =
                $"Warning: the {RichText.Violet}{target}{RichText.ClearColor} " +
                $"'{RichText.Orange}{memberInfo.Name}{RichText.ClearColor}' in: " +
                $"{RichText.LightGray}'{memberInfo.DeclaringType?.FullName}.{RichText.ClearColor}" +
                $"{RichText.Blue}{memberInfo.Name}'{RichText.ClearColor} requires: " +
                $"{(requiredPublic != null ? $"{RichText.Red}{requiredPublic}{RichText.ClearColor}" : "")}" +
                $"{(requiredPublic != null && requiredStatic != null ? " and " : "")}" +
                $"{(requiredStatic != null ? $"{RichText.Red}{requiredStatic}{RichText.ClearColor}" : "")} access " +
                $"because of the members '{RichText.Orange}{origin.GetType().Name.Delete("Attribute")}" +
                $"{RichText.ClearColor}' Attribute.\n";

            Debug.LogWarning(message);    
        }
    }
}