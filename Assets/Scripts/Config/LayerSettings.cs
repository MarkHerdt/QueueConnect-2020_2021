using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Config
{
    /// <summary>
    /// Enum Layer have to match the one in the Inspector
    /// </summary>
    public enum Layer
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Water,
        UI,
        Robot
    }


    /// <summary>
    /// Scriptable Object to manage all Layer settings
    /// </summary>
    [HideMonoScript]
    [CreateAssetMenu(menuName = "LayerSettings", fileName = "LayerSettings_SOB")]
    public class LayerSettings : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("Layer", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning enable
            #endif
            [Tooltip("Layer for all Robots")]
            [SerializeField] private Layer robotLayer = Layer.Robot;
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("FilePath", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning enable
            #endif
            [Tooltip("FilePath to the \"LayerSettings\"-ScriptableObject (Is set inside the Constructor)"), LabelText("LayerSettings FilePath")]
            [FilePath(ParentFolder = "Assets/Resources", Extensions = "asset", RequireExistingPath = true)]
            [ShowInInspector][ReadOnly] private static readonly string LAYER_SETTINGS_FILEPATH;
        #endregion

        #region Privates
            private static LayerSettings instance;
        #endregion

        #region Properties
            /// <summary>
            /// Layer for all Robots
            /// </summary>
            public static Layer RobotLayer => instance.robotLayer;
        #endregion

        static LayerSettings()
        {
            LAYER_SETTINGS_FILEPATH = "LayerSettings_SOB.asset";
        }

        /// <summary>
        /// Initializes this ScriptableObject<br/>
        /// Is called after the "Awake"-Method<br/>
        /// <b>Method must be static!</b>
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            instance = Singleton.Persistent(instance, LAYER_SETTINGS_FILEPATH.Substring(0, LAYER_SETTINGS_FILEPATH.IndexOf('.')));
        }
    }
}
