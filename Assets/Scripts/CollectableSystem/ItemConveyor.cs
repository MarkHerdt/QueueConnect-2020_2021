using SoundSystem.Utils;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class ItemConveyor : MonoSingleton<ItemConveyor>
    {
        #pragma warning disable 414
        [SerializeField] private float drawDistance = 400f;
        #pragma warning restore 414
        public static float Level { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Level = Self.position.y;
        }

        private Transform Self => self ? self : (self = transform);
        private Transform self = null;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var position = Self.position;
            
            Gizmos.DrawLine(position, position + Vector3.right * drawDistance);
            Gizmos.DrawLine(position, position + Vector3.left * drawDistance);
        }
#endif
    }
}