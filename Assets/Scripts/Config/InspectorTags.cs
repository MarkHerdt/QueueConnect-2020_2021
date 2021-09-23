using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Config
{
    /// <summary>
    /// SafeFile for all Tags from the Inspector
    /// </summary>
    [HideMonoScript]
    [CreateAssetMenu(menuName = "InspectorTags", fileName = "InspectorTags_SOB")]
    public class InspectorTags : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("Inspector Tags", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning enable
            #endif
            [Tooltip("All Tags from the Inspector will be saved in this ScriptableObject when the Game is started in the Editor")]
            [ListDrawerSettings(Expanded = true)]
            #pragma warning disable 649
            [SerializeField][ReadOnly] private string[] tags;
            #pragma warning restore 649
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("FilePath", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning enable
            #endif
            [Tooltip("FilePath to the \"InspectorTags\"-ScriptableObject (Is set inside the Constructor)"), LabelText("InspectorTags FilePath")]
            [FilePath(ParentFolder = "Assets/Resources", Extensions = "asset", RequireExistingPath = true)]
            [ShowInInspector][ReadOnly] private static readonly string INSPECTOR_TAGS_FILEPATH;
        #endregion

        #region Privates
            private static InspectorTags instance;
        #endregion

        #region Properties
            /// <summary>
            /// All Tags from the Inspector will be saved in this ScriptableObject when the Game is started in the Editor
            /// </summary>
            public static string[] Tags => instance.tags;
        #endregion

        static InspectorTags()
        {
            INSPECTOR_TAGS_FILEPATH = "InspectorTags_SOB.asset";
        }

        /// <summary>
        /// Initializes this ScriptableObject<br/>
        /// Is called before the "Awake"-Method<br/>
        /// <b>Method must be static!</b>
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            instance = Singleton.Persistent(instance, INSPECTOR_TAGS_FILEPATH.Substring(0, INSPECTOR_TAGS_FILEPATH.IndexOf('.')));
        }
        
        #if UNITY_EDITOR
            private void OnValidate()
            {
                GetTags();
            }
            
            /// <summary>
            /// Saves all Inspector Tags to this ScriptableObject
            /// </summary>
            private void GetTags()
            {
                tags = new string[UnityEditorInternal.InternalEditorUtility.tags.Length];

                for (ushort i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
                {
                    tags[i] = UnityEditorInternal.InternalEditorUtility.tags[i];
                }
            }
        #endif
    }
}