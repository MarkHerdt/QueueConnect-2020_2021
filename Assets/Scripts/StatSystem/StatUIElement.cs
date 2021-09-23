using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.StatSystem;
using QueueConnect.UISystem;
using TMPro;
using UnityEngine;

public class StatUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text nameField = null;
    [SerializeField] private TMP_Text valueField = null;
    
    public static StatUIElement Create(StatUIElement prefab, Transform where, Stat stat)
    {
        var element = Instantiate(prefab, where);

        
        element.valueField.SetText(stat.value);
        
        element.valueField.color = stat.color;
        element.nameField.color = stat.color;
        
        LocalizedTextComponent.CreateInstance(element.nameField.gameObject, stat.name);
        
        if (stat.isEmpty)
        {
            ((RectTransform) element.transform).sizeDelta = new Vector2(0, 5);
            element.nameField.SetText("");
        }

        
#if UNITY_EDITOR
        element.gameObject.name = $"STAT:{stat.name}_{stat.value}";
#endif
        
        return element;
    }
}