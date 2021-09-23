using Ganymed.Utils.Attributes;
using UnityEngine;

namespace QueueConnect.SceneManagement
{
    [ScriptOrder(10000)]
    public class InitializationHelper : MonoBehaviour
    {
        public static bool IsDone { get; private set; } = false;
        public static float Progress { get; private set; } = 0;
    
        private void Awake()
        {
            Progress = .3f;
        }

        private void Start()
        {
            Progress =.6f;
        }
        
        private void Update()
        {
            Progress = 1f;
            IsDone = true;
            Destroy(this);
        }
    }
}
