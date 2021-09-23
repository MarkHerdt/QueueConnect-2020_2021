using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ganymed.Utils.Attributes;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Helper;
using UnityEngine;

// ReSharper disable PossibleMultipleEnumeration

namespace Ganymed.Utils.Editor.AttributeValidation
{
    internal static partial class AttributeReflection
    {
        /// <summary>
        /// Method checks integrity of 'RequiresAdditionalAttributesAttribute' instances present within the passes member/types.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="fieldInfos"></param>
        /// <param name="propertyInfos"></param>
        /// <param name="methodInfos"></param>
        /// <param name="constructorInfos"></param>
        /// <param name="eventInfos"></param>
        private static void ReflectRequiredAttributes(IEnumerable<Type> types,
            IEnumerable<FieldInfo> fieldInfos,
            IEnumerable<PropertyInfo> propertyInfos,
            IEnumerable<MethodInfo> methodInfos,
            IEnumerable<ConstructorInfo> constructorInfos,
            IEnumerable<EventInfo> eventInfos)
        {
            #region --- [TYPE: CLASS | INTERFACE | DELEGATE | STRUCT | GENERIC] ---

            foreach (var type in types)
            {
                // --- > Classes | Interface | Enum | Delegate | Struct
                
                // Get every attribute of the type.
                foreach (var inspectedAttribute in type.GetCustomAttributes())
                {
                    foreach (var potentialMatch in inspectedAttribute.GetType().GetCustomAttributes())
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, type.GetCustomAttributes());
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(type.GetTarget(), type.DeclaringType, type);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }
                    }
                }
                
                #region --- [GENERIC] ---

                //--- > GenericParameter (Class)
                foreach (var genericParameterConstraint in type.GetGenericArguments())
                {
                    // Get every attribute of the 'GenericParameter'. 
                    var attributes = genericParameterConstraint.GetCustomAttributes();
                    foreach (var inspectedAttribute in attributes)
                    {
                        // Get every attribute of every attribute the genericParameter.
                        var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                        foreach (var potentialMatch in potentialMatches) 
                        {
                            if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                            {
                                var missing = GetMissingAttributeTypes(required, attributes);
                                
                                if (IsInspectedValid(missing, required)) continue;
                                
                                var ctx = new Context(AttributeTargets.GenericParameter, type);
                                LogWarningMessage(required, missing, inspectedAttribute, ctx);
                            }   
                        }
                    }
                }

