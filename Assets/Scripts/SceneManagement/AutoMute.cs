using Ganymed.Utils.Attributes;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.SceneManagement
{
    [ScriptOrder(-1000)]
    [RequireComponent(typeof(AudioSource))]
    public class AutoMute : MonoBehaviour
    {
        
        private void Awake()
        {
            if (AudioSystem.IsMuted)
            {
                GetComponent<AudioSource>().mute = true;
            }
        }
    }
}
