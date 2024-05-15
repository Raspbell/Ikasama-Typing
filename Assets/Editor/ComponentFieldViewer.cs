using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ComponentFieldViewer : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Malen/Component Field Viewer")]
    public static void ShowWindow()
    {
        var window = GetWindow<ComponentFieldViewer>("Component Field Viewer");
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += MyUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= MyUpdate;
    }

    void MyUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (Selection.activeGameObject == null)
        {
            GUILayout.Label("No GameObject selected.");
            return;
        }
        else
        {
            GUILayout.Label(Selection.activeGameObject.name, EditorStyles.boldLabel);
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (Component component in Selection.activeGameObject.GetComponents<Component>())
        {
            if (component == null)
                continue;

            GUILayout.Label(component.GetType().Name, EditorStyles.boldLabel);

            FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(component);
                if (value is IEnumerable collection && !(value is string))
                {
                    EditorGUILayout.LabelField(field.Name + " (" + field.FieldType.Name + ")", "Collection with elements:");
                    EditorGUI.indentLevel++;
                    if (value is IDictionary dictionary)
                    {
                        foreach (DictionaryEntry entry in dictionary)
                        {
                            string keyStr = entry.Key != null ? entry.Key.ToString() : "null";
                            string valueStr = entry.Value != null ? entry.Value.ToString() : "null";
                            EditorGUILayout.LabelField("Key: " + keyStr, "Value: " + valueStr);
                        }
                    }
                    else
                    {
                        int index = 0;
                        foreach (var element in collection)
                        {
                            string elementStr = element != null ? element.ToString() : "null";
                            EditorGUILayout.LabelField("[" + index + "]", elementStr);
                            index++;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    string valueStr = value != null ? value.ToString() : "null";
                    EditorGUILayout.LabelField(field.Name + " (" + field.FieldType.Name + ")", valueStr);
                }
            }

            GUILayout.Space(10);
        }

        GUILayout.EndScrollView();
    }
}
