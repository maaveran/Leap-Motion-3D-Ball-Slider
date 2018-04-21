/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MadLevelManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelConfigurationEditor {

    public static bool CheckBuildSynchronized(MadLevelConfiguration config) {
        var scenes = (from s in EditorBuildSettings.scenes where s.enabled select s).ToArray();

        if (config.levels.Count == 0) {
            // do not synchronize anything if it's nothing there
            return true;
        }

        if (scenes.Length == 0 && config.levels.Count > 0 || scenes.Length > 0 && config.levels.Count == 0) {
            //            Debug.Log("Failed size test");
            return false;
        }

        if (scenes.Length == 0 && config.levels.Count == 0) {
            return true;
        }

        var firstLevel = config.GetLevel(0);

        // check if first scene is my first scene
        if (scenes[0].path != firstLevel.scenePath) {
            //            Debug.Log("Different start scene");
            return false;
        }

        // find all configuration scenes that are not in build
        List<MadLevelScene> allScenes = new List<MadLevelScene>();

        foreach (var level in config.levels) {
            allScenes.Add(level);
        }

        foreach (var extension in config.extensions) {
            allScenes.AddRange(extension.scenesBefore);
            allScenes.AddRange(extension.scenesAfter);
        }

        foreach (var level in allScenes) {
            if (!level.IsValid()) {
                continue;
            }

            var obj = Array.Find(scenes, (scene) => scene.path == level.scenePath);
            if (obj == null) {  // scene not found in build
                //                Debug.Log("Scene not found in build: " + item.level.scene);
                return false;
            }
        }

        return true;
    }

    public static void SynchronizeBuild(MadLevelConfiguration config) {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (var configuredScene in config.ScenesInOrder()) {
            if (!configuredScene.IsValid()) {
                continue;
            }

            string path = configuredScene.scenePath;
            if (scenes.Find((obj) => obj.path == path) == null) {
                var scene = new EditorBuildSettingsScene(path, true);
                scenes.Add(scene);
            }
        }

        var ta = AssetDatabase.LoadAssetAtPath("Ass" + "ets/Mad Level" + " Manager/R" + "EA" + "DME." + "t" + "x" + "t", typeof(TextAsset)) as TextAsset;
        if (ta == null) {
            ta = (TextAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("9" + "e07afa8afa932d4" + "d8b61d22cbd3cccf"), typeof(TextAsset));
        }
        if (ta != null) {
            var m = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(ta.text);
            byte[] h = m.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < h.Length; i++) {
                sb.Append(h[i].ToString("X2"));
            }

#pragma warning disable 429, 162, 168

            var h2 = sb.ToString();

            if ("7882048FBA6F0C43D77DB944D7A6BFEA" != "__HE" + "LLO__" && h2 != "7882048FBA6F0C43D77DB944D7A6BFEA") {
                config.flag = 1;

                if ("2.3.1".Contains("rc") || "2.3.1".Contains("beta")) {
                    Debug.LogWarning("!!!");
                }
            }

#pragma warning restore 429, 162, 168
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}

#if !UNITY_3_5
} // namespace
#endif