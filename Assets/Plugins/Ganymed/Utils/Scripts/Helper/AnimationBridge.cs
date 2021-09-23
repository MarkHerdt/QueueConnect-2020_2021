using UnityEngine;

namespace Ganymed.Utils.Helper
{
    /// <summary>
    /// Helper class to enable the play/pause method of animations to be invoked by unity events. 
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public class AnimationBridge : MonoBehaviour
    {
        [SerializeField] private Animation targetAnimation;

        private void Awake()
        {
            targetAnimation = targetAnimation ? targetAnimation : GetComponent<Animation>();
            if(targetAnimation == null)
                Debug.Log($"Component of type: {nameof(Animation)} could not be found!");
        }

        public void Play()
        {
            Debug.Log("Play");
            targetAnimation.Play();
        }
    }
}
