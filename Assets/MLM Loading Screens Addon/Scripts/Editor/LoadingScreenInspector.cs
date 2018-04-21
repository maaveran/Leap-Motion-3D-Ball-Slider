/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;
using MadLevelManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(LoadingScreen))]
public class LoadingScreenInspector : Editor {

    #region Fields

    private SerializedProperty testMode;
    private SerializedProperty testLevelToLoad;

    private SerializedProperty loadingMethod;

    private SerializedProperty loadingBar;
    private SerializedProperty loadingBarSmooth;
    private SerializedProperty loadingBarSmoothSpeed;

    private SerializedProperty delayLoadingSeconds;

    private SerializedProperty ignoreObjects;

    private SerializedProperty whenLevelLoaded;
    private SerializedProperty timeScaleToOneWhenShown;

    private SerializedProperty waitAndShowSeconds;
    private SerializedProperty waitForBarToFillUp;

    private SerializedProperty changeEnable;
    private SerializedProperty changeDisable;

    private SerializedProperty onLoadedMessage;
    private SerializedProperty onLevelShowMessage;

    #endregion

    #region Methods

    void OnEnable() {
        testMode = serializedObject.FindProperty("testMode");
        testLevelToLoad = serializedObject.FindProperty("testLevelToLoad");
        loadingMethod = serializedObject.FindProperty("loadingMethod");

        loadingBar = serializedObject.FindProperty("loadingBar");
        loadingBarSmooth = serializedObject.FindProperty("loadingBarSmooth");

        loadingBarSmooth = serializedObject.FindProperty("loadingBarSmooth");
        loadingBarSmoothSpeed = serializedObject.FindProperty("loadingBarSmoothSpeed");

        delayLoadingSeconds = serializedObject.FindProperty("delayLoadingSeconds");

        ignoreObjects = serializedObject.FindProperty("ignoreObjects");

        whenLevelLoaded = serializedObject.FindProperty("whenLevelLoaded");
        timeScaleToOneWhenShown = serializedObject.FindProperty("timeScaleToOneWhenShown");

        waitAndShowSeconds = serializedObject.FindProperty("waitAndShowSeconds");
        waitForBarToFillUp = serializedObject.FindProperty("waitForBarToFillUp");

        changeEnable = serializedObject.FindProperty("changeEnable");
        changeDisable = serializedObject.FindProperty("changeDisable");

        onLoadedMessage = serializedObject.FindProperty("onLoadedMessage");
        onLevelShowMessage = serializedObject.FindProperty("onLevelShowMessage");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        MadGUI.BeginBox("Test Mode");
        using (MadGUI.Indent()) {
            MadGUI.PropertyField(testMode, "Enabled");
            var ignoreObject = GameObject.Find("/_mlm_ignore");

            if (testMode.boolValue && ignoreObject == null) {
                new GameObject("_mlm_ignore");
            } else if (!testMode.boolValue && ignoreObject != null) {
                DestroyImmediate(ignoreObject);
            }

            MadGUI.PropertyField(testLevelToLoad, "Load Level");
        }
        MadGUI.EndBox();

        MadGUI.BeginBox("Loading");
        using (MadGUI.Indent()) {
            MadGUI.PropertyFieldEnumPopup(loadingMethod, "Method");
            MadGUI.PropertyField(delayLoadingSeconds, "Delay");
            MadGUI.PropertyField(loadingBar, "Loading Bar", ValidateLoadingBar);
            if (!ValidateLoadingBar(loadingBar)) {
                MadGUI.Error("Attached object does not have MadSprite, UISprite (NGUI) or Image (uGUI) component. " +
                             "Please make sure that you've assigned a valid game object.");
            }

            using (MadGUI.Indent()) {
                MadGUI.PropertyField(loadingBarSmooth, "Smooth");
                using (MadGUI.EnabledIf(loadingBarSmooth.boolValue)) {
                    MadGUI.PropertyField(loadingBarSmoothSpeed, "Smooth Speed");
                    MadGUI.PropertyField(waitForBarToFillUp, "Wait To Fill Up");
                }
            }

        }
        MadGUI.EndBox();

        MadGUI.BeginBox("When Loaded");
        using (MadGUI.Indent()) {
            MadGUI.PropertyFieldEnumPopup(whenLevelLoaded, "");
            EditorGUILayout.Space();

            if (whenLevelLoaded.enumValueIndex == (int) LoadingScreen.WhenLevelLoaded.WaitAndShow) {
                using (MadGUI.Indent()) {
                    MadGUI.PropertyField(waitAndShowSeconds, "Seconds");
                    EditorGUILayout.Space();
                }
            }

            if (whenLevelLoaded.enumValueIndex != (int) LoadingScreen.WhenLevelLoaded.ShowImmediately) {
                GUIGameObjectList("Enable Objects", changeEnable);
                GUIGameObjectList("Disable Objects", changeDisable);

            }

            EditorGUILayout.Space();

            GUILayout.Label("Don't Destroy Objects", "HeaderLabel");
            var arrayList = new MadGUI.ArrayList<string>(ignoreObjects, property => {
                MadGUI.PropertyField(property, "");
            });
            arrayList.drawOrderButtons = false;
            arrayList.Draw();

            GUILayout.Label("Notify", "HeaderLabel");
            FieldMessage(onLoadedMessage);
        }
        MadGUI.EndBox();

        MadGUI.BeginBox("When Level Shown");
        using (MadGUI.Indent()) {
            MadGUI.PropertyField(timeScaleToOneWhenShown, "Set Time Scale To One");

            GUILayout.Label("Notify", "HeaderLabel");
            FieldMessage(onLevelShowMessage);
        }
        MadGUI.EndBox();

        serializedObject.ApplyModifiedProperties();
    }

    private void FieldMessage(SerializedProperty sp) {
        var receiver = sp.FindPropertyRelative("receiver");
        var methodName = sp.FindPropertyRelative("methodName");

        MadGUI.PropertyField(receiver, "");
        if (receiver.objectReferenceValue != null) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(methodName, "Method Name");
            }
        }
    }

    private void GUIGameObjectList(string header, SerializedProperty array) {
        GUILayout.Label(header, "HeaderLabel");

        using (MadGUI.Indent()) {
            var arraylist = new MadGUI.ArrayList<GameObject>(array, property => {
                MadGUI.PropertyField(property, "");
            });
            arraylist.drawOrderButtons = false;
            arraylist.Draw();
        }
    }

    private bool ValidateLoadingBar(SerializedProperty sp) {
        var obj = sp.objectReferenceValue as GameObject;
        if (obj != null) {
            if (obj.GetComponent<MadSprite>() == null && obj.GetComponent("UISprite") == null &&
                obj.GetComponent("Image") == null) {
                return false;
            }
        }

        return true;
    }

    #endregion

    
}

#if !UNITY_3_5
} // namespace
#endif