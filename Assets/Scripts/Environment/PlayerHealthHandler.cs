using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.Config;
using QueueConnect.GameSystem;
using QueueConnect.Platform;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace QueueConnect.Environment
{
    [HideMonoScript]
    [ExecuteInEditMode]
    public class PlayerHealthHandler : MonoBehaviour
    {
        #region Inspector Fields

        [SerializeField] private AnimationCurve slowCurve = null;
        [SerializeField] [Range(0,5f)] private float slowDuration = 1f;
        
        [Space]

        [Tooltip("Color when health is on")] [BoxGroup("Color")] [SerializeField]
        private Color32 healthActiveColor = new Color32(144, 255, 0, 255);

        [Tooltip("Color when armor is on")] [BoxGroup("Color")] [SerializeField]
        private Color32 armorActiveColor = new Color32(0, 255, 173, 255);

        [Tooltip("Color when health is off")] [BoxGroup("Color")] [SerializeField]
        private Color32 healthInactiveColor = new Color32(255, 0, 0, 255);

        [Tooltip("Color when armor is off")] [BoxGroup("Color")] [SerializeField]
        private Color32 inactiveColor = new Color32(255, 255, 255, 0);

        [Tooltip("Delay in milliseconds between each life, when they're added at game start")] [SerializeField]
        private short lifeDelay = 250;

        [InfoBox("First the \"Life\"-Sprites from low to high, then the \"Armor\"-Sprites from, low to high")]
        [SerializeField]
        private Lights[] lights = new Lights[6];

        [Header("Debug")] [Tooltip("Current life of Player")] [ShowInInspector] [ReadOnly]
        
        private static byte currentHealth;

        private static byte currentArmor;
        
        private static bool isInvincible = false;

        #endregion

        #region Privates

        private static PlayerHealthHandler instance;
        private static bool isResetting;

        #endregion

        #region Properties

        public static bool IsInvincible => isInvincible;
        
        public static byte CurrentHealth => currentHealth;
        public static byte CurrentArmor => currentArmor;

        
        /// <summary>
        /// Color when health is off
        /// </summary>
        public static Color32 HealthInactiveColor => instance.healthInactiveColor;

        #endregion

        #region --- [EVENTS] ---
        
        public delegate void VitalPointGainedDelegate(int amount = 1, bool reset = false);

        public static event VitalPointGainedDelegate OnHealthGained;
        public static event VitalPointGainedDelegate OnHealthLost;
        public static event VitalPointGainedDelegate OnArmorGained;
        public static event VitalPointGainedDelegate OnArmorLost;

        public static event Action<bool> OnPlayerInvincibilityChanged; 
        
        #endregion

        private void Awake()
        {
            var _isNull = lights.Any(_Light => _Light.SpriteRenderer == null || _Light.LightFlicker == null);

            if (lights.Length != 6 || _isNull)
            {
                var _children = GetComponentsInChildren<LightFlicker>();

                lights = new Lights[6];

                for (byte i = 0; i < lights.Length; i++)
                {
                    lights[i] = new Lights(_SpriteRenderer: _children[i].gameObject.GetComponent<SpriteRenderer>(),
                        //_Light2D: _children[i].gameObject.GetComponent<Light2D>(),
                        _LightFlicker: _children[i]);
                }
            }

            instance = Singleton.Persistent(this);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                instance = Singleton.Persistent(this);
            }
#endif

            // EventController.OnMenuOpened += DisableLights;
            // EventController.OnMenuClosed += EnableLights;
        }

        // private void OnDisable()
        // {
        //     EventController.OnMenuOpened -= DisableLights;
        //     EventController.OnMenuClosed -= EnableLights;
        // }

        private void Start()
        {
            ResetLife(0);
        }



        #region --- [ADD ARMOR / HEALTH] ---

        /// <summary>
        /// Gain a health point
        /// </summary>
        /// <param name="reset">when true the health is gained by a reset and will not increase stats</param>
        public static bool AddLife(bool reset)
        {
            if (currentHealth >= 3) return false;
          
            instance.lights[currentHealth].SpriteRenderer.color =instance.healthActiveColor;
            //instance.lights[currentHealth].Light2D.color =instance.healthActiveColor;
            instance.lights[currentHealth].LightFlicker.TurnLightOn();

            if (reset) return false;
            
            currentHealth++;
            OnHealthGained?.Invoke();
            return true;
        }
        
        
        
        /// <summary>
        /// Gain an armor point
        /// </summary>
        /// <param name="reset">when true the health is gained by a reset and will not increase stats</param>
        public static bool AddArmor(bool reset)
        {
            if (currentArmor >= 3) return false;

            instance.lights[currentArmor + 3].SpriteRenderer.color = instance.armorActiveColor;
            //instance.lights[currentArmor + 3].Light2D.color = instance.armorActiveColor;
            instance.lights[currentArmor + 3].LightFlicker.TurnLightOn();

            if (reset) return false;
            
            currentArmor++;
            OnArmorGained?.Invoke();
            return true;
        }

        #endregion

        #region --- [LOSE ARMOR / HEALTH] ---

        /// <summary>
        /// Subtracts one life
        /// </summary>
        public static void SubtractLife()
        {
            if (Application.isPlaying && (GameConfig.Invincible || GameController.GameState != GameState.Playing)) return;
            if (isInvincible)
            {
                //TODO: add to stats
                return;
            }

            if (currentArmor > 0)
            {
                
                currentArmor--;

                instance.lights[currentArmor + 3].SpriteRenderer.color = instance.inactiveColor;
                instance.lights[currentArmor + 3].LightFlicker.enabled = true;

                if (Application.isPlaying)
                {
                    instance.lights[currentArmor + 3].LightFlicker.TurnLightOff(instance.lights[currentArmor + 3]);
                }
                OnArmorLost?.Invoke();
            }
            else if (currentHealth > 0)
            {
                currentHealth--;

                instance.lights[currentHealth].SpriteRenderer.color = instance.inactiveColor;
                instance.lights[currentHealth].LightFlicker.enabled = true;

                if (Application.isPlaying)
                {
                    instance.lights[currentHealth].LightFlicker.TurnLightOff(instance.lights[currentHealth]);
                }
                OnHealthLost?.Invoke();
            }
            
            
            // --- Check if player has lost all of their health
            if (currentHealth <= 0 && Application.isPlaying && (GameController.GameState == GameState.Playing || GameController.IsPaused))
            {
                AudioSystem.PlayVFX(VFX.OnGameOver);
                RobotScanner.DestroyAllRobots(false);
                GameController.SetGameState(GameState.Menu);
                
                // Has been revived already
                if (AdvertisementHelper.HasBeenRevived)
                {
                    EventController.GameEnded(false);
                }
                // Has not been revived in this round
                else
                {
                    GameController.PauseGame();
                    AdvertisementHelper.OfferPlayerReward();
                }
            }
            else
            {
                if (GameController.IsTutorial) return;
                
                    if (GameController.SlowTime(instance.slowDuration, instance.slowCurve, callback: cancelled =>
                    {
                        if(!RobotScanner.AutoRepairRobots)
                            SetInvincibility(false);
                    }))
                    {
                        SetInvincibility(true);
                    }
            }
        }

        public static void RevivePlayer()
        {
            AddLife(false);
            GameController.ResumeGame();
            GameConfig.SetRobotSpawn(true);
        }
        
        #endregion

        #region --- [INVINCIBILITY] ---

        /// <summary>
        /// Set invincibility for the player health
        /// </summary>
        /// <param name="invincible"></param>
        public static void SetInvincibility(bool invincible) => instance.ApplyInvincibility(invincible);
        
        private void ApplyInvincibility(bool invincible)
        {
            isInvincible = invincible;
            OnPlayerInvincibilityChanged?.Invoke(isInvincible);
        }

        #endregion

        
        
        /// <summary>
        /// Resets the Players life
        /// </summary>
        /// <param name="_SetLifeTo">Value to set the life to, min = 0 | max = 6 | default = 3</param>
        public static async void ResetLife(byte _SetLifeTo = 3, bool _GameStart = false)
        {
            if (isResetting) return;

            isResetting = true;

            try
            {
                currentHealth = _SetLifeTo;
                currentArmor = 0;
                
                
                for (var i = 0; i < 6; i++)
                {
                    //instance.lights[i].Light2D.enabled = false;
                    instance.lights[i].SpriteRenderer.color = i < 3 ? instance.healthInactiveColor : instance.inactiveColor;
                    instance.lights[i].LightFlicker.enabled = false;

                    if (Application.isPlaying)
                    {
                        instance.lights[i].LightFlicker.TurnLightOff(instance.lights[currentArmor + 3]);
                    }
                }


                for (var i = 0; i < currentHealth; i++)
                {
                    if (_GameStart) await Task.Delay(instance.lifeDelay);
                    instance.lights[i].SpriteRenderer.color = instance.healthActiveColor;
                    await TaskHelper.AwaitCondition(() => GameController.GameState != GameState.Playing);
                    instance.lights[i].LightFlicker.TurnLightOn();
                    // instance.lights[i].Light2D.color = instance.healthActiveColor;
                    // instance.lights[i].Light2D.enabled = true;
                }
            }
            catch
            {
                isResetting = false;
                return;
            }

            isResetting = false;
        }

        // /// <summary>
        // /// Disables the 2D-Light
        // /// </summary>
        // private void DisableLights()
        // {
        //     for (byte i = 0; i < 6; i++)
        //     {
        //         lights[i].LightFlicker.Light2D.enabled = false;
        //     }
        // }

        // /// <summary>
        // /// Enables the 2D-Light
        // /// </summary>
        // private void EnableLights()
        // {
        //     // enable health lights
        //     for (byte i = 0; i < currentHealth; i++)
        //     {
        //         lights[i].LightFlicker.Light2D.enabled = true;
        //     }
        //     
        //     // enable armor lights
        //     for (byte i = 3; i < currentArmor + 3; i++)
        //     {
        //         lights[i].LightFlicker.Light2D.enabled = true;
        //     }
        // }

#if UNITY_EDITOR
        /// <summary>
        /// Sets the values of the Lights to their Prefab state
        /// </summary>
        [Button]
        private void ResetValues()
        {
            foreach (var _light in lights)
            {
                PrefabUtility.RevertObjectOverride(_light.SpriteRenderer, InteractionMode.UserAction);
                //PrefabUtility.RevertObjectOverride(_light.LightFlicker.Light2D, InteractionMode.UserAction);
            }

            currentHealth = 0;
        }
#endif

        /// <summary>
        /// Dataclass for the "SpriteRenderer"/"Light2D"-Components of the Lights
        /// </summary>
        [Serializable]
        public class Lights
        {
            #region Inspector Fields

            [Tooltip("\"SpriteRenderer\"-Component of the \"Light\"")] [SerializeField, BoxGroup("Lights")]
            private SpriteRenderer spriteRenderer;

            // [Tooltip("\"Light2D\"-Component of the \"Light\"")] [SerializeField, BoxGroup("Lights")]
            // private Light2D light2D;

            [Tooltip("\"LightFlicker\"-Component of the \"Light\"")] [SerializeField, BoxGroup("Lights")]
            private LightFlicker lightFlicker;

            #endregion

            #region Properties

            /// <summary>
            /// "SpriteRenderer"-Component of the "Light"-GameObject
            /// </summary>
            public SpriteRenderer SpriteRenderer => spriteRenderer;

            /// <summary>
            /// "Light2D"-Component of the "Light"-GameObject
            /// </summary>
            //public Light2D Light2D => light2D;

            /// <summary>
            /// "LightFlicker"-Component of the "Light"
            /// </summary>
            public LightFlicker LightFlicker => lightFlicker;

            #endregion

            /// <param name="_SpriteRenderer">"SpriteRenderer"-Component of the "Light"-GameObject</param>
            /// <param name="_Light2D">"Light2D"-Component of the "Light"-GameObject</param>
            /// <param name="_LightFlicker">"LightFlicker"-Component of the "Light"</param>
            public Lights(SpriteRenderer _SpriteRenderer, /*Light2D _Light2D,*/ LightFlicker _LightFlicker)
            {
                this.spriteRenderer = _SpriteRenderer;
                //this.light2D = _Light2D;
                this.lightFlicker = _LightFlicker;
            }
        }
    }
}