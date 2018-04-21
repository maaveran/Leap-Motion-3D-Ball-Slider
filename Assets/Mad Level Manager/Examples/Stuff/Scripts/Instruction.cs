/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/
using UnityEngine;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class Instruction : MonoBehaviour {

    #region Public Fields

    public string text;

    #endregion

    #region Public Properties
    #endregion

    #region Slots

    void OnGUI() {
        GUILayout.Box(text);
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