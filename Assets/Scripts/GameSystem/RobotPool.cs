using Sirenix.OdinInspector;
using System;
using QueueConnect.Robot;
using UnityEngine;
using static QueueConnect.Config.RobotTypePrefab;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Object Pool for the various Robot Types
    /// </summary>
    [Serializable]
    public class RobotPool
    {
        #region Inspector Fields
            [Tooltip("RobotTypes in this Pool")]
            [SerializeField][ReadOnly] private RobotType type;
            [Tooltip("Spawn for this RobotType")]
            [SerializeField][ReadOnly] private RobotSpawn spawn;
            [Tooltip("Index this Robot Type has in the \"robotTypePrefabs\"-List")]
            [SerializeField][ReadOnly] private short typeIndex;
            [Tooltip("Reference to the Pool")]
            [SerializeField][ReadOnly] private ObjectPool pool;
        #endregion
        
        #region Properties
            /// <summary>
            /// RobotTypes in this Pool
            /// </summary>
            public RobotType Type { get => type; private set => type = value; }
            /// <summary>
            /// Spawn chance for Robot Types in this Pool
            /// </summary>
            public RobotSpawn Spawn => spawn;
            /// <summary>
            /// Index this Robot Type has in the "robotTypePrefabs"-List
            /// </summary>
            public short TypeIndex { get => typeIndex; private set => typeIndex = value; }
            /// <summary>
            /// Reference to the Pool
            /// </summary>
            public ObjectPool Pool { get => pool; private set => pool = value; }
            /// <summary>
            /// Number of currently active Robots on the Map
            /// </summary>
            public static uint ActiveRobots { get; set; }
        #endregion

        /// <param name="_Type">RobotTypes in this Pool</param>
        /// <param name="_SpawnChance">Spawn chance for Robot Types in this Pool</param>
        /// <param name="_TypeIndex">Index this Robot Type has in the "robotTypePrefabs"-List</param>
        /// <param name="_StartAmount">How many Robots to instantiate on start</param>
        /// <param name="_Pool">Reference to the Pool</param>
        public RobotPool(RobotType _Type, RobotSpawn _SpawnChance, short _TypeIndex, byte _StartAmount, ObjectPool _Pool)
        {
            this.type = _Type;
            this.spawn = _SpawnChance;
            this.typeIndex = _TypeIndex;
            this.pool = _Pool;
            
            _Pool.AddObject(_StartAmount);
        }
    }
}