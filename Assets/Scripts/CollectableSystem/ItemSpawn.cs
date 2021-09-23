using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.Color;

namespace QueueConnect.CollectableSystem
{
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public class ItemSpawn : MonoBehaviour
    {
        [SerializeField][Range(1,50)] private ushort spawnValue = 1;

        public ushort SpawnValue => spawnValue;

        public Vector3 StartPosition => Self.position;
        
        private Transform Self => self ? self : (self = transform);
        private Transform self = null;
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = green;
            Gizmos.DrawSphere(Self.position, 3f);
        }
#endif
    }
}