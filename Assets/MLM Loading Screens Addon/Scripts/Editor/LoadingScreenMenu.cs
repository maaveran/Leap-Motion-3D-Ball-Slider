/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadLevelManager {

public class LoadingScreenMenu {

    private const string GUIRootGUID = "5d829e57ff6836948b4e01f2ca9d4ce6";
    private const string LoadingScriptGUID = "89c0160f62d8fe44a9b58c4506ad3745";

    [MenuItem("Tools/Mad Level Manager/Create Loading Screen", false, 122)]
    // ReSharper disable once UnusedMember.Local
    static void CreateLoadingScreen() {
        if (!IsSceneEmpty()) {
            if (!EditorUtility.DisplayDialog("Scene not empty",
                "You should create a loading screen only on an empty scene. Are you sure that you want to create it here?",
                "Yes", "No")) {
                return;
            }
        }

        if (Camera.main != null) {
            RemoveMainCamera();
        }

        Initialize();
    }

    private static bool IsSceneEmpty() {
        var objects = Object.FindObjectsOfType(typeof(Transform));
        if (objects.Length == 0) {
            return true;
        }

        if (objects.Length == 1 && objects[0].name == "Main Camera") {
            return true;
        }

        return false;
    }

    private static void RemoveMainCamera() {
        Object.DestroyImmediate(Camera.main.gameObject);
    }

    private static void Initialize() {
        var guiRootPrefab = AssetDatabase.LoadAssetAtPath(
            AssetDatabase.GUIDToAssetPath(GUIRootGUID), typeof(GameObject)) as GameObject;
        var loadingScriptPrefab = AssetDatabase.LoadAssetAtPath(
            AssetDatabase.GUIDToAssetPath(LoadingScriptGUID), typeof (GameObject)) as GameObject;

        var guiRootInstance = Object.Instantiate(guiRootPrefab) as GameObject;
        var loadingScriptInstance = Object.Instantiate(loadingScriptPrefab) as GameObject;

        guiRootInstance.name = guiRootPrefab.name;
        loadingScriptInstance.name = loadingScriptPrefab.name;

        PrefabUtility.DisconnectPrefabInstance(guiRootInstance);
        PrefabUtility.DisconnectPrefabInstance(loadingScriptInstance);

        var loadingBar = MadTransform.FindChild<MadSprite>(guiRootInstance.transform, sprite => sprite.name == "bar").gameObject;
        var loadingText = MadTransform.FindChild<MadText>(guiRootInstance.transform, text => text.name == "loading text").gameObject;
        var loadedText = MadTransform.FindChild<MadText>(guiRootInstance.transform, text => text.name == "loaded text").gameObject;
        var pressAnywhereText = MadTransform.FindChild<MadText>(guiRootInstance.transform, text => text.name == "press anywhere text").gameObject;

        var loadingScreen = loadingScriptInstance.GetComponent<LoadingScreen>();

        loadingScreen.loadingBar = loadingBar;

        loadingScreen.changeDisable.Clear();
        loadingScreen.changeDisable.Add(loadingText);

        loadingScreen.changeEnable.Clear();
        loadingScreen.changeEnable.Add(loadedText);
        loadingScreen.changeEnable.Add(pressAnywhereText);

        MadGameObject.SetActive(loadedText, false);
        MadGameObject.SetActive(pressAnywhereText, false);

        new GameObject("_mlm_ignore");
    }
}

} // namespace