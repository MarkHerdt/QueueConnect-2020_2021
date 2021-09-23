using System;
using SoundSystem.Utils;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class ItemDespawn : MonoSingleton<ItemDespawn>
    {
        #pragma warning disable 414
        [SerializeField] private float drawDistance = 100f;
        #pragma warning restore 414
        public static float Threshold { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Threshold = Self.position.x;
        }

        private Transform Self => self ? self : (self = transform);
        private Transform self = null;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var position = Self.position;
            
            Gizmos.DrawLine(position, position + Vector3.up * drawDistance);
            Gizmos.DrawLine(position, position + Vector3.down * drawDistance);
        }
#endif
    }
}