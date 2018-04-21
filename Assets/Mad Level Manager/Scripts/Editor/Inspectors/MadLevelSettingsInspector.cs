/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using MadLevelManager;
using MadLevelManager.Backend;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(MadLevelSettings))]
public class MadLevelSettingsInspector : Editor {

    #region Fields

    private List<string> backendList = new List<string>();
    private List<string> backendDisplayedNameList = new List<string>();
    private int chosenBackend;

    private MadLevelSettings settings;

    #endregion

    #region Public Properties
    #endregion

    #region Methods

    void OnEnable() {
        RefreshBackendList();
        settings = (MadLevelSettings) target;

        chosenBackend = BackendToIndex(settings.profileBackend);

        if (chosenBackend == -1) {
            Debug.LogWarning("Cannot found backend " + settings.profileBackend + ", switching to default.");
            settings.profileBackend = typeof(MadLevelProfile.DefaultBackend).ToString();
            chosenBackend = BackendToIndex(settings.profileBackend);
            EditorUtility.SetDirty(settings);
        }
    }

    private int BackendToIndex(string backend) {
        return backendList.FindIndex((s) => s == backend);
    }

    private string IndexToBackend(int index) {
        return backendList[index];
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();
        chosenBackend = EditorGUILayout.Popup(
            "Load & Save Backend", chosenBackend, backendDisplayedNameList.ToArray());
        if (EditorGUI.EndChangeCheck()) {
            settings.profileBackend = IndexToBackend(chosenBackend);
            EditorUtility.SetDirty(settings);
        }

        if (backendList.Count <= chosenBackend) {
            return;
        }

        string backend = backendList[chosenBackend];
        DrawBackendProperties(backend);

        EditorGUILayout.Space();
        DrawBackendHelpIfAvailable(backend);

        if (backendList[chosenBackend] == (typeof (MadLevelProfile.DefaultBackend).FullName)) {
            MadGUI.Info("To get access to more Load & Save backends, unpack packages from the \"Mad Level Manager/Save Load Backends\" directory.");
        }
    }

    private void DrawBackendHelpIfAvailable(string backend) {
        var backendType = Type.GetType(backend + ", Assembly-CSharp");
        object[] customAttributes = backendType.GetCustomAttributes(typeof(HelpURL), false);
        if (customAttributes.Length >= 1) {
            HelpURL helpUrl = (HelpURL) customAttributes[0];
            if (MadGUI.InfoFix("Need help?", "Open Documentation Page")) {
                Help.BrowseURL(helpUrl.url);
            }
        }
    }

    private void DrawBackendProperties(string backend) {
        var backendType = Type.GetType(backend + ", Assembly-CSharp");

        var requiredFields = GetFieldsWithAttribute(backendType, typeof(Required));
        DrawFields("Required", requiredFields);

        var optionalFields = GetFieldsWithAttribute(backendType, typeof(Optional));
        DrawFields("Optional", optionalFields);
    }

    private List<FieldInfo> GetFieldsWithAttribute(Type type, Type attribute) {
        var result = new List<FieldInfo>();

        var fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++) {
            var field = fields[i];
            if (field.GetCustomAttributes(attribute, false).Length > 0) {
                result.Add(field);
            }
        }

        return result;
    }

    private void DrawFields(string label, List<FieldInfo> fields) {
        if (fields.Count == 0) {
            return;
        }

        GUILayout.Label(label, "HeaderLabel");
        using (MadGUI.Indent()) {
            foreach (var field in fields) {
                DrawField(field);
            }
        }
    }

    private void DrawField(FieldInfo requiredField) {
        var fieldName = requiredField.Name;

        string propertyValue;
        bool found = settings.FindPropertyValue(fieldName, out propertyValue);
        if (!found) {
            propertyValue = "";
        }

        var fieldType = requiredField.FieldType;
        if (fieldType == typeof (string)) {
            EditorGUI.BeginChangeCheck();
            var val = EditorGUILayout.TextField(FormatFieldName(fieldName), propertyValue);
            if (EditorGUI.EndChangeCheck()) {
                settings.SetPropertyValue(fieldName, val);
                EditorUtility.SetDirty(settings);
            }
        } else if (fieldType == typeof (int)) {
            EditorGUI.BeginChangeCheck();
            int val;
            int.TryParse(propertyValue, out val);

            val = EditorGUILayout.IntField(FormatFieldName(fieldName), val);
            if (EditorGUI.EndChangeCheck()) {
                settings.SetPropertyValue(fieldName, val.ToString());
                EditorUtility.SetDirty(settings);
            }
        } else if (fieldType == typeof (float)) {
            EditorGUI.BeginChangeCheck();
            float val;
            float.TryParse(propertyValue, out val);

            val = EditorGUILayout.FloatField(FormatFieldName(fieldName), val);
            if (EditorGUI.EndChangeCheck()) {
                settings.SetPropertyValue(fieldName, val.ToString());
                EditorUtility.SetDirty(settings);
            }
        } else if (fieldType == typeof (bool)) {
            EditorGUI.BeginChangeCheck();
            bool val;
            bool.TryParse(propertyValue, out val);

            val = EditorGUILayout.Toggle(FormatFieldName(fieldName), val);
            if (EditorGUI.EndChangeCheck()) {
                settings.SetPropertyValue(fieldName, val.ToString());
                EditorUtility.SetDirty(settings);
            }
        } else if (typeof(Enum).IsAssignableFrom(fieldType)) {
            EditorGUI.BeginChangeCheck();

            Enum val;
            try {
                val = (Enum) Enum.Parse(fieldType, propertyValue);
            } catch (Exception) {
                val = (Enum) Enum.Parse(fieldType, "0");
            }

            val = EditorGUILayout.EnumPopup(FormatFieldName(fieldName), val);

            if (EditorGUI.EndChangeCheck()) {
                settings.SetPropertyValue(fieldName, val.ToString());
                EditorUtility.SetDirty(settings);
            }

        } else {
            Debug.LogError("Unsupported backend property type: " + fieldType);
        }
    }

    private string FormatFieldName(string fieldName) {
        var sb = new StringBuilder();

        for (int i = 0; i < fieldName.Length; ++i) {
            char c = fieldName[i];
            if (i == 0) {
                c = char.ToUpper(c);
            } else if (char.IsUpper(c)) {
                sb.Append(" ");
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private void RefreshBackendList() {
        var assembly = typeof(MadLevelSettings).Assembly;

        var types = assembly.GetTypes();

        var @interface = typeof(IMadLevelProfileBackend);

        List<Type> backendTypes = new List<Type>();

        foreach (var type in types) {
            if (type.IsAbstract || type.IsInterface) {
                continue;
            }

            if (@interface.IsAssignableFrom(type)) {
                backendTypes.Add(type);
                
            }
        }

        // using toString() instead of FullName because of _bug in Mono?
        backendTypes.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

        foreach (var type in backendTypes) {
            backendList.Add(type.ToString());

            object[] displayedName = type.GetCustomAttributes(typeof(DisplayedName), false);
            if (displayedName.Length >= 1) {
                DisplayedName dn = (DisplayedName)displayedName[0];
                backendDisplayedNameList.Add(dn.name);
            } else {
                backendDisplayedNameList.Add(type.Name);
            }
        }
    }

    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

#if !UNITY_3_5
} // namespace
#endif