using System.Linq;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace QueueConnect.Environment
{
    public class ScannerWall : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("BoxCollider Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private BoxCollider2D boxCollider2D;
            [Tooltip("TrafficLights")]
            [ChildGameObjectsOnly, ListDrawerSettings(Expanded = true)]
            [SerializeField] private  GameObject[] trafficLights = new GameObject[2];
            [Tooltip("Beam GameObject")]
            [ChildGameObjectsOnly]
            [SerializeField] private GameObject beam;
        #endregion

        #region Privates
            private static ScannerWall instance;
            private RobotBehaviour robot;
            private bool trafficLightSet;
        #endregion
        
        private void Awake()
        {
            if (boxCollider2D == null)
            {
                boxCollider2D = GetComponentInChildren<BoxCollider2D>();
            }
            if (trafficLights[0] == null || trafficLights[1] == null)
            {
                trafficLights = GetComponentsInChildren<Light2D>().Select(_Light => _Light.gameObject).ToArray();
            }
            if (beam == null)
            {
                beam = GetComponentInChildren<SpriteMask>().transform.parent.gameObject;
            }

            instance = Singleton.Persistent(this);
        }
        
        private void OnTriggerEnter2D(Collider2D _Collider)
        {
            trafficLightSet = false;
            robot = _Collider.gameObject.GetComponent<RobotBehaviour>();
            
            if (robot != null)
            {
                RobotLeavesMap(robot);

                AudioSystem.PlayVFX(VFX.ScannerStationActive);
                beam.SetActive(true);
            }
        }

        private void OnTriggerStay2D(Collider2D _Collider)
        {
            if (!trafficLightSet && _Collider.bounds.center.x >= transform.position.x)
            {
                trafficLightSet = true;
                CheckIfRepaired(robot.DisabledPartsCount <= 0);
            }
        }

        private void OnTriggerExit2D(Collider2D _Collider)
        {
            if (_Collider == robot.RobotCollider)
            {
                AudioSystem.StopVFX(VFX.ScannerStationActive);
                beam.SetActive(false);
                
                trafficLights[0].SetActive(false);
                trafficLights[1].SetActive(false);
            }
        }

        /// <summary>
        /// Dequeues a Robot from the Queue when it leaves the Map
        /// </summary>
        private void RobotLeavesMap(RobotBehaviour _Robot)
        {
            if (_Robot == RobotScanner.Robots.SafePeek())
            {
                RobotScanner.DequeueRobot(_Robot);
                RayGun.Robots.Enqueue(_Robot);
            }
        }
        
        /// <summary>
        /// Turns the respective Lamp on depending if the Robot is repaired or not
        /// </summary>
        /// <param name="_Repaired">"true" = repaired, "false" = not repaired</param>
        private void CheckIfRepaired(bool _Repaired)
        {
            // Repaired
            if (_Repaired)
            {
                AudioSystem.PlayVFX(VFX.ScannerStationaryScanSuccess);
                
                trafficLights[0].SetActive(false);
                trafficLights[1].SetActive(true);
            }
            // Not repaired
            else
            {
                AudioSystem.PlayVFX(VFX.ScannerStationaryScanFailure);
                
                trafficLights[0].SetActive(true);
                trafficLights[1].SetActive(false);
            }
        }
    }   
}