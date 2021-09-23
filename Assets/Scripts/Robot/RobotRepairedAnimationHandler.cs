using System;
using System.Collections;
using System.Threading.Tasks;
using Ganymed.Utils;
using Ganymed.Utils.Callbacks;
using QueueConnect.GameSystem;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace QueueConnect.Robot
{
    [RequireComponent(typeof(Animation))]
    public class RobotRepairedAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animation onRepairAnimation = null;
        [SerializeField][Range(0,1000)] private int animationDelay = 250;
        private Vector3 origin;
        private bool init = false;
        
        public void Reset()
        {
            onRepairAnimation.Stop();
            if (init)
            {
                transform.localPosition = origin;
            }
            gameObject.SetActive(false);
        }

        public async void StartAnimation()
        {
            await Task.Delay(animationDelay);
            if(!Application.isPlaying || GameController.GameState != GameState.Playing) return;
            origin = transform.localPosition;
            init = true;
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            onRepairAnimation.Play();
        }

        protected RobotRepairedAnimationHandler() =>
            UnityEventCallbacks.AddEventListener(() => gameObject.SetActive(false), ApplicationState.PlayMode,
                UnityEventType.Awake);
    }
}