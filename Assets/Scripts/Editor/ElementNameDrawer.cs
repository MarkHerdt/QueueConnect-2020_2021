using QueueConnect.Attributes;
using UnityEngine;
using UnityEditor;

namespace QueueConnect.Editor
{
    /// <summary>
    /// Override PropertyDrawer of the [ElementName] Attribute
    /// </summary>
    [CustomPropertyDrawer(typeof(ElementNameAttribute))]
    public class ElementNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _Rect, SerializedProperty _Property, GUIContent _Label)
        {
            try
            {
                //current index/position of the element within the IEnumerable
                var _pos = int.Parse(_Property.propertyPath.Split('[', ']')[1]);

                EditorGUI.PropertyField(_Rect, _Property,
                                        ((ElementNameAttribute) attribute).DisplayIndex
                                            ? new GUIContent($"{((ElementNameAttribute) attribute).ElementName} {_pos.ToString(((ElementNameAttribute) attribute).IndexFormat)}")
                                            : new GUIContent(((ElementNameAttribute) attribute).ElementName));
            }
            catch
            {
                EditorGUI.PropertyField(_Rect, _Property, _Label);
            }
        }
    }
}