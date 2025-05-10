using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace LB.Utilities
{
    /// <summary>
    /// Adds menu items to set or clear the scene that the Editor loads first when entering Play Mode.
    /// </summary>
    public static class PlayModeStartSceneSetter
    {
        private const string MenuRoot = "Tools/Play Mode Start Scene/";

        // Menu item to set the currently open scene as the start scene
        [MenuItem(MenuRoot + "Set Current Scene As Start Scene", false, 1)]
        private static void SetCurrentSceneAsPlayModeStart()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (!currentScene.IsValid() || string.IsNullOrEmpty(currentScene.path))
            {
                Debug.LogError("Cannot set start scene. The current scene is not saved or invalid.");
                return;
            }

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(currentScene.path);
            if (sceneAsset == null)
            {
                Debug.LogError($"Failed to load SceneAsset at path: {currentScene.path}");
                return;
            }

            EditorSceneManager.playModeStartScene = sceneAsset;
            Debug.Log($"Play Mode start scene set to: '{sceneAsset.name}' ({currentScene.path})");
        }

        // Validation for the "Set" menu item: Disable if no scene is open/saved
        [MenuItem(MenuRoot + "Set Current Scene As Start Scene", true)]
        private static bool ValidateSetCurrentSceneAsPlayModeStart()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            // Enable only if the current scene is valid and has a path (i.e., is saved)
            return currentScene.IsValid() && !string.IsNullOrEmpty(currentScene.path);
        }

        // --- Clear Setting ---

        // Menu item to clear the start scene setting
        [MenuItem(MenuRoot + "Clear Start Scene", false, 2)]
        private static void ClearPlayModeStartScene()
        {
            EditorSceneManager.playModeStartScene = null;
            Debug.Log("Play Mode start scene cleared. Editor will start with the currently open scene(s).");
        }

        // Validation for the "Clear" menu item: Disable if no start scene is currently set
        [MenuItem(MenuRoot + "Clear Start Scene", true)]
        private static bool ValidateClearPlayModeStartScene()
        {
            // Enable only if a start scene is currently set
            return EditorSceneManager.playModeStartScene != null;
        }
    }
}