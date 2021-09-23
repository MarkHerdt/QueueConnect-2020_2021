using UnityEngine;

namespace SoundSystem.Utils
{
    public static class Extensions
    {
        public static void ForceDestroy(this Component component)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(component);
            }
            else
            {
                Object.DestroyImmediate(component);
            }
        }
    }
}