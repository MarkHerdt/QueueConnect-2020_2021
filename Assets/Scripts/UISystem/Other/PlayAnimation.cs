using UnityEngine;

namespace QueueConnect.UISystem.Other
{
    [RequireComponent(typeof(Animation))]
    public class PlayAnimation : MonoBehaviour
    {
        private Animation _animation = null;
        private void Awake()
        {
            _animation = GetComponent<Animation>();
            if (_animation == null)
            {
                _animation = GetComponentInChildren<Animation>();
            }
        }

        public void Play(bool stop)
        {
            if(stop)
                _animation.Stop();
        
            _animation.Play();
        }
    }
}
