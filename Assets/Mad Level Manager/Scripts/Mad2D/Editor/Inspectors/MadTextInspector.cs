/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CanEditMultipleObjects]
[CustomEditor(typeof(MadText))]
public class MadTextInspector : MadSpriteInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    SerializedProperty panel;
    SerializedProperty font;
    SerializedProperty atlas;
    SerializedProperty text;
    SerializedProperty scale;
    SerializedProperty letterSpacing;
    SerializedProperty align;
    SerializedProperty wordWrap;
    SerializedProperty wordWrapLength;

    private MadText madText;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();

        panel = serializedObject.FindProperty("panel");
        font = serializedObject.FindProperty("font");
        atlas = serializedObject.FindProperty("atlas");
        text = serializedObject.FindProperty("text");
        scale = serializedObject.FindProperty("scale");
        letterSpacing = serializedObject.FindProperty("letterSpacing");
        align = serializedObject.FindProperty("align");
        wordWrap = serializedObject.FindProperty("wordWrap");
        wordWrapLength = serializedObject.FindProperty("wordWrapLength");

        showLiveBounds = false;

        madText = (MadText) target;
        UpdateTextureGUID();
    }

    public void UpdateTextureGUID() {
        if (madText.font != null) {
            var texturePath = AssetDatabase.GetAssetPath(madText.font.texture);
            var guid = AssetDatabase.AssetPathToGUID(texturePath);
            if (guid != madText.fontTextureGUID) {
                madText.fontTextureGUID = guid;
                EditorUtility.SetDirty(this);
            }
        } else if (madText.fontTextureGUID != null) {
            madText.fontTextureGUID = null;
            EditorUtility.SetDirty(this);
        }
    }

    public override void OnInspectorGUI() {
        SectionSprite(DisplayFlag.WithoutSize | DisplayFlag.WithoutMaterial | DisplayFlag.WithoutFill);
        
        serializedObject.Update();
        MadGUI.PropertyField(panel, "Panel", MadGUI.ObjectIsSet);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        MadGUI.PropertyField(font, "Font", MadGUI.ObjectIsSet);
        if (EditorGUI.EndChangeCheck()) {
            UpdateTextureGUID();
        }

        MadGUI.PropertyField(atlas, "Atlas");

        if (madText.atlas != null && madText.font != null) {
            var texture = madText.font.texture;
            var texturePath = AssetDatabase.GetAssetPath(texture);
            var textureGuid = AssetDatabase.AssetPathToGUID(texturePath);
            if (madText.atlas.GetItem(textureGuid) == null) {
                if (MadGUI.WarningFix("This font texture is not available in selected atlas.", "Add to atlas")) {
                    MadAtlasBuilder.AddToAtlas(madText.atlas, madText.font.texture);
                }
            }
        }

        EditorGUILayout.LabelField("Text");
        if (text.hasMultipleDifferentValues) {
            EditorGUILayout.TextArea("-");
        } else {
            text.stringValue = EditorGUILayout.TextArea(text.stringValue);
        }
        MadGUI.PropertyField(scale, "Scale");
        MadGUI.PropertyField(align, "Align");
        MadGUI.PropertyField(letterSpacing, "Letter Spacing");
        MadGUI.PropertyField(wordWrap, "Word Wrap");
        MadGUI.Indent(() => { 
            MadGUI.PropertyField(wordWrapLength, "Line Length");
        });
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif