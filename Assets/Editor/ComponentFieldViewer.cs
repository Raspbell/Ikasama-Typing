using UnityEngine;
using UnityEditor;
using System.Reflection;

public class ComponentFieldViewer : EditorWindow {
    private Vector2 scrollPosition;

    [MenuItem("Window/Component Field Viewer")]
    public static void ShowWindow() {
        var window = GetWindow<ComponentFieldViewer>("Component Field Viewer");
        window.Show();
    }

    private void OnEnable() {
        EditorApplication.update += MyUpdate;
    }

    private void OnDisable() {
        EditorApplication.update -= MyUpdate;
    }

    void MyUpdate() {
        Repaint();
    }

    void OnGUI() {
        
        if (Selection.activeGameObject == null) {
            GUILayout.Label("No GameObject selected.");
            return;
        }
        else {
            GUILayout.Label(Selection.activeGameObject.name, EditorStyles.boldLabel);
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (Component component in Selection.activeGameObject.GetComponents<Component>()) {
            if (component == null)
                continue;

            GUILayout.Label(component.GetType().Name, EditorStyles.boldLabel);

            FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields) {
                object value = field.GetValue(component);
                string valueStr = value != null ? value.ToString() : "null";
                EditorGUILayout.LabelField(field.Name + " (" + field.FieldType.Name + ")", valueStr);
            }

            GUILayout.Space(10);
        }

        GUILayout.EndScrollView();
    }
}
