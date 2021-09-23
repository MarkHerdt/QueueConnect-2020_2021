using Sirenix.OdinInspector;
using System;
using QueueConnect.Config;
using UnityEngine;

namespace QueueConnect.Robot.Skins.Cyclops
{
    /// <summary>
    /// Humanoid robot default skin
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "RobotSkins/Cyclops/1 Default", fileName = "1 Default_SOB")]
    public class CyclopsDefaultSkin : RobotParts
    {
        #region Privates
            private const RobotType TYPE = RobotType.Cyclops;
            private const ushort ID = 1;
            private const string SKIN_NAME = "Default";
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
        private CyclopsDefaultSkin()
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