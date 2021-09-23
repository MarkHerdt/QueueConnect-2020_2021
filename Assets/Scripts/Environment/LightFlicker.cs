using QueueConnect.Config;
using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Flickers the Light before it turns off
    /// </summary>
    [HideMonoScript]
    public class LightFlicker : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("\"Light2D\"-Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private SpriteRenderer light2D;
            [Tooltip("Turns the light red after the flickering stops")]
            #pragma warning disable 649
            [SerializeField] private bool turnLightRed;
            #pragma warning restore 649
        #endregion

        #region Privates
            #pragma warning disable 109
            private new PlayerHealthHandler.Lights light;
            #pragma warning restore 109
            private float flickerStep;
            private float nextFlicker;
        #endregion

        #region Properties
            /// <summary>
            /// Light2D-Component
            /// </summary>
            public SpriteRenderer Light2D => light2D;
            /// <summary>
            /// Set to "true" to disable the "Light2D"-Component of this GameObject
            /// </summary>
            public bool DisableLight { get; private set; }
        #endregion

        private void Awake()
        {
            if (light2D == null)
            {
                light2D = GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            if (DisableLight)
            {
                FlickerLight();
            }
        }

        public void TurnLightOff(PlayerHealthHandler.Lights _Light)
        {
            light = _Light;
            DisableLight = true;
        }
        
        /// <summary>
        /// Enables the "Light2D"-Component of this GameObject
        /// </summary>
        public void TurnLightOn()
        {
            DisableLight = false;

            light2D.enabled = true;
        }

        /// <summary>
        /// Flickers the "Light2D"-Component of this GameObject and disables it at the end
        /// </summary>
        private void FlickerLight()
        {
            if (flickerStep >= nextFlicker)
            {
                light2D.enabled = !light2D.enabled;
                nextFlicker = flickerStep + Random.Range(GameConfig.FlickerStep.x, GameConfig.FlickerStep.y);
            }

            flickerStep++;

            if (!(flickerStep >= GameConfig.FlickerDuration)) return;
            
                flickerStep = 0f;
                nextFlicker = 0;
                DisableLight = false;
                this.enabled = false;

                if (turnLightRed)
                {
                    light.SpriteRenderer.color = PlayerHealthHandler.HealthInactiveColor;
                    light.SpriteRenderer.enabled = GameController.GameState == GameState.Playing;
                    //light.Light2D.color = PlayerHealthHandler.HealthInactiveColor;
                }
                else
                {
                    light2D.enabled = false;
                }
        }
    }
}