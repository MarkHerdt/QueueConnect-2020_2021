using UnityEditor;
using UnityEngine;
using System;
using QueueConnect.Config;
using Sirenix.OdinInspector;

namespace QueueConnect.Robot
{
    /// <summary>
    /// Type of the robot
    /// </summary>
    public enum RobotType
    {
        Cyclops = 0,
        Aqua = 1,
        Faun = 2,
    }

    /// <summary>
    /// Base class for all robot skin ScriptableObjects
    /// </summary>
    [Serializable, HideMonoScript, InlineButton("Add_Remove_Skin", "+/-")]
    public abstract class RobotParts : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("Debug", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning enable
            #endif
            [Tooltip("Type of the robot")]
            [BoxGroup("Debug", ShowLabel = false)]
            [SerializeField][ReadOnly] protected RobotType type;
            [Tooltip("Id of this robot skin")]
            [BoxGroup("Debug", ShowLabel = false)]
            [SerializeField][ReadOnly] protected ushort skinID;
            [Tooltip("InGame name of this skin")]
            [BoxGroup("Debug", ShowLabel = false)]
            [SerializeField][ReadOnly] protected string skinName;
            [Tooltip("Has the player unlocked this skin?")]
            [BoxGroup("Debug", ShowLabel = false)]
            [SerializeField] protected bool unlocked;
            #if UNITY_EDITOR
                #pragma warning disable
                [Title("Sprites", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning enable
            #endif
        #endregion

        #region Properties
            /// <summary>
            /// Type of the robot
            /// </summary>
            public RobotType Type => type;
            /// <summary>
            /// Id of this robot skin
            /// </summary>
            public ushort SkinID => skinID;
            /// <summary>
            /// InGame name of this skin
            /// </summary>
            public string SkinName => skinName;
            /// <summary>
            /// Has the player unlocked this skin <br/>
            /// Setter only for testing
            /// </summary>
            public bool Unlocked => unlocked;
            /// <summary>
            /// Sprites for the Robots
            /// </summary>
            public Sprite[] RobotSprites { get; protected set; }
            /// <summary>
            /// Sprites for the Displays
            /// </summary>
            public Sprite[] DisplaySprites { get; protected set; }
        #endregion

        /// <summary>
        /// Unlocks a robot skin
        /// </summary>
        public abstract void UnlockSkin();

        #if UNITY_EDITOR
            private void OnValidate()
            {
                if ((unlocked || !unlocked) && (EditorApplication.isPlaying))
                {
                    UnlockSkin();
                }
            }
        #endif
    }
}