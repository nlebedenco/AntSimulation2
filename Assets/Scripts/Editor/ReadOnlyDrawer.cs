using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    private bool CanEdit(RunMode scope)
    {
        switch (scope)
        {
            case RunMode.Play:
                return !(EditorApplication.isPlaying || EditorApplication.isPaused);
            case RunMode.Editor:
                return (EditorApplication.isPlaying || EditorApplication.isPaused);
            default:
                return false;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (ReadOnlyAttribute)this.attribute;
        GUI.enabled = CanEdit(attribute.scope);
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[CustomPropertyDrawer(typeof(ReadOnlyRangeAttribute))]
public class ReadOnlyRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (ReadOnlyRangeAttribute)this.attribute;
        bool canEdit = !(EditorApplication.isPlaying || EditorApplication.isPaused);
        GUI.enabled = canEdit;
        if (property.propertyType == SerializedPropertyType.Float)
            EditorGUI.Slider(position, property, attribute.min, attribute.max, label);
        else if (property.propertyType == SerializedPropertyType.Integer)
            EditorGUI.IntSlider(position, property, (int)attribute.min, (int)attribute.max, label);
        else
            EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
        GUI.enabled = !canEdit;
    }
}