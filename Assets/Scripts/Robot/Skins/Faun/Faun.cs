using Sirenix.OdinInspector;
using System;
using UnityEngine;
using QueueConnect.Config;

namespace QueueConnect.Robot.Skins.Faun
{
    /// <summary>
    /// TYPE_NAME robot SKIN_NAME skin
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "RobotSkins/Faun/0 Default", fileName = "1 Default_SOB")]
    public class Faun : RobotParts
    {
        #region Privates
            private const RobotType TYPE = RobotType.Faun;                                                
            private const ushort SKIN_ID = 1;                                                                 
            private const string SKIN_NAME = "Default";                                                
        #endregion
                                                                                                     
        #region Inspector Fields
            #pragma warning disable
            [Tooltip("Faun Robot Stand sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotStand; 
            [Tooltip("Faun Robot Head sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotHead;
            [Tooltip("Faun Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotTorso;
            [Tooltip("Faun Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmLeft;
            [Tooltip("Faun Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotArmRight;
            [Tooltip("Faun Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegLeft;
            [Tooltip("Faun Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotLegRight;
            [Tooltip("Faun Robot Head sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayHead;
            [Tooltip("Faun Robot Torso sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayTorso;
            [Tooltip("Faun Robot left Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmLeft;
            [Tooltip("Faun Robot right Arm sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayArmRight;
            [Tooltip("Faun Robot left Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegLeft;
            [Tooltip("Faun Robot right Leg sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displayLegRight;
            #pragma warning enable
        #endregion

        /// <summary>
        /// Is only called when the ScriptableObject is created
        /// </summary>
        private Faun()                                                                   
        {
            base.type = TYPE;
            base.skinID = SKIN_ID;
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