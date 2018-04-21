/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MadLevelManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelProfileWatcher : MonoBehaviour {

    #region Fields

    private bool alreadyWatching;
    private MadLevelProfileBufferedBackend bufferedBackend;

    #endregion

    #region Slots

    void OnEnable() {
        //gameObject.hideFlags = HideFlags.HideInHierarchy;
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause() {
        if (bufferedBackend != null) {
            bufferedBackend.Flush();
        }
    }

    void OnApplicationQuit() {
        if (bufferedBackend != null) {
            bufferedBackend.Flush();
        }
    }

    #endregion

    #region Public Methods

    public void Watch(MadLevelProfileBufferedBackend bufferedBackend) {
        this.bufferedBackend = bufferedBackend;
        if (alreadyWatching) {
            Debug.LogWarning("You're creating more than one BufferedBackend for this project. " +
                             "Please make sure that you will do that only once.");
        }

        StartCoroutine(bufferedBackend.Run());
        alreadyWatching = true;
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif