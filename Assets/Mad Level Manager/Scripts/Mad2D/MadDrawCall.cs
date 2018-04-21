/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MadDrawCall : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    public Mesh mesh;

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        var meshFilter = transform.GetComponent<MeshFilter>();
        if (mesh == null) {
            mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            meshFilter.mesh = mesh;
        }
#if !UNITY_3_5
        mesh.MarkDynamic();
#endif

#if UNITY_4_2
        if (Application.unityVersion.StartsWith("4.2.0")) {
            Debug.LogError("Unity 4.2 comes with terrible bug that breaks down Mad Level Manager rendering process. "
                + "Please upgrade/downgrade to different version. http://forum.unity3d.com/threads/192467-Unity-4-2-submesh-draw-order");
            }
#endif
    }

    void Update() {
    }

    void OnDestroy() {
        if (Application.isEditor) {
            DestroyImmediate(mesh);
        } else {
            Destroy(mesh);
        }
    }

    public void SetMaterial(Material material) {
        var rend = GetComponent<Renderer>();

        if (GetComponent<Renderer>().sharedMaterials.Length != 1) {
            rend.sharedMaterials = new[] {material};
        } else {
            rend.sharedMaterial = material;
        }
    }

    public void SetMaterials(Material[] materials) {
        var shared = GetComponent<Renderer>().sharedMaterials;

        if (shared.Length != materials.Length) {
            GetComponent<Renderer>().sharedMaterials = materials;
            return;
        }

        for (int i = 0; i < shared.Length; ++i) {
            var s = shared[i];
            var m = materials[i];

            if (s != m) {
                GetComponent<Renderer>().sharedMaterials = materials;
                return;
            }
        }
    }

    public void Destroy() {
        MadGameObject.SetActive(gameObject, false);

#if UNITY_EDITOR
        EditorApplication.delayCall += () => {
            DestroyImmediate(gameObject);
        };
#else
        MadGameObject.SafeDestroy(gameObject);
#endif
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    public static MadDrawCall Create() {
#if UNITY_EDITOR
        GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_draw_call",
 #if MAD_DEBUG
            HideFlags.DontSave,
 #else
            HideFlags.HideAndDontSave,
 #endif
            typeof(MadDrawCall));
#else
        GameObject go = new GameObject("_draw_call");
        go.hideFlags = 
 #if MAD_DEBUG
            HideFlags.DontSave;
 #else
            HideFlags.HideAndDontSave;
#endif

        go.AddComponent<MadDrawCall>();
#endif
        return go.GetComponent<MadDrawCall>();
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif