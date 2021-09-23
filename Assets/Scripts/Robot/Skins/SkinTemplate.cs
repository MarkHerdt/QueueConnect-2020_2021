using Sirenix.OdinInspector;
using System;
using QueueConnect.Config;
using UnityEngine;

namespace QueueConnect.Robot.Skins
{
    /*
     * Template for all Robot Skins
     * Just Copy this and change everything that's marked with "CHANGE"
     * Also change "TYPE_NAME" and "SKIN_NAME" in the Documentation and Tooltips to the correct Robot Type name and Skin name
     */

    /// <summary>
    /// TYPE_NAME robot SKIN_NAME skin
    /// </summary>
    [Serializable]
    //[CreateAssetMenu(menuName = "RobotSkins/TYPE_NAME/ID SKIN_NAME", fileName = "ID SKIN_NAME + _SOB")] // CHANGE: Change "TYPE_NAME"/"SKIN_NAME" to the correct names and "ID" to the correct number 
                                                                                                          //         Just look under the current ScriptableObjects what Type name and id you have to use
                                                                                                          //         Also remove the "//" before "[CreateAssetMenu]"
    public class SkinTemplate : RobotParts                                                                // CHANGE: Change the ScriptName
    {
        #region Privates
            private const RobotType TYPE = RobotType.Aqua;                                                // CHANGE: Change to the correct Robot Type
            private const ushort ID = 0;                                                                  // CHANGE: Change to the correct number
            private const string SKIN_NAME = "";                                                          // CHANGE: Change to the correct name
        #endregion
                                                                                                          // CHANGE: Make a new Field for each Sprite the Robot has (Order of the Fields should be the same as in the Robot Prefab)
        #region Inspector Fields
            #pragma warning disable
            [Tooltip("TYPE_NAME Robot Stand sprite")]
            [AssetsOnly, BoxGroup("Robot Sprites")]
            [SerializeField] private Sprite robotStand;    // First Sprite on the RobotSprites has to be the RobotStand!
            [Tooltip("TYPE_NAME Robot SPRITE sprite")]
            [AssetsOnly, BoxGroup("Display Sprites")]
            [SerializeField] private Sprite displaySprite; // RobotStand mustn't be included on the DisplaySprites!
            #pragma warning enable
        #endregion

        /// <summary>
        /// Is only called when the ScriptableObject is created
        /// </summary>
        private SkinTemplate()                                                                            // CHANGE: Change the Constructor name to the same name as the Class
        {
            base.type = TYPE;
            base.skinID = ID;
            base.skinName = SKIN_NAME;
            base.unlocked = false;                                                                        // CHANGE: The default Skin ("ID" = 1) of a Robot Type must always be "base.unlocked = true"
        }                                                                                                 //         Otherwise, set "base.unlocked = false"

        private void OnEnable()
        {
            base.RobotSprites = new Sprite[0];                                                             // CHANGE: Change to correct Array-size (size = number of Sprites)
            base.RobotSprites[0] = robotStand;                                                             // CHANGE: Add every Field to the "base.RobotSprites"-Array (Order must be the same as in the Robot Prefab)
            base.DisplaySprites = new Sprite[0];                                                           // CHANGE: Change to correct Array-size (size = number of Sprites)
            base.DisplaySprites[0] = displaySprite;                                                        // CHANGE: Add every Field to the "base.DisplaySprites"-Array (Order must be the same as in the Robot Prefab)
        }

        public override void UnlockSkin()
        {
            base.unlocked = true;

            RobotPartConfig.LoadRobotSkins();
        }
    }
}