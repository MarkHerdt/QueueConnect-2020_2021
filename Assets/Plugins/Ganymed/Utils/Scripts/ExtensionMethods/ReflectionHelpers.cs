using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ganymed.Utils.ExtensionMethods
{
    /// <summary>
    /// Helper class containing Extension Methods for Reflection
    /// </summary>
    public static class ReflectionHelpers
    {
        #region --- [FIELDS] ---

        private const BindingFlags MemberFlags 
            = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        #endregion
        
        #region --- [TYPE EXTENSIONS] ---

        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            var result = new List<Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
            }

            return result.ToArray();
        }

        public static Type[] GetAllDerivedTypes<T>(this AppDomain aAppDomain)
        {
            return GetAllDerivedTypes(aAppDomain, typeof(T));
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [INTERFACE TYPE EXTENSIONS] ---

        public static System.Type[] GetTypesWithInterface(this System.AppDomain aAppDomain, System.Type aInterfaceType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (aInterfaceType.IsAssignableFrom(type))
                        result.Add(type);
            }

            return result.ToArray();
        }

        public static System.Type[] GetTypesWithInterface<T>(this System.AppDomain aAppDomain)
        {
            return GetTypesWithInterface(aAppDomain, typeof(T));
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [PARAMETER INFO] ---

        public static bool isParams(this ParameterInfo param)
        {
            return param.GetCustomAttributes(typeof (ParamArrayAttribute), false).Length > 0;
        }
        
        public static bool TryGetAttribute<T>(this ParameterInfo info, out T attribute) where T : Attribute
        {
            attribute = null;
            
            foreach (var viewed in info.GetCustomAttributes())
            {
                if (viewed.GetType() != typeof(T)) continue;
                attribute = (T)viewed;
                return true;
            }

            return false;
        }
        

        #endregion

        #region --- [PROPERTYINFO] ---

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="propertyInfo"/> object for the 
        /// method on the direct or indirect base class in which the property represented 
        /// by this instance was first declared. 
        /// </summary>
        /// <returns>A <see cref="propertyInfo"/> object for the first implementation of this property.</returns>
        public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            var method = propertyInfo.GetAccessors(true)[0];
            if (method == null)
                return null;
 
            var baseMethod = method.GetBaseDefinition();
 
            if (baseMethod == method)
                return propertyInfo;
 
            const BindingFlags allProperties = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
 
            var arguments = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
 
            return baseMethod.DeclaringType?.GetProperty(propertyInfo.Name, allProperties, 
                null, propertyInfo.PropertyType, arguments, null);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [TYPES FROM ASSEMBLIES] ---

        /// <summary>
        /// out params of all Assembles.
        /// </summary>
        /// <param name="aAppDomain"></param>
        /// <param name="types"></param>
        /// <param name="fieldInfos"></param>
        /// <param name="propertyInfos"></param>
        /// <param name="methodInfos"></param>
        /// <param name="constructorInfos"></param>
        /// <param name="eventInfos"></param>
        /// <param name="memberInfos"></param>
        public static void GetAll(this AppDomain aAppDomain, 
            out List<Type> types,
            out List<FieldInfo> fieldInfos,
            out List<PropertyInfo> propertyInfos,
            out List<MethodInfo> methodInfos,
            out List<ConstructorInfo> constructorInfos,
            out List<EventInfo> eventInfos,
            out List<MemberInfo> memberInfos)
        {
            types = new List<Type>();
            memberInfos = new List<MemberInfo>();
            fieldInfos = new List<FieldInfo>();
            propertyInfos= new List<PropertyInfo>();
            methodInfos= new List<MethodInfo>();
            constructorInfos= new List<ConstructorInfo>();
            eventInfos= new List<EventInfo>();

            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    types.Add(type);
                    memberInfos.AddRange(type.GetMembers(MemberFlags));
                    fieldInfos.AddRange(type.GetFields(MemberFlags));
                    propertyInfos.AddRange(type.GetProperties(MemberFlags));
                    methodInfos.AddRange(type.GetMethods(MemberFlags));
                    constructorInfos.AddRange(type.GetConstructors(MemberFlags));
                    eventInfos.AddRange(type.GetEvents(MemberFlags));
                }
            }
        }
        
