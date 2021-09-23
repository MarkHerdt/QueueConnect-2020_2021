using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.Test;
using UnityEngine;

namespace QueueConnect.ExtensionMethods
{
    /// <summary>
    /// Enum Tags have to match the one in the Inspector
    /// </summary>
    public enum Tags
    {
        Untagged,
        Respawn,
        Finish,
        EditorOnly,
        MainCamera,
        Player,
        GameController,
        Map,
        Background,
        Border,
        MapExit,
        RobotRelease,
        Scanner,
        LeftScannerElement,
        RightScannerElement,
        ScannerBeam,
        RepairArm
    }

    /// <summary>
    /// ExtensionMethods for Tags
    /// </summary>
    public static class TagExtensions
    {
        /// <summary>
        /// Compares the passed Tag with the Objects Tag and checks if the Tag exists in the Inspector
        /// </summary>
        /// <param name="_GameObject">Object to compare the Tag of</param>
        /// <param name="_Tag">Entry from the "Tags" Enum</param>
        /// <returns>Returns true or false</returns>
        public static bool Tag(this GameObject _GameObject, Tags _Tag)
        {
            return CompareTag(_GameObject.transform, _Tag);
        }

        /// <summary>
        /// Compares the passed Tag with the Objects Tag and checks if the Tag exists in the Inspector
        /// </summary>
        /// <param name="_Transform">Object to compare the Tag of</param>
        /// <param name="_Tag">Entry from the "Tags" Enum</param>
        /// <returns>Returns true or false</returns>
        public static bool Tag(this Transform _Transform, Tags _Tag)
        {
            return CompareTag(_Transform, _Tag);
        }

        /// <summary>
        /// Compares the passed Tag with the Objects Tag and checks if the Tag exists in the Inspector
        /// </summary>
        /// <typeparam name="T">T must be a Component</typeparam>
        /// <param name="_Obj">Object to compare the Tag of</param>
        /// <param name="_Tag">Entry from the "Tags" Enum</param>
        /// <returns>Returns true or false</returns>
        private static bool CompareTag<T>(T _Obj, Tags _Tag) where T : Component
        {
            var _tagExists = false;

            // Goes through all Tags in the Inspector
            foreach (var _inspectorTag in InspectorTags.Tags)
            {
                // Checks if the Tag exist in the Inspector
                if (_inspectorTag == null || _inspectorTag.Replace(" ", "") != _Tag.ToString().Replace(" ", "")) continue;
                
                    _tagExists = true;

                    // Checks if this Object has the passed Tag
                    if (_Obj.tag.Replace(" ", "") == _Tag.ToString().Replace(" ", ""))
                    {
                        return true;
                    }
            }

            if (!_tagExists)
            {
                DebugLog.Red_White_Red("The Tag", $"{_Tag}", "does not exist in the Inspector");
            }

            return false;
        }
    }
}