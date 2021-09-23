using QueueConnect.Attributes;
using UnityEditor;
using UnityEngine;

namespace QueueConnect.Editor
{
    /// <summary>
    /// Override PropertyDrawer of the [ElementNames] Attribute
    /// </summary>
    [CustomPropertyDrawer(typeof(ElementNamesAttribute))]
    public class ElementNamesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _Rect, SerializedProperty _Property, GUIContent _Label)
        {
            try
            {
                //current index/position of the element within the IEnumerable
                var _pos = int.Parse(_Property.propertyPath.Split('[', ']')[1]);
                EditorGUI.PropertyField(_Rect, _Property, new GUIContent(((ElementNamesAttribute)attribute).ElementNames[_pos]));
            }
            catch
            {
                try
                {
                    //current index/position of the element within the IEnumerable
                    var _pos = int.Parse(_Property.propertyPath.Split('[', ']')[1]);

                    EditorGUI.PropertyField(_Rect, _Property,
                                            ((ElementNamesAttribute) attribute).DisplayIndex
                                                ? new GUIContent($"{((ElementNamesAttribute) attribute).DefaultElementName} {_pos.ToString(((ElementNamesAttribute) attribute).IndexFormat)}")
                                                : new GUIContent(((ElementNamesAttribute) attribute).DefaultElementName));
                }
                catch
                {
                    EditorGUI.PropertyField(_Rect, _Property, _Label);
                }
            }
        }
    }
}