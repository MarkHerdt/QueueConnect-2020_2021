using System;

#if UNITY_EDITOR
namespace SoundSystem.Utils
{
    public class AssetImportHelper : UnityEditor.AssetPostprocessor
    {
        public static event Action OnBeforeAssetsImported;
        public static event Action OnAfterAssetsImported;
        
        public static bool IsImporting => isImporting;
        private static volatile bool isImporting = false;

        private void OnPreprocessAsset()
        {
            OnBeforeAssetsImported?.Invoke();
            isImporting = true;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            isImporting = false;
            OnAfterAssetsImported?.Invoke();
        }
    }
}
#endif