                #endregion
            }

            #endregion
            
            #region --- [FIELDS] ---

            //--- > Fields
            foreach (var fieldInfo in fieldInfos)
            {
                // Get every attribute of the 'Field'. 
                var attributes = fieldInfo.GetCustomAttributes();
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the field.
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches) 
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(AttributeTargets.Field, fieldInfo.DeclaringType, fieldInfo);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }   
                    }
                }
            }

            #endregion
            
            #region --- [PROPERTIES] ---

            //--- > Properties
            foreach (var propertyInfo in propertyInfos)
            {
                // Get every attribute of the 'Property'. 
                var attributes = propertyInfo.GetBaseDefinition().GetCustomAttributes();
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the property. 
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches) 
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(AttributeTargets.Property, propertyInfo.DeclaringType, propertyInfo);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }   
                    }
                }
            }
            
            #endregion
            
            #region --- [METHODS] ---

            //--- > Methods
            foreach (var methodInfo in methodInfos)
            {
                // Get every attribute of the 'Methods'. 
                    
                var attributes = methodInfo.GetBaseDefinition().GetCustomAttributes();
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the method.
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches) 
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);
                            
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(AttributeTargets.Method, methodInfo.DeclaringType, methodInfo);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }   
                    }
                }
            }

            #endregion
            
            #region --- [CONSTRUCTOR] ---

            //--- > Constructor
            foreach (var constructorInfo in constructorInfos)
            {
                // Get every attribute of the 'Constructor'. 
                var attributes = constructorInfo.GetCustomAttributes();
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the constructor.
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches) 
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(AttributeTargets.Constructor, constructorInfo.DeclaringType, constructorInfo);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }   
                    }
                }
            }

            #endregion
            
            #region --- [EVENTS] ---

            //--- > Event
            foreach (var eventInfo in eventInfos)
            {
                // Get every attribute of the 'Event'. 
                var attributes = eventInfo.GetCustomAttributes();
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the event.
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches) 
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);
                                
                            if (IsInspectedValid(missing, required)) continue;
                                
                            var ctx = new Context(AttributeTargets.Event, eventInfo.DeclaringType, eventInfo);
                            LogWarningMessage(required, missing, inspectedAttribute,ctx);
                        }   
                    }
                }
            }

            #endregion
            
            #region --- [RETURNVALUES] ---

            //--- > ReturnValue
            foreach (var methodInfo in methodInfos)
            {
                // Get every attribute of the 'ReturnValue' of the method. 
                var attributes = (methodInfo.GetBaseDefinition().ReturnParameter)?.GetCustomAttributes();
                if (attributes == null) continue;
                foreach (var inspectedAttribute in attributes)
                {
                    // Get every attribute of every attribute the returnValue.
                    var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                    foreach (var potentialMatch in potentialMatches)
                    {
                        if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                        {
                            var missing = GetMissingAttributeTypes(required, attributes);

                            if (IsInspectedValid(missing, required)) continue;

                            var ctx = new Context(AttributeTargets.ReturnValue, methodInfo.DeclaringType, methodInfo);
                            LogWarningMessage(required, missing, inspectedAttribute, ctx);
                        }
                    }
                }
            }

            #endregion

            #region --- [PARAMETER] ---

            //--- > Parameter (Methods)
            foreach (var methodInfo in methodInfos)
            {
                // Get every Parameter of the of the Method. 
                var parameters = methodInfo.GetBaseDefinition().GetParameters();
                foreach (var parameterInfo in parameters)
                {
                    // Get every attribute of the 'Parameter' of the method. 
                    var attributes = parameterInfo.GetCustomAttributes();
                    foreach (var inspectedAttribute in attributes)
                    {
                        // Get every attribute of every attribute the method.
                        var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                        foreach (var potentialMatch in potentialMatches)
                        {
                            if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                            {
                                var missing = GetMissingAttributeTypes(required, attributes);

                                if (IsInspectedValid(missing, required)) continue;

                                var ctx = new Context(AttributeTargets.Parameter, methodInfo.DeclaringType, methodInfo,
                                    parameterInfo);
                                LogWarningMessage(required, missing, inspectedAttribute, ctx);
                            }
                        }
                    }
                }
            }

            //--- > Parameter (Constructor)
            foreach (var constructorInfo in constructorInfos)
            {
                // Get every Parameter of the of the Constructor. 
                var parameters = constructorInfo.GetParameters();
                foreach (var parameterInfo in parameters)
                {
                    // Get every attribute of the 'Parameter' of the constructor. 
                    var attributes = parameterInfo.GetCustomAttributes();
                    foreach (var inspectedAttribute in attributes)
                    {
                        // Get every attribute of every attribute the constructor.
                        var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                        foreach (var potentialMatch in potentialMatches)
                        {
                            if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                            {
                                var missing = GetMissingAttributeTypes(required, attributes);

                                if (IsInspectedValid(missing, required)) continue;

                                var ctx = new Context(AttributeTargets.Parameter, constructorInfo.DeclaringType,
                                    constructorInfo, parameterInfo);
                                LogWarningMessage(required, missing, inspectedAttribute, ctx);
                            }
                        }
                    }
                }
            }

            #endregion

            #region --- [GENERIC PARAMETER] ---

            //--- > GenericParameter (Methods)
            foreach (var methodInfo in methodInfos)
            {
                // Check if the method is generic
                if (!methodInfo.GetBaseDefinition().IsGenericMethod) continue;

                // Get every 'GenericParameter' of the of the method. 
                var genericArguments = methodInfo.GetBaseDefinition().GetGenericArguments();

                foreach (var method in genericArguments)
                {
                    // Get every attribute of the 'GenericParameter' of the method. 
                    var attributes = method.GetCustomAttributes();

                    foreach (var inspectedAttribute in attributes)
                    {
                        // Get every attribute of every attribute the method.
                        var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                        foreach (var potentialMatch in potentialMatches)
                        {
                            if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                            {
                                var missing = GetMissingAttributeTypes(required, attributes);

                                if (IsInspectedValid(missing, required)) continue;

                                var ctx = new Context(AttributeTargets.GenericParameter, methodInfo.DeclaringType,
                                    methodInfo);
                                LogWarningMessage(required, missing, inspectedAttribute, ctx);
                            }
                        }
                    }
                }
            }

            //--- > GenericParameter (Constructor)
            foreach (var constructorInfo in constructorInfos)
            {
                // Check if the method is generic
                if (!constructorInfo.IsGenericMethod) continue;

                // Get every 'GenericParameter' of the of the constructor. 
                var genericArguments = constructorInfo.GetGenericArguments();

                foreach (var method in genericArguments)
                {
                    // Get every attribute of the 'GenericParameter' of the constructor. 
                    var attributes = method.GetCustomAttributes();

                    foreach (var inspectedAttribute in attributes)
                    {
                        // Get every attribute of every attribute the constructor.
                        var potentialMatches = inspectedAttribute.GetType().GetCustomAttributes();
                        foreach (var potentialMatch in potentialMatches)
                        {
                            if (potentialMatch is RequiresAdditionalAttributesAttribute required)
                            {
                                var missing = GetMissingAttributeTypes(required, attributes);

                                if (IsInspectedValid(missing, required)) continue;

                                var ctx = new Context(AttributeTargets.GenericParameter, constructorInfo.DeclaringType,
                                    constructorInfo);
                                LogWarningMessage(required, missing, inspectedAttribute, ctx);
                            }
                        }
                    }
                }
            }

            #endregion
        }

        #region --- [HELPER] ---
        
        private static Type[] GetMissingAttributeTypes(RequiresAdditionalAttributesAttribute requiresAdditionalAttributes, IEnumerable<Attribute> attributes)
        {
            var enumerable = attributes as Attribute[] ?? attributes.ToArray();
            
            if(enumerable.Count() == 1 && enumerable[0].GetType() == typeof(RequiresAdditionalAttributesAttribute))
                return requiresAdditionalAttributes.RequiredAttributes;
            
            var missingTypes = new List<Type>();

            foreach (var type in requiresAdditionalAttributes.RequiredAttributes)
            {
                var typeAvailable = false;
                
                foreach (var attribute in enumerable)
                {
                    var aType = attribute.GetType();
                    if (requiresAdditionalAttributes.Inherited)
                    {
                        if (aType.IsSubclassOf(type) ||aType.IsAssignableFrom(type) || aType == type)
                        {
                            typeAvailable = true;
                            break;
                        }
                    }
                    else
                    {
                        if (aType == type)
                        {
                            typeAvailable = true;
                            break;    
                        }
                    }
                }
                
                if(typeAvailable) continue;
                missingTypes.Add(type);
            }

            return missingTypes.ToArray();
        }

        private static bool IsInspectedValid(Type[] missing, RequiresAdditionalAttributesAttribute requiresAdditionalAttributes)
        {
            if(missing.Length < requiresAdditionalAttributes.RequiredAttributes.Length && requiresAdditionalAttributes.RequiresAny) return true;
            return missing.Length <= 0;
        }
        
        private static void LogWarningMessage(RequiresAdditionalAttributesAttribute requiresAdditionalAttributes, Type[] missingTypes, Attribute inspected, Context ctx)
        {
            var requiredTypes = requiresAdditionalAttributes.RequiredAttributes;

            var any = requiresAdditionalAttributes.RequiresAny;
            var one = requiredTypes.Length == 1;
            
            var message =
                $"Warning: the {RichText.Violet}{ctx.Target}{RichText.ClearColor} attribute " +
                $"'{RichText.Orange}{inspected.GetType().Name.Replace("Attribute", "")}{RichText.ClearColor}' in " +
                $"'{RichText.LightGray}{ctx.Declaring.Namespace?.Replace("+", ".")}{ctx.Declaring.Name}" +
                $"{(ctx.MemberInfo != null ? $".{RichText.Blue}{ctx.MemberInfo.Name}{RichText.ClearColor}" : "")}" +
                $"{(ctx.ParameterInfo != null ? $"| parameter: {RichText.Blue}{ctx.ParameterInfo.Name} | type: [{ctx.ParameterInfo.ParameterType.Name}]{RichText.ClearColor}" : "")}" +
                $"{RichText.ClearColor}' requires " +
                $"{(one? "the" : any? "any of the listed" : "all of the listed")} attribute{(one? "" : "s")}: ";

            foreach (var type in requiredTypes)
            {
                message += $"{(missingTypes.Contains(type) ? RichText.Red : RichText.Green)}" +
                                  $"{type.Name}{RichText.ClearColor} | ";
            }

            message = $"{message.Remove(message.Length - 3, 3)}\n";
            
            Debug.LogWarning(message);    
        }
        #endregion
    }
}