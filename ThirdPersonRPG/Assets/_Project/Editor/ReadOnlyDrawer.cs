#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LB.Utilities
{
    /// <summary>
    /// The property drawer for the ReadOnly attribute.
    /// Displays the property in the Inspector but makes it non-editable.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Disable editing for the next control
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif
