using UnityEditor;

namespace Ganymed.Utils.Templates
{
#if false
    /// <summary>
    /// Class adding scripting define symbols (SDS).
    /// </summary>
    //[InitializeOnLoad] // uncomment this
    internal sealed class SDS_TEMPLATE
    {
        private const string define = "EXAMPLE"; // Set define at this point.
    
        static SDS_TEMPLATE()
        {
            // Get existing defines
            var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        
            // Check if this define already exists. If it exists return.
            if (defineString.Contains(define)) return;

            // Cut whitespace at the end of the string to determine if it ends with ";"
            while (defineString.EndsWith(" ") && defineString.Length > 0)
            {
                defineString = defineString.Remove(defineString.Length - 1, 1);
            }
            
            // Create a new define string combining existing defines and this.
            defineString += defineString.EndsWith(";") ? $"{define}" : $";{define}";
            
            // Apply updated define string.
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineString);
        }
    }
#endif
}
