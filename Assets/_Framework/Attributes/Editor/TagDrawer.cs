using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if String
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use Tag attribute with strings.");
            return;
        }

        // Edit the string value
        string[] tags = InternalEditorUtility.tags;

        if (!tags.Contains(property.stringValue))
        {
            property.stringValue = tags[0];
        }
        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
    }
}
