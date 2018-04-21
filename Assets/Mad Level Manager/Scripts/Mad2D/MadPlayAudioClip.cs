/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadPlayAudioClip : MonoBehaviour {

    #region Public Fields

    public EventType eventType;

    public AudioClip audioClip;

    public float volume = 1;

    #endregion

    #region Public Properties
    #endregion

    #region Slots

    void OnEnable() {
        var sprite = GetComponent<MadSprite>();
        if (sprite == null) {
            Debug.LogError("This component requires MadSprite!");
            return;
        }

        switch (eventType) {
            case EventType.OnMouseEnter:
                sprite.onMouseEnter += Invoke;
                break;
            case EventType.OnMouseExit:
                sprite.onMouseExit += Invoke;
                break;
            case EventType.OnMouseDown:
                sprite.onMouseDown += Invoke;
                break;
            case EventType.OnMouseUp:
                sprite.onMouseUp += Invoke;
                break;
            case EventType.OnTouchEnter:
                sprite.onTouchEnter += Invoke;
                break;
            case EventType.OnTouchExit:
                sprite.onTouchExit += Invoke;
                break;
            case EventType.OnFocus:
                sprite.onFocus += Invoke;
                break;
            case EventType.OnFocusLost:
                sprite.onFocusLost += Invoke;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Invoke(MadSprite sprite) {
        var cam = Camera.main;
        if (cam == null) {
            cam = FindObjectOfType(typeof (Camera)) as Camera;
        }

        AudioSource.PlayClipAtPoint(audioClip, cam.transform.position, volume);
    }

    #endregion

    #region Public Static Methods
    #endregion

    #region Inner and Anonymous Classes

    public enum EventType  {
        OnMouseEnter,
        OnMouseExit,
        OnMouseDown,
        OnMouseUp,
        OnTouchEnter,
        OnTouchExit,
        OnFocus,
        OnFocusLost,
    }

    #endregion
}

#if !UNITY_3_5
} // namespace
#endif