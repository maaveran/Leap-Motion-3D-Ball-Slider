/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class LoadingScreen : MonoBehaviour {

    #region Public Fields

    public bool testMode;
    public string testLevelToLoad;

    public LoadingMethod loadingMethod;

    public GameObject loadingBar;
    public bool loadingBarSmooth = true;
    public float loadingBarSmoothSpeed = 0.4f;

    public float delayLoadingSeconds = 2;

    public WhenLevelLoaded whenLevelLoaded;
    public bool timeScaleToOneWhenShown = true;

    public float waitAndShowSeconds = 3;
    public bool waitForBarToFillUp = false;

    public List<GameObject> changeEnable = new List<GameObject>();
    public List<GameObject> changeDisable = new List<GameObject>();

    public List<GameObject> eventReceivers = new List<GameObject>();

    public List<string> ignoreObjects = new List<string>();

    private State state = State.Before;

    public Message onLoadedMessage = new Message();
    public Message onLevelShowMessage = new Message();

    #endregion

    #region Private Fields

    private FakeCoroutine testFakeCoroutine = new FakeCoroutine();

    private float lastTime;

    private bool canContinue;
    private List<GameObject> objectsToDestroy;
    private AsyncOperation loadingOperation;

    // allows this script to be enabled only once
    // this fixes the issue with calling OnEnable() when DontDestroy object are passed through
    private bool enabledOnce;

    #endregion


    #region Public Properties

    private float loadingBarValue { get; set; }

    private float loadingBarValueReal {
        get {
            if (loadingBar != null) {
                var spriteComponent = loadingBar.GetComponent<MadSprite>();
                if (spriteComponent != null) {
                    return spriteComponent.fillValue;
                }

                // uGUI
                var uguiImageComponent = loadingBar.GetComponent("Image");
                if (uguiImageComponent != null) {
                    var fillAmountField = uguiImageComponent.GetType().GetProperty("fillAmount");
                    return (float)fillAmountField.GetValue(uguiImageComponent, null);
                }

                // NGUI
                var uiSpriteComponent = loadingBar.GetComponent("UISprite");
                if (uiSpriteComponent != null) {
                    var fillAmountProperty = uiSpriteComponent.GetType().GetProperty("fillAmount");
                    return (float) fillAmountProperty.GetValue(uiSpriteComponent, null);
                }

                Debug.LogError("Loading bar should have one of these components attached: MadSprite, UISprite (NGUI), Image (nGUI)", loadingBar);
                return 0;
            } else {
                return 1; // will immediately go forward when waiting for the bar
            }
        }

        set {
            if (loadingBar != null) {
                var spriteComponent = loadingBar.GetComponent<MadSprite>();
                if (spriteComponent != null) {
                    spriteComponent.fillValue = value;
                    return;
                }

                // uGUI
                var uguiImageComponent = loadingBar.GetComponent("Image");
                if (uguiImageComponent != null) {
                    var fillAmountField = uguiImageComponent.GetType().GetProperty("fillAmount");
                    fillAmountField.SetValue(uguiImageComponent, value, null);
                    return;
                }

                // NGUI
                var uiSpriteComponent = loadingBar.GetComponent("UISprite");
                if (uiSpriteComponent != null) {
                    var fillAmountProperty = uiSpriteComponent.GetType().GetProperty("fillAmount");
                    fillAmountProperty.SetValue(uiSpriteComponent, value, null);
                    return;
                }

                Debug.LogError("Loading bar should have one of these components attached: MadSprite, UISprite (NGUI), Image (nGUI)", loadingBar);
            }
        }

    }

    public bool isDone {
        get {
            if (loadingOperation != null) {
                return loadingOperation.isDone;
            }

            return _isDone;
        }

        set {
            _isDone = value;
        }
    }

    private bool _isDone;

    public float loadingProgress {
        get {
            if (!testMode) {
                return loadingOperation != null ? loadingOperation.progress : 0;
            }

            return testModeProgress;
        }

        set {
            if (testMode) {
                testModeProgress = value;
            } else {
                Debug.LogWarning("This setter can be called only in the test mode.");
            }
        }
    }

    private float testModeProgress;

    #endregion

    #region Slots

    // ReSharper disable once UnusedMember.Local
    void OnEnable() {
        if (enabledOnce) {
            return;
        }
        enabledOnce = true;

        if (testMode && !MadLevel.extensionDefined) {
            Debug.Log("Test mode enabled");
            canContinue = true;
        } else if (!IsCurrentSceneInTheConfiguration() || !MadLevel.hasExtension) {
            Debug.Log("This is not an extension and this is the not proper way of loading this scene. Please enable test mode " +
                      "or see the documentation.");
        } else if (!MadLevel.CanContinue()) {
            Debug.LogError("Loading screen can be set only before target level. Please see the documentation.");
        } else {
            if (testMode) {
                Debug.Log("Test mode disabled because loading scene has been loaded as an extension. " +
                          "This message is not harmful. You can disable the test mode in order to get rid of it.");
                testMode = false;
            }

            canContinue = true;
        }
    }

    // ReSharper disable once UnusedMember.Local
    void Update() {
        if (!canContinue) {
            return;
        }

        UpdateTime();
        UpdateLoadingState();
        UpdateLoadingBar();
    }

    #endregion

    #region Private Methods

    void UpdateTime() {
        deltaTime = 0;
        if (lastTime != 0) {
            deltaTime = Time.realtimeSinceStartup - lastTime;
            testFakeCoroutine.Update(deltaTime);
        }
        lastTime = Time.realtimeSinceStartup;
    }

    private void UpdateLoadingState() {
        switch (state) {
            case State.Before:
                UpdateBefore();
                break;
            case State.Loading:
                UpdateLoading();
                break;
            case State.Loaded:
                UpdateLoaded();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateBefore() {
        if (delayLoadingSeconds < Time.timeSinceLevelLoad) {
            StartLoading();
        }
    }

    private bool IsCurrentSceneInTheConfiguration() {
        return HasExtensionScene(Application.loadedLevelName);
    }

    private void StartLoading() {
        state = State.Loading;

        switch (whenLevelLoaded) {
            case WhenLevelLoaded.ShowImmediately:
                if (waitForBarToFillUp) {
                    DontDestroyAnythingOnLoad();
                }
                break;
            case WhenLevelLoaded.WaitAndShow:
                DontDestroyAnythingOnLoad();
                Time.timeScale = 0;
                break;
            case WhenLevelLoaded.ChangeAndWaitForClick:
                DontDestroyAnythingOnLoad();
                Time.timeScale = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var loadingMethod = GetLoadingMethod();

        switch (loadingMethod) {
            case LoadingMethod.Regular:
                Continue();
                break;
            case LoadingMethod.Async:
                ContinueAsync();
                state = State.Loading;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Continue() {
        if (!testMode) {
            MadLevel.Continue();
            isDone = true;
        } else {
            TestFakeLoadLevel();
        }
    }

    private void TestFakeLoadLevel() {
        testFakeCoroutine.Wait(1.5f, () => {
            isDone = true;
        });
    }

    private void ContinueAsync() {
        if (!testMode) {
            loadingOperation = MadLevel.ContinueAsync();
        } else {
            TestFakeLoadLevelAsync();
        }
    }

    private void TestFakeLoadLevelAsync() {
        loadingProgress = 0.1f;
        testFakeCoroutine.Wait(1f, () => loadingProgress = 0.3f);
        testFakeCoroutine.Wait(1f, () => loadingProgress = 0.4f);
        testFakeCoroutine.Wait(1f, () => loadingProgress = 0.6f);
        testFakeCoroutine.Wait(1f, () => loadingProgress = 0.9f);
        testFakeCoroutine.Wait(1f, () => {
            loadingProgress = 1.0f;
            isDone = true;
        });

    }

    private void DontDestroyAnythingOnLoad() {
        objectsToDestroy = RootObjects();
        foreach (var rootObject in objectsToDestroy) {
            DontDestroyOnLoad(rootObject);
        }
    }

    private void DestroyLoadingScreen() {
        foreach (var o in objectsToDestroy) {
            if (o == null) {
                continue;
            }
            if (!ignoreObjects.Contains(o.name)) {
                Destroy(o);
            }
        }
    }

    private List<GameObject> RootObjects() {
        List<GameObject> result = new List<GameObject>();

        var objects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (var o in objects) {
            if (o.transform.parent == null) {
                result.Add(o);
            }
        }

        return result;
    }

    private LoadingMethod GetLoadingMethod() {
#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1)
        if (!Application.HasProLicense()) {
            return LoadingMethod.Regular;
        }
#endif

        return loadingMethod;
    }

    private void UpdateLoading() {
        loadingBarValue = loadingProgress;
        if (isDone) {
            loadingBarValue = 1;
            loadingFinishedTime = Time.realtimeSinceStartup;

            if (waitForBarToFillUp) {
                if (loadingBarValueReal == 1) {
                    StartLoaded();
                }
            } else {
                StartLoaded();
            }
            
        }
    }

    private float loadingFinishedTime;
    private float deltaTime;

    private void UpdateLoaded() {
        float timeSinceLoad = Time.realtimeSinceStartup - loadingFinishedTime;

        switch (whenLevelLoaded) {
            case WhenLevelLoaded.ShowImmediately:
                if (waitForBarToFillUp) {
                    ShowLevel();
                }
                break;
            case WhenLevelLoaded.WaitAndShow:
                if (timeSinceLoad >= waitAndShowSeconds) {
                    ShowLevel();
                }
                break;
            case WhenLevelLoaded.ChangeAndWaitForClick:
                if (Input.GetMouseButton(0) || Input.touchCount > 0) {
                    ShowLevel();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void UpdateLoadingBar() {
        if (loadingBarValue < loadingBarValueReal || !loadingBarSmooth) {
            loadingBarValueReal = loadingBarValue;
            return;
        }

        float deltaTotal = loadingBarValue - loadingBarValueReal;
        if (deltaTotal == 0) {
            return;
        }

        float delta;

        if (deltaTotal < 0) {
            delta = -loadingBarSmoothSpeed;
        } else {
            delta = loadingBarSmoothSpeed;
        }

        delta *= deltaTime;

        if (Mathf.Abs(delta) > Mathf.Abs(deltaTotal)) {
            loadingBarValueReal = loadingBarValue;
        } else {
            loadingBarValueReal += delta;
        }
    }

    private void StartLoaded() {
        state = State.Loaded;

        onLoadedMessage.Execute();

        if (testMode && TestLevelDefined()) {
            TestLoadLevel();
        }

        if (whenLevelLoaded != WhenLevelLoaded.ShowImmediately) {
            for (int i = 0; i < changeDisable.Count; i++) {
                var toDisable = changeDisable[i];
                if (toDisable != null) {
                    MadGameObject.SetActive(toDisable.gameObject, false);
                }
            }

            for (int i = 0; i < changeEnable.Count; i++) {
                var toEnable = changeEnable[i];
                if (toEnable != null) {
                    MadGameObject.SetActive(toEnable.gameObject, true);
                }
            }
        }
    }

    private void TestLoadLevel() {
        TestLoadLevel(GetLoadingMethod() == LoadingMethod.Async);
    }

    private void TestLoadLevel(bool async) {
        if (HasExtensionScene(testLevelToLoad)) {
            if (async) {
                loadingOperation = MadLevel.LoadLevelByNameAsync(testLevelToLoad);
            } else {
                MadLevel.LoadLevelByName(testLevelToLoad);
            }
        } else {
            if (async) {
                loadingOperation = Application.LoadLevelAsync(testLevelToLoad);
            } else {
                Application.LoadLevel(testLevelToLoad);
            }
        }
    }

    private bool TestLevelDefined() {
        if (string.IsNullOrEmpty(testLevelToLoad)) {
            return false;
        }

        return true;
    }

    private bool HasExtensionScene(string levelName) {

        var extensions = MadLevel.activeConfiguration.extensions;
        for (int i = 0; i < extensions.Count; i++) {
            var extension = extensions[i];

            for (int j = 0; j < extension.scenesBefore.Count; j++) {
                var scene = extension.scenesBefore[j];
                if (scene.sceneName == levelName) {
                    return true;
                }
            }

            for (int j = 0; j < extension.scenesAfter.Count; j++) {
                var scene = extension.scenesBefore[j];
                if (scene.sceneName == levelName) {
                    return true;
                }
            }
        }

        return false;
    }

    private void ShowLevel() {
        onLevelShowMessage.Execute();
        DestroyLoadingScreen();
        if (timeScaleToOneWhenShown) {
            Time.timeScale = 1;
        }
    }

    #endregion


    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes

    public enum LoadingMethod {
        Regular,
        Async,
    }

    public enum WhenLevelLoaded {
        ShowImmediately,
        WaitAndShow,
        ChangeAndWaitForClick,
    }

    public enum State {
        Before,
        Loading,
        Loaded,
    }

    private class FakeCoroutine {

        private List<Function> functionList = new List<Function>();
        private float timeElapsed;
        private float timeAdded = 0;

        public void Wait(float time, F function) {
            functionList.Add(new Function(timeElapsed + timeAdded + time, function));
            functionList.Sort((function1, function2) => function1.executionTime.CompareTo(function2.executionTime));
            timeAdded += time;
        }

        public void Update(float deltaTime) {
            timeElapsed += deltaTime;
            if (functionList.Count > 0) {
                var first = functionList[0];
                if (first.executionTime <= timeElapsed) {
                    first.function();
                    functionList.RemoveAt(0);
                }
            }
        }

        private class Function {
            public readonly float executionTime;
            public readonly F function;

            public Function(float executionTime, F function) {
                this.executionTime = executionTime;
                this.function = function;
            }
        }

        public delegate void F();
    }

    [Serializable]
    public class Message {
        public MonoBehaviour receiver;
        public string methodName;

        public void Execute() {
            if (receiver != null && !string.IsNullOrEmpty(methodName)) {
                receiver.SendMessage(methodName);
            }
        }
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif