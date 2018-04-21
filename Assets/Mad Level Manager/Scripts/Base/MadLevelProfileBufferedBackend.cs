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

namespace MadLevelManager {

public abstract class MadLevelProfileBufferedBackend : IMadLevelProfileBackend {

    #region Private Fields

    private Dictionary<string, string> profileValues = new Dictionary<string, string>();

    private bool started;

    #endregion

    #region Public Properties

    public float maxTimePause {
        get { return _maxTimePause; }
        set { _maxTimePause = value; }
    }

    private float _maxTimePause = 16;
    protected MadLevelProfileWatcher profileWatcher;

    #endregion

    #region Methods

    protected MadLevelProfileBufferedBackend() {
        // create a object to keep the coroutine working
        if (Application.isPlaying) {
            profileWatcher = MadTransform.GetOrCreateChild<MadLevelProfileWatcher>(null, "_MLM_ProfileWatcher");
            profileWatcher.Watch(this);
        }
    }

    public IEnumerator Run() {
        started = true;

        while (true) {
            yield return new WaitForSeconds(maxTimePause);

            if (profileValues.Count > 0) {
                Flush();
            }
        }
    }

    public abstract void Start();

    public abstract string LoadProfile(string profileName);

    public void SaveProfile(string profileName, string value) {
        profileValues[profileName] = value;

        if (!started) {
            Flush();
        }
    }

    public void Flush() {
        foreach (var key in profileValues.Keys) {
            Flush(key, profileValues[key]);
        }

        profileValues.Clear();
    }

    public abstract bool CanWorkInEditMode();

    protected abstract void Flush(string profileName, string value);

    #endregion

}

} // namespace