using QueueConnect.Development;
using UnityEditor;

namespace QueueConnect.Editor
{
    /// <summary>
    /// Keyboard shortcuts for the Editor
    /// </summary>
    public static class Shortcuts
    {
        /// <summary>
        /// Starts/Exits the PlayMode
        /// </summary>
        [MenuItem("Shortcuts/StartPlayMode _F12")]
        private static void Start_ExitPlayMode()
        {
            // Toggle
            EditorApplication.isPlaying = !EditorApplication.isPlaying;

            if (!EditorApplication.isPlaying)
            {
                DebugLog.White_Green("PlayMode:", "Start");
            }
            else if (EditorApplication.isPlaying)
            {
                DebugLog.White_Red("PlayMode:", "Exit");
            }
        }

        /// <summary>
        /// Pauses the PlayMode
        /// </summary>
        [MenuItem("Shortcuts/PausePlayMode _F11")]
        private static void PausePlayMode()
        {
            // Toggle
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
    }
}