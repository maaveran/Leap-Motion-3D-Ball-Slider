/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MadLevelManager;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelGeneratorTool : EditorWindow {

    #region Fields

    private List<string> generators = new List<string>();

    private int chosenGeneratorIndex;
    private int chosenGroupIndex;
    private int levelsCount = 1;
    private Object scene;

    #endregion

    #region Public Properties
    #endregion

    #region Slots

    void OnEnable() {
        ScanImplementations();
    }

    private void ScanImplementations() {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        Type[] types = currentAssembly.GetTypes();

        foreach (var type in types) {
            if ((typeof (IMadLevelGenerator).IsAssignableFrom(type))) {
                if (!type.IsAbstract) {
                    generators.Add(type.FullName);
                }
            }
        }
    }

    void OnGUI() {
        MadGUI.Info("To create a generator, implement IMadLevelGenerator interface and place your script in any Editor folder.");

        if (generators.Count == 0) {
            MadGUI.Error("I cannot found any generator in your project. Please implement at least one!");
            return;
        }

        chosenGeneratorIndex = EditorGUILayout.Popup("Generator", chosenGeneratorIndex, generators.ToArray());

        MadLevelConfiguration conf = MadLevelConfiguration.GetActive();
        chosenGroupIndex = EditorGUILayout.Popup("Group", chosenGroupIndex, GroupNames(conf));

        scene = EditorGUILayout.ObjectField("Scene", scene, typeof (Object), false);

        levelsCount = EditorGUILayout.IntField("Level Count", levelsCount);

        GUI.enabled = scene != null;
        if (MadGUI.Button("Create")) {
            string generatorName = generators[chosenGeneratorIndex];
            Type generatorType = Type.GetType(generatorName + ", Assembly-CSharp-Editor");
            var generator = Activator.CreateInstance(generatorType) as IMadLevelGenerator;

            MadLevelConfiguration.Group group;
            if (chosenGroupIndex == 0) {
                group = conf.defaultGroup;
            } else {
                group = conf.groups[chosenGroupIndex - 1];
            }

            for (int i = 1; i <= levelsCount; ++i) {
                MadLevelConfiguration.Level level = conf.CreateLevel();
                level.sceneObject = scene;
                level.groupId = group.id;
                level.type = MadLevel.Type.Level;
                level.order = int.MaxValue;
                level.name = generator.GetLevelName(i);
                level.arguments = generator.GetLevelArguments(i);
                conf.levels.Add(level);
                conf.SetDirty();
            }

            EditorUtility.SetDirty(conf);
        }
    }

    private static string[] GroupNames(MadLevelConfiguration conf) {
        List<string> names = new List<string>();
        names.Add(conf.defaultGroup.name);
        foreach (var @group in conf.groups) {
            names.Add(@group.name);
        }
        return names.ToArray();
    }

    #endregion

    #region Public Static Methods

    public static void OpenWindow() {
        GetWindow<MadLevelGeneratorTool>(false, "Level Gen", true);
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

#if !UNITY_3_5
} // namespace
#endif