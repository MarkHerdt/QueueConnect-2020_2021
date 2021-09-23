using Sirenix.OdinInspector;
using System;
using QueueConnect.Config;
using UnityEngine;

namespace QueueConnect.Robot.Skins.Aqua
{
    /// <summary>
    /// Aqua robot default skin
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "RobotSkins/Aqua/1 Default", fileName = "1 Default_SOB")]
    public class AquaDefaultSkin : RobotParts
    {
        #region Privates
            private const RobotType TYPE = RobotType.Aqua;
            private const ushort ID = 1;
            private const string SKIN_NAME = "Default";
        #endregion

        #region Inspector Field
            #pragma warning disable
            [Tooltip("Aqua RobotStand sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotStand;
            [Tooltip("Aqua Robot Head sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotHead;
            [Tooltip("Aqua Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotTorso;
            [Tooltip("Aqua Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmLeft;
            [Tooltip("Aqua Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmRight;
            [Tooltip("Aqua Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegLeft;
            [Tooltip("Aqua Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegRight;
            [Tooltip("Aqua Robot Head sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayHead;
            [Tooltip("Aqua Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayTorso;
            [Tooltip("Aqua Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmLeft;
            [Tooltip("Aqua Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmRight;
            [Tooltip("Aqua Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegLeft;
            [Tooltip("Aqua Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegRight;
            #pragma warning enable
        #endregion

        /// <summary>
        /// Is only called when the ScriptableObject is created
        /// </summary>
        private AquaDefaultSkin()
        {
            base.type = TYPE;
            base.skinID = ID;
            base.skinName = SKIN_NAME;
            base.unlocked = true;
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
    }
}