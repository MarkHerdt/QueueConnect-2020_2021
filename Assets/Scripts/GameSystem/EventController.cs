using QueueConnect.Config;
using QueueConnect.Robot;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Manages all events
    /// </summary>
    public static class EventController
    {
        /// <summary>
        /// Is fired when a Menu is opened
        /// </summary>
        public static event Action OnMenuOpened;

        /// <summary>
        /// Fires an event for when a Menu is opened
        /// </summary>
        public static void MenuOpened()
        {
            OnMenuOpened?.Invoke();
        }

        
        
        /// <summary>
        /// Is fired when all Menus are closed
        /// </summary>
        public static event Action OnMenuClosed;
        
        /// <summary>
        /// Fires an event for when all Menus are closed
        /// </summary>
        public static void AllMenusClosed()
        {
            OnMenuClosed?.Invoke();
        }

        
        
        /// <summary>
        /// Is fired once when the Game starts
        /// </summary>
        public static event Action OnGameStarted;

        /// <summary>
        /// Fires an event when the Game starts
        /// </summary>
        public static void GameStarted()
        {
            OnGameStarted?.Invoke();
        }



        public delegate void GameEndedCallback(bool wasAborted = false);

        /// <summary>
        /// Is fired once when the Game ends
        /// </summary>
        public static event GameEndedCallback OnGameEnded;

        /// <summary>
        /// Invokes an event noticing systems that the game has been ended.
        /// </summary>
        /// <param name="wasAborted">When true, the game was quit manually</param>
        public static void GameEnded(bool wasAborted)
        {
            OnGameEnded?.Invoke(wasAborted);
        }

        /// <summary>
        /// Is fired when the RobotTypes, that are allowed to spawn have changed (contains the new List of spawnable RobotTypes)
        /// </summary>
        public static event Action<List<RobotType>> OnAllowedRobotTypesChanged;

        /// <summary>
        /// Fires an event when the RobotTypes, that are allowed to spawn have changed
        /// </summary>
        /// <param name="_AllowedTypes">List of RobotTypes that are allowed to spawn</param>
        public static void AllowedRobotTypesChanged(List<RobotType> _AllowedTypes)
        {
            OnAllowedRobotTypesChanged?.Invoke(_AllowedTypes);
        }
        
        /// <summary>
        /// Is fired when a Robot is spawned
        /// </summary>
        public static event Action OnRobotSpawned;

        /// <summary>
        /// Fires an event when a Robot is spawned
        /// </summary>
        public static void RobotSpawned()
        {
            OnRobotSpawned?.Invoke();
        }

        /// <summary>
        /// Is fired when the ConveyorLift releases a Robot
        /// </summary>
        public static event Action OnRobotReleased;

        /// <summary>
        /// Fires an event when the ConveyorLift releases a Robot
        /// </summary>
        public static void RobotReleased()
        {
            OnRobotReleased?.Invoke();
        }

        /// <summary>
        /// Is fired when a Robot is dequeued from the RobotScanner
        /// </summary>
        public static event Action OnRobotDequeued;

        /// <summary>
        /// Fires an event when a Robot is dequeued from the RobotScanner
        /// </summary>
        public static void RobotDequeued()
        {
            OnRobotDequeued?.Invoke();
        }
        
        /// <summary>
        /// Is fired when an Object Pool has Instantiated a new RepairParticle
        /// </summary>
        public static event Action<ObjectPool, GameObject> OnParticleInstantiated;

        /// <summary>
        /// Fires an event with the reference to the Particle that was just Instantiated by an Object Pool and a reference to the Pool that Instantiated the Particle
        /// </summary>
        /// <param name="_Pool">Pool that fired the Event</param>
        /// <param name="_Particle">Particle that was Instantiated</param>
        public static void ParticleInstantiated(ObjectPool _Pool, GameObject _Particle)
        {
            OnParticleInstantiated?.Invoke(_Pool, _Particle);
        }

        
        
        /// <summary>
        /// Is fired when the LightBeam has targeted a Robot
        /// </summary>
        public static event Action OnRobotTargeted;

        /// <summary>
        /// Fires an event when a Robot has been targeted by the LightBeam
        /// </summary>
        public static void RobotTargeted()
        {
            OnRobotTargeted?.Invoke();
        }

        
        
        /// <summary>
        /// Is fired after the LightBeam looses a targeted Robot
        /// </summary>
        public static event Action OnNoRobotTargeted;

        /// <summary>
        /// Fires an event when the LightBeam looses a targeted Robot
        /// </summary>
        public static void NoRobotTargeted()
        {
            OnNoRobotTargeted?.Invoke();
        }

        
        
        /// <summary>
        /// Is fired when a Robot has been fully repaired
        /// </summary>
        public static event Action<RobotBehaviour> OnRobotRepaired;

        /// <summary>
        /// Fires an event when a Robot has been fully repaired
        /// </summary>
        /// <param name="_Robot">The Robot that has been repaired</param>
        public static void RobotRepaired(RobotBehaviour _Robot)
        {
            OnRobotRepaired?.Invoke(_Robot);
        }

        
        
        /// <summary>
        /// Is fired when the List of Parts that are allowed to be disabled has changed
        /// </summary>
        public static event Action OnPartsToDisableChanged;

        /// <summary>
        /// Fires an event that tells the Robots to disable a new set of Sprites
        /// </summary>
        public static void PartsToDisableChanged()
        {
            OnPartsToDisableChanged?.Invoke();
        }

        /// <summary>
        /// Is fired when the ShuffleCountdown duration changes
        /// </summary>
        public static event Action<float> OnShuffleCountdownChanged;
        
        /// <summary>
        /// Fires an Event when the ShuffleCountdown duration is changed
        /// </summary>
        /// <param name="_Duration">The new duration in seconds</param>
        public static void ShuffleCountdownChanged(float _Duration)
        {
            OnShuffleCountdownChanged?.Invoke(_Duration);
        }
        
        /// <summary>
        /// Is fired when a new set of Robot Parts is being shuffled
        /// </summary>
        public static event Action<List<Part>> OnShuffleParts;

        /// <summary>
        /// Fires an event with a reference to a List of Robot Parts
        /// </summary>
        /// <param name="_PartList">List of Robot Parts</param>
        public static void ShuffleParts(List<Part> _PartList)
        {
            OnShuffleParts?.Invoke(_PartList);
        }


        
        /// <summary>
        /// Event is invoked when a new wave has been stated.
        /// </summary>
        public static event Action<int> OnWaveStared;
        
        /// <summary>
        /// Invoke the OnWaveStarted event.
        /// </summary>
        /// <param name="waveIndex"></param>
        public static void StartWave(int waveIndex)
        {
            OnWaveStared?.Invoke(waveIndex);
        }
    }
}