#if UNITY_EDITOR
        
        /// <summary>
        /// Out params of unity assemblies. 
        /// </summary>
        /// <param name="aAppDomain"></param>
        /// <param name="unityAssemblyDefinitions"></param>
        /// <param name="types"></param>
        /// <param name="fieldInfos"></param>
        /// <param name="propertyInfos"></param>
        /// <param name="methodInfos"></param>
        /// <param name="constructorInfos"></param>
        /// <param name="eventInfos"></param>
        /// <param name="memberInfos"></param>
        public static void GetTypesFromUnityAssemblies(this AppDomain aAppDomain, 
            IEnumerable<UnityEditor.Compilation.Assembly> unityAssemblyDefinitions,
            out List<Type> types,
            out List<FieldInfo> fieldInfos,
            out List<PropertyInfo> propertyInfos,
            out List<MethodInfo> methodInfos,
            out List<ConstructorInfo> constructorInfos,
            out List<EventInfo> eventInfos,
            out List<MemberInfo> memberInfos)
        {
            types = new List<Type>();
            memberInfos = new List<MemberInfo>();
            fieldInfos = new List<FieldInfo>();
            propertyInfos= new List<PropertyInfo>();
            methodInfos= new List<MethodInfo>();
            constructorInfos= new List<ConstructorInfo>();
            eventInfos= new List<EventInfo>();
            
            var assemblies = unityAssemblyDefinitions.Select(assembly
                => aAppDomain.GetAssemblies().SingleOrDefault(a
                    => a.GetName().Name == assembly.name)).ToList();

            foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypes()))
            {
                types.Add(type);
                memberInfos.AddRange(type.GetMembers(MemberFlags));
                fieldInfos.AddRange(type.GetFields(MemberFlags));
                propertyInfos.AddRange(type.GetProperties(MemberFlags));
                methodInfos.AddRange(type.GetMethods(MemberFlags));
                constructorInfos.AddRange(type.GetConstructors(MemberFlags));
                eventInfos.AddRange(type.GetEvents(MemberFlags));
            }
        }
        
#endif
        
        public static AttributeTargets GetTarget(this Type type)
        {
            return
                type.IsEnum ? AttributeTargets.Enum :
                type.IsDelegate() ? AttributeTargets.Delegate :
                type.IsInterface ? AttributeTargets.Interface :
                type.IsStruct() ? AttributeTargets.Struct : 
                AttributeTargets.Class;
        }
        
        
        public static IEnumerable<Type> GetAllTypes(this AppDomain aAppDomain)
        {
            var result = new List<Type>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                result.AddRange(assembly.GetTypes());
            }

            return result;
        }
        
        public static IEnumerable<FieldInfo> GetAllFields(this AppDomain aAppDomain)
        {
            var result = new List<FieldInfo>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    result.AddRange(type.GetFields(MemberFlags));
                }
            }

            return result;
        }
        
        public static IEnumerable<PropertyInfo> GetAllProperties(this AppDomain aAppDomain)
        {
            var result = new List<PropertyInfo>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    result.AddRange(type.GetProperties(MemberFlags));
                }
            }

            return result;
        }
        
        
        public static IEnumerable<MethodInfo> GetAllMethods(this AppDomain aAppDomain)
        {
            var result = new List<MethodInfo>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    result.AddRange(type.GetMethods(MemberFlags));
                }
            }

            return result;
        }
        
        public static IEnumerable<ConstructorInfo> GetAllConstructor(this AppDomain aAppDomain)
        {
            var result = new List<ConstructorInfo>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    result.AddRange(type.GetConstructors(MemberFlags));
                }
            }

            return result;
        }
        
        public static IEnumerable<EventInfo> GetAllEvents(this AppDomain aAppDomain)
        {
            var result = new List<EventInfo>();
            
            foreach (var assembly in aAppDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    result.AddRange(type.GetEvents(MemberFlags));
                }
            }

            return result;
        }
        #endregion
    }
}