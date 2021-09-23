using Sirenix.OdinInspector;
using System;
using QueueConnect.Config;
using UnityEngine;

namespace QueueConnect.Robot.Skins.Cyclops
{
    /// <summary>
    /// Humanoid robot test skin
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "RobotSkins/Cyclops/0 Test", fileName = "0 Test_SOB")]
    public class CyclopsTestSkin : RobotParts
    // #if UNITY_EDITOR
    //     , UnityEditor.Build.IPreprocessBuildWithReport, UnityEditor.Build.IPostprocessBuildWithReport 
    // #endif
    {
        #region Privates
            private const RobotType TYPE = RobotType.Cyclops;
            private const ushort ID = 0;
            private const string SKIN_NAME = "Test";
        #endregion

        #region Inspector Field
            #pragma warning disable
            [Tooltip("Cyclops Robot Stand sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotStand;
            [Tooltip("Cyclops Robot Head sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotHead;
            [Tooltip("Cyclops Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotTorso;
            [Tooltip("Cyclops Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmLeft;
            [Tooltip("Cyclops Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmRight;
            [Tooltip("Cyclops Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegLeft;
            [Tooltip("Cyclops Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegRight;
            [Tooltip("Cyclops Robot Head sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayHead;
            [Tooltip("Cyclops Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayTorso;
            [Tooltip("Cyclops Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmLeft;
            [Tooltip("Cyclops Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmRight;
            [Tooltip("Cyclops Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegLeft;
            [Tooltip("Cyclops Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegRight;
            #pragma warning enable
        #endregion

        /// <summary>
        /// Is only called when the ScriptableObject is created
        /// </summary>
        public CyclopsTestSkin()
        {
            base.type = TYPE;
            base.skinID = ID;
            base.skinName = SKIN_NAME;
            base.unlocked = false;
        }

        private void OnEnable()
        {
            base.RobotSprites = new Sprite[7];
            base.RobotSprites[0] = robotStand;
            base.RobotSprites[1] = robotHead;
            base.RobotSprites[2] = robotTorso;
            base.RobotSprites[3] = robotArmLeft;
            base.RobotSprites[4] = robotArmRight;
            base.RobotSprites[5] = robotLegLeft;
            base.RobotSprites[6] = robotLegRight;
            base.DisplaySprites = new Sprite[6];
            base.DisplaySprites[0] = displayHead;
            base.DisplaySprites[1] = displayTorso;
            base.DisplaySprites[2] = displayArmLeft;
            base.DisplaySprites[3] = displayArmRight;
            base.DisplaySprites[4] = displayLegLeft;
            base.DisplaySprites[5] = displayLegRight;
        }

        public override void UnlockSkin()
        {
            base.unlocked = true;

            RobotPartConfig.LoadRobotSkins();
        }
        
        // #if UNITY_EDITOR
        //     #pragma warning disable 649
        //     [Title("FilePaths", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title;
        //     #pragma warning restore 649
        //     [Tooltip("EditorPrefs-Key of the Original Filepath of this Object")]
        //     [SerializeField][ReadOnly] private string origin;
        //     [Tooltip("EditorPrefs-Key of the Targeted Filepath of this Object")]
        //     [SerializeField][ReadOnly] private string target;
        //
        //     private void OnValidate()
        //     {
        //         if (UnityEditor.AssetDatabase.GetAssetPath(this) == GetString("Origin")) return;
        //         
        //             SetString("Origin", UnityEditor.AssetDatabase.GetAssetPath(this));
        //
        //             var _source = GetString("Origin");
        //             var _file = _source.Substring(_source.LastIndexOf('/'));
        //             var _destination = _source;
        //             
        //             const char _CHARACTER = '/';
        //             
        //             while (_destination != Application.dataPath.Substring(Application.dataPath.LastIndexOf(_CHARACTER) + 1))
        //             {
        //                 _destination = _destination.Substring(0, _destination.LastIndexOf(_CHARACTER));
        //             }
        //             
        //             _destination = $"{_destination}{_file}";
        //             
        //             SetString("Target", _destination);
        //
        //             origin = GetString("Origin");
        //             target = GetString("Target");
        //     }
        //
        //     /// <summary>
        //     /// Saves a string to the EditorPrefs
        //     /// </summary>
        //     /// <param name="_AdditionalKey">Key = typeof(this.GetType() + "_AdditionalKey")</param>
        //     /// <param name="_FilePath">Filepath of this Object</param>
        //     private void SetString(string _AdditionalKey, string _FilePath)
        //     {
        //         UnityEditor.EditorPrefs.SetString($"{GetType().ToString().Substring(GetType().ToString().LastIndexOf('.') + 1)}{_AdditionalKey}", $"{_FilePath}");
        //     }
        //
        //     /// <summary>
        //     /// Gets a string from the EditorPrefs
        //     /// </summary>
        //     /// <param name="_AdditionalKey">Key = typeof(this.GetType() + "_AdditionalKey"</param>
        //     /// <returns></returns>
        //     private string GetString(string _AdditionalKey)
        //     {
        //         return UnityEditor.EditorPrefs.GetString($"{GetType().ToString().Substring(GetType().ToString().LastIndexOf('.') + 1)}{_AdditionalKey}");
        //     }
        //     
        //     public int callbackOrder { get; }
        //     
        //     public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport _Report)
        //     {
        //         // Moves the ScriptableObject out of the Folder it's in before the Build starts
        //         if ((_Report.summary.options & UnityEditor.BuildOptions.Development) == 0)
        //         {
        //             UnityEditor.FileUtil.MoveFileOrDirectory(GetString("Origin"), GetString("Target"));
        //             UnityEditor.FileUtil.MoveFileOrDirectory($"{GetString("Origin")}.meta", $"{GetString("Target")}.meta");
        //             UnityEditor.AssetDatabase.Refresh();
        //         }
        //     }
        //     
        //     public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport _Report)
        //     {
        //         // Moves the ScriptableObject back to its original Folder after the Build has finished
        //         if ((_Report.summary.options & UnityEditor.BuildOptions.Development) == 0)
        //         {
        //             UnityEditor.FileUtil.MoveFileOrDirectory(GetString("Target"), GetString("Origin"));
        //             UnityEditor.FileUtil.MoveFileOrDirectory($"{GetString("Target")}.meta", $"{GetString("Origin")}.meta");
        //             UnityEditor.AssetDatabase.Refresh();
        //         }
        //     }
        // #endif
    }
}