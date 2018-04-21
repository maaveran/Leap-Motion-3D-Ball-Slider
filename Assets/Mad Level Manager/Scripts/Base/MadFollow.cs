/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadFollow : MonoBehaviour {

    #region Public Fields

    public Transform followTransform;

    #endregion

    #region Slots

    void Update() {
        if (followTransform != null) {
            transform.position = followTransform.position;
        }
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif