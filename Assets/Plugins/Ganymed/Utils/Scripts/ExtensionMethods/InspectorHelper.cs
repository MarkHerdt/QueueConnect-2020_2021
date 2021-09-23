using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ganymed.Utils.ExtensionMethods
{
    public static class InspectorHelper
    {
    
        public static string GetTooltip(this Object behaviour, string memberName, bool inherit = true)
        {
            var field = behaviour.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) return string.Empty;
            var attributes = field.GetCustomAttributes();

            foreach (var attribute in field.GetCustomAttributes())
            {
                if (attribute is TooltipAttribute tooltipAttribute)
                {
                    return tooltipAttribute.tooltip;
                }
            }

            return string.Empty;
        }
    
        public static string GetTooltip(this Type type, string memberName, bool inherit = true)
        {
            var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) return string.Empty;
            var attributes = field.GetCustomAttributes();

            foreach (var attribute in field.GetCustomAttributes())
            {
                if (attribute is TooltipAttribute tooltipAttribute)
                {
                    return tooltipAttribute.tooltip;
                }
            }

            return string.Empty;
        }
    }
}
