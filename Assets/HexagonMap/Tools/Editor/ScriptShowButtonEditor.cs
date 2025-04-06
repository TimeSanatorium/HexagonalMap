using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MonoBehaviour), true)]
public class ScriptShowButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targetObject = target as MonoBehaviour;
        var methods = targetObject.GetType().GetMethods();
        foreach (var method in methods)
        {
            var buttonAttribute = (ButtonAttribute)System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
            if (buttonAttribute != null)
            {
                if (GUILayout.Button(buttonAttribute.ButtonName ?? method.Name))
                {
                    method.Invoke(targetObject, null);
                }
            }
        }
    }
}
