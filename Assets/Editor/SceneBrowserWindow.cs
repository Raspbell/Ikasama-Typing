using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; // 必要な名前空間を追加
using System.Collections.Generic;
using System.IO;

public class SceneBrowserWindow : EditorWindow
{
    private Dictionary<string, List<string>> scenesByFolder = new Dictionary<string, List<string>>();
    private Vector2 scrollPosition;

    [MenuItem("Malen/Scene Browser")]
    public static void ShowWindow()
    {
        GetWindow<SceneBrowserWindow>("Scene Browser");
    }

    private void OnEnable()
    {
        RefreshScenes();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            RefreshScenes();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (var folder in scenesByFolder.Keys)
        {
            EditorGUILayout.LabelField(folder, EditorStyles.boldLabel);
            foreach (var scene in scenesByFolder[folder])
            {
                if (GUILayout.Button(scene))
                {
                    OpenScene(folder, scene);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void RefreshScenes()
    {
        scenesByFolder.Clear();

        string[] scenePaths = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);

        foreach (var scenePath in scenePaths)
        {
            string relativePath = "Assets" + scenePath.Replace(Application.dataPath, "").Replace("\\", "/");
            string folder = Path.GetDirectoryName(relativePath);
            string sceneName = Path.GetFileNameWithoutExtension(relativePath);

            if (!scenesByFolder.ContainsKey(folder))
            {
                scenesByFolder[folder] = new List<string>();
            }

            scenesByFolder[folder].Add(sceneName);
        }
    }

    private void OpenScene(string folder, string scene)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string scenePath = folder + "/" + scene + ".unity";
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
