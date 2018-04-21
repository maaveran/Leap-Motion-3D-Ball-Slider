/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
#if !UNITY_3_5
namespace MadLevelManager {
#endif
using MadLevelManager;

public class MadLevelGroupsEditTool : EditorWindow {

    private MadLevelConfiguration conf;

    public static void Display(MadLevelConfiguration conf) {
        var window = GetWindow<MadLevelGroupsEditTool>(false, "Groups", true);
        window.conf = conf;
    }

    void OnGUI() {
        var arrayList = new MadGUI.ArrayList<MadLevelConfiguration.Group>(conf.groups, @group => {
            if (MadGUI.Button(@group.name)) {
                var builder = new MadInputDialog.Builder("Rename group " + @group.name, "New name for group " + @group.name,
                    newName => TryRename(@group, newName));
                builder.BuildAndShow();
            }
            return @group;
        });
        arrayList.beforeRemove += @group => {
            if (
                !EditorUtility.DisplayDialog("Remove Group",
                    "Are you sure that you want to remove group " + @group.name + "?", "Yes", "No")) {
                return false;
            }

            if (group.GetLevels().Count > 0) {
                MadUndo.RecordObject2(conf, "Remove Group");

                if (EditorUtility.DisplayDialog("Remove Levels As Well?",
                    "Do you want to remove all levels in this group as well? "
                    + "If no, all levels will be moved to default group.", "Yes", "No")) {
                    var levels = group.GetLevels();
                    conf.levels.RemoveAll((level) => levels.Contains(level));
                } else {
                    var defaultGroup = conf.defaultGroup;
                    var levels = group.GetLevels();
                    foreach (var level in levels) {
                        level.groupId = defaultGroup.id;
                    }
                }
            }

            return true;
        };

        arrayList.beforeAdd = () => MadUndo.RecordObject2(conf, "Add Group");

        arrayList.createFunctionGeneric = CreateGroup;

        if (arrayList.Draw()) {
            EditorUtility.SetDirty(conf);
        }
    }

    private void TryRename(MadLevelConfiguration.Group @group, string newName) {
        if (conf.FindGroupByName(newName) == null) {
            @group.name = newName;
            EditorUtility.SetDirty(conf);
        } else {
            EditorUtility.DisplayDialog("Group Exists", "Group '" + newName + "' already exists.", "OK");
        }

        Repaint();

    }

    private MadLevelConfiguration.Group CreateGroup() {
        string name = FindNewGroupName();
        var @group = conf.CreateGroup();
        @group.name = name;

        return @group;
    }

    private string FindNewGroupName() {
        int i = 1;
        string name;
        do {
            name = "New Group " + i++;
        } while (conf.FindGroupByName(name) != null);

        return name;
    }
}

#if !UNITY_3_5
} // namespace
#endif