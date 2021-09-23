using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Ganymed.Utils.Editor.AttributeValidation
{
    /// <summary>
    /// Helper struct for attribute reflection
    /// </summary>
    internal readonly struct Context
    {
        public readonly AttributeTargets Target;
        public readonly Type Declaring;
        [CanBeNull] public readonly MemberInfo MemberInfo;
        [CanBeNull] public readonly ParameterInfo ParameterInfo;

        public Context(
            AttributeTargets target, 
            Type declaring,
            [CanBeNull] MemberInfo memberInfo = null, 
            [CanBeNull] ParameterInfo parameterInfo = null)
        {
            Target = target;
            Declaring = declaring;
            MemberInfo = memberInfo;
            ParameterInfo = parameterInfo;
        }
    }
}