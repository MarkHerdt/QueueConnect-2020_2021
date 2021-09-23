using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem.Other
{
    public class ResetTextColor : MonoBehaviour
    {
        [SerializeField] private Text field = null;
        private void OnEnable() => field.color = new Color(field.color.r, field.color.g, field.color.b, 0);
    }
}