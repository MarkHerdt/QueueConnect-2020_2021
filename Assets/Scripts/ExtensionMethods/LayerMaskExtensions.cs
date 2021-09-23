using QueueConnect.Config;
using QueueConnect.Development;
using UnityEngine;

namespace QueueConnect.ExtensionMethods
{
    /// <summary>
    /// ExtensionMethods for the LayerMask struct
    /// </summary>
    public static class LayerMaskExtensions
    {
        #region Privates
            private const int LAYER_COUNT = 32;
        #endregion

        /// <summary>
        /// Checks if the passed Layer exists in the Inspector and returns it as a LayerMask
        /// </summary>
        /// <param name="_LayerMask">LayerMask Object</param>
        /// <param name="_LayerName">Entry from the "Layer" Enum</param>
        /// <returns>Returns a LayerMask</returns>
        public static LayerMask LayerToLayerMask(this LayerMask _LayerMask, Layer _LayerName)
        {
            // Checks if the Layer from the Enum exists in the Inspector
            for (var i = 0; i < LAYER_COUNT; i++)
            {
                if (LayerMask.LayerToName(i).Replace(" ", "") == _LayerName.ToString().Replace(" ", ""))
                {
                    return LayerMask.GetMask(LayerMask.LayerToName(i));
                }
            }

            DebugLog.Red_White_Red("The LayerMask", $"{_LayerName}", "does not exist in the Inspector");
            return LayerMask.GetMask(LayerMask.LayerToName(0));
        }

        /// <summary>
        /// Checks if the passed Layer exists in the Inspector and returns it as an Int
        /// </summary>
        /// <param name="_LayerMask">LayerMask Object</param>
        /// <param name="_LayerName">Entry from the "Layer" Enum</param>
        /// <returns>Returns the int value of the LayerMask</returns>
        public static int LayerToInt(this LayerMask _LayerMask, Layer _LayerName)
        {
            // Checks if the Layer from the Enum exists in the Inspector
            for (var i = 0; i < LAYER_COUNT; i++)
            {
                if (LayerMask.LayerToName(i).Replace(" ", "") == _LayerName.ToString().Replace(" ", ""))
                {
                    return LayerMask.NameToLayer(LayerMask.LayerToName(i));
                }
            }

            DebugLog.Red_White_Red("The LayerMask", $"{_LayerName}", "does not exist in the Inspector");
            return LayerMask.NameToLayer(LayerMask.LayerToName(0));
        }

        /// <summary>
        /// Checks if the passed Layer exists in the Inspector and returns it as a String
        /// </summary>
        /// <param name="_LayerMask">LayerMask Object</param>
        /// <param name="_LayerName">Entry from the "Layer" Enum</param>
        /// <returns>Returns the name of the LayerMask as a String</returns>
        public static string LayerToString(this LayerMask _LayerMask, Layer _LayerName)
        {
            // Checks if the Layer from the Enum exists in the Inspector
            for (var i = 0; i < LAYER_COUNT; i++)
            {
                if (LayerMask.LayerToName(i).Replace(" ", "") == _LayerName.ToString().Replace(" ", ""))
                {
                    return LayerMask.LayerToName(i);
                }
            }

            DebugLog.Red_White_Red("The LayerMask", $"{_LayerName}", "does not exist in the Inspector");
            return LayerMask.LayerToName(0);
        }
    }
}