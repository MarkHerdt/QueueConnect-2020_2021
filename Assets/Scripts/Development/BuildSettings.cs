namespace QueueConnect.Development
{
    #if UNITY_EDITOR
        /// <summary>
        /// Sets values in the PlayerSettings depending on the Build
        /// </summary>
        public class BuildSettings : UnityEditor.Build.IPreprocessBuildWithReport
        {
            public int callbackOrder { get; }
            
            public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport _Report)
            {
                var _target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
                var _group = UnityEditor.BuildPipeline.GetBuildTargetGroup(_target);

                // Development
                if ((_Report.summary.options & UnityEditor.BuildOptions.Development) != 0)
                {
                    //C++ Compiler Configuration = Debug
                    UnityEditor.PlayerSettings.SetIl2CppCompilerConfiguration(_group, UnityEditor.Il2CppCompilerConfiguration.Debug);
                    //Managed Stripping Level = Disabled
                    UnityEditor.PlayerSettings.SetManagedStrippingLevel(_group, UnityEditor.ManagedStrippingLevel.Disabled);
                    //Minify
                    //-Release = None
                    UnityEditor.EditorUserBuildSettings.androidReleaseMinification = UnityEditor.AndroidMinification.None;
                    //-Debug = None
                    UnityEditor.EditorUserBuildSettings.androidDebugMinification = UnityEditor.AndroidMinification.None;
                    //Compression Method = LZ4
                }
                // Release
                else
                {
                    //C++ Compiler Configuration = Master
                    UnityEditor.PlayerSettings.SetIl2CppCompilerConfiguration(_group, UnityEditor.Il2CppCompilerConfiguration.Master);
                    //Managed Stripping Level = High
                    UnityEditor.PlayerSettings.SetManagedStrippingLevel(_group, UnityEditor.ManagedStrippingLevel.High);
                    //Minify
                    //-Release = Gradle (Experimental)
                    UnityEditor.EditorUserBuildSettings.androidReleaseMinification = UnityEditor.AndroidMinification.Gradle;
                    //-Debug = Gradle (Experimental)
                    UnityEditor.EditorUserBuildSettings.androidDebugMinification = UnityEditor.AndroidMinification.Gradle;
                    //Compression Method = LZ4HC
                }
            }
        }   
    #endif
}