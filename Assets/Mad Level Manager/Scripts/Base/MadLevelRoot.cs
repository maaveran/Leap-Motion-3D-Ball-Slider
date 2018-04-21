/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using MadLevelManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadLevelRoot : MadNode {

    public const int CameraNearClip = -10;
    public const int CameraFarClip = 5;
    private const int CurrentVersion = 230;

    [SerializeField]
    private int version = 0;

    void Update() {
        if (version < CurrentVersion) {
            Upgrade();
        }
    }

    private void Upgrade() {
        var cam = MadTransform.FindChild<Camera>(transform, (obj) => obj.name == "Camera 2D");
        if (cam != null) {
            cam.nearClipPlane = CameraNearClip;
            cam.farClipPlane = CameraFarClip;
            Debug.Log("Camera 2D clip planes has been updated to recommended values. Please save the scene afterwards.", cam);

#if UNITY_EDITOR
            EditorUtility.SetDirty(cam);
#endif
        }

        SetCurrentVersion();
    }

    public void SetCurrentVersion() {
        version = CurrentVersion;
    }
}

#if !UNITY_3_5
} // namespace
#endif