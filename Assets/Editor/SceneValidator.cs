#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class SceneValidator
{
    [MenuItem("Tools/Validate Scenes for EventSystem & Managers")]
    public static void ValidateScenes()
    {
        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene");
        Debug.Log($"SceneValidator: Found {sceneGUIDs.Length} scenes to scan.");

        foreach (var guid in sceneGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            if (!scene.IsValid()) continue;

            bool hasEventSystem = false;
            bool hasQuestManager = false;

            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>(true) != null)
                    hasEventSystem = true;
                if (go.GetComponentInChildren<QuestManager>(true) != null)
                    hasQuestManager = true;
                if (hasEventSystem && hasQuestManager) break;
            }

            if (hasEventSystem || hasQuestManager)
            {
                Debug.Log($"[SceneValidator] Scene: {path} - EventSystem: {hasEventSystem}, QuestManager: {hasQuestManager}");
            }

            // No need to save, just close
            EditorSceneManager.CloseScene(scene, true);
        }

        Debug.Log("SceneValidator: scan complete.");
    }
}
#endif