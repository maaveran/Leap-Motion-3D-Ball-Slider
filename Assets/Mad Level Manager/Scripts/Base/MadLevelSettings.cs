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
using MadLevelManager.Backend;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelSettings : ScriptableObject {

    public static MadLevelSettings current {
        get {
            if (_current == null) {
                _current = (MadLevelSettings) Resources.Load("MLM_Settings", typeof(MadLevelSettings));
            }

            return _current;
        }
    }

    private static MadLevelSettings _current;

    public string profileBackend = typeof(MadLevelProfile.DefaultBackend).ToString();

    public List<Property> profileBackendProperties = new List<Property>();

    public IMadLevelProfileBackend CreateBackend() {
        if (string.IsNullOrEmpty(profileBackend)) {
            return new MadLevelProfile.DefaultBackend();
        }

        var backendType = Type.GetType(profileBackend);
        if (backendType == null) {
            Debug.LogWarning("Cannot find backend " + profileBackend + ", using default.");
            return new MadLevelProfile.DefaultBackend();
        }

        try {
            if (!typeof (IMadLevelProfileBackend).IsAssignableFrom(backendType)) {
                throw new Exception("Not a instance of IMadLevelProfileBackend");
            }

            var instance = Activator.CreateInstance(backendType);

            ConfigureProperties(instance);

            var backend = (IMadLevelProfileBackend) instance;
            backend.Start();

            return backend;
        } catch (Exception e) {
            Debug.LogError("Cannot create instance of " + profileBackend + ": " + e);
            return new MadLevelProfile.DefaultBackend();
        }
    }

    private void ConfigureProperties(object instance) {
        var requiredFields = GetRequiredFields(instance.GetType());
        if (!AssignFields(instance, requiredFields)) {
            Debug.LogError("Do not have all required properties! Please adjust correct MLM_Settings.", this);
        }

        var optionalFields = GetOptionalFields(instance.GetType());
        AssignFields(instance, optionalFields);
    }

    private bool AssignFields(object instance, List<FieldInfo> fields) {
        bool gotAll = true;
        
        for (int i = 0; i < fields.Count; i++) {
            var field = fields[i];
            var fieldName = field.Name;
            string value;
            if (FindPropertyValue(fieldName, out value)) {
                SetValue(field, instance, value);
            } else {
                gotAll = false;
            }
        }

        return gotAll;
    }

    private void SetValue(FieldInfo field, object instance, string value) {
        var type = field.FieldType;
        if (type == typeof (string)) {
            field.SetValue(instance, value);
        } else if (type == typeof (int)) {
            int val;
            int.TryParse(value, out val);
            field.SetValue(instance, val);
        } else if (type == typeof (float)) {
            float val;
            float.TryParse(value, out val);
            field.SetValue(instance, val);
        } else if (type == typeof (bool)) {
            bool val;
            bool.TryParse(value, out val);
            field.SetValue(instance, val);
        } else if (typeof (Enum).IsAssignableFrom(type)) {
            Enum val;
            if (!string.IsNullOrEmpty(value)) {
                val = (Enum) Enum.Parse(type, value);
            } else {
                val = (Enum) Enum.Parse(type, "0");
            }

            field.SetValue(type, val);
        } else {
            Debug.LogError("Unsupported type: " + type);
        }
    }

    private List<FieldInfo> GetRequiredFields(Type type) {
        return GetFieldsWithAttribute(type, typeof(Required));
    }

    private List<FieldInfo> GetOptionalFields(Type type) {
        return GetFieldsWithAttribute(type, typeof(Optional));
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

    private object CreateInstance(ConstructorInfo constructor) {
        object[] parameters;
        if (CreateParameters(constructor, out parameters)) {
            return constructor.Invoke(null, parameters);
        } else {
            throw new Exception("Cannot create instance. Check assigned parameters.");
        }
    }

    private bool CreateParameters(ConstructorInfo constructor, out object[] outResult) {
        var parameterInfos = constructor.GetParameters();
        object[] result = new object[parameterInfos.Length];

        for (int i = 0; i < parameterInfos.Length; i++) {
            var parameterInfo = parameterInfos[i];
            var name = parameterInfo.Name;
            string argumentValue;
            if (FindPropertyValue(name, out argumentValue)) {
                result[i] = argumentValue;
            } else {
                outResult = null;
                return false;
            }
        }

        outResult = result;
        return true;
    }

    public bool FindPropertyValue(string name, out string outValue) {
        var property = FindProperty(name);
        if (property != null) {
            outValue = property.value;
            return true;
        } else {
            outValue = null;
            return false;
        }
    }

    private Property FindProperty(string name) {
        for (int i = 0; i < profileBackendProperties.Count; i++) {
            var property = profileBackendProperties[i];
            if (property.name == name) {
                return property;
            }
        }

        return null;
    }

    public void SetPropertyValue(string name, string @value) {
        var property = FindProperty(name);
        if (property == null) {
            property = new Property();
            property.name = name;
            profileBackendProperties.Add(property);
        }

        property.value = @value;
    }

    public static ConstructorInfo FindConstructor(Type backendType) {
        var constructors = backendType.GetConstructors();
        foreach (var constructorInfo in constructors) {
            return constructorInfo;
        }

        return null;
    }

    private static void VerifyConstructor(ConstructorInfo constructorInfo) {
        var parameterInfos = constructorInfo.GetParameters();
        foreach (var parameterInfo in parameterInfos) {
            if (parameterInfo.ParameterType != typeof (string)) {
                throw new Exception("Invalid constructor, should only have strings as parameters");
            }
        }
    }

    [Serializable]
    public class Property {
        public string name;
        public string @value;
    }

}

#if !UNITY_3_5
} // namespace
#endif