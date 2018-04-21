/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadBigMeshRenderer : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    MadPanel panel;
    
    // helpers for decrasing GC activity
    MadList<Vector3> vertices = new MadList<Vector3>();
    MadList<Color32> colors = new MadList<Color32>();
    MadList<Vector2> uv = new MadList<Vector2>();
    MadList<MadList<int>> triangleList = new MadList<MadList<int>>();

    MadObjectPool<MadList<int>> trianglesPool = new MadObjectPool<MadList<int>>(32);

    [SerializeField]
    List<MadDrawCall> drawCalls = new List<MadDrawCall>(32);

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void OnEnable() {
        panel = GetComponent<MadPanel>();
        
        // enable existing draw calls
        for (int i = 0; i < drawCalls.Count; i++) {
            var drawCall = drawCalls[i];
            if (drawCall == null) {
                drawCalls.RemoveAt(i);
                i--;
            } else {
                MadGameObject.SetActive(drawCall.gameObject, true);
            }
        }
    }

    private int nextDrawCall = 0;

    private MadDrawCall NextDrawCall() {
        MadDrawCall drawCall;

        if (nextDrawCall >= drawCalls.Count) {
            // create new draw call
            drawCall = MadDrawCall.Create();
            drawCall.gameObject.layer = gameObject.layer;

            MadTransform.SetLocalScale(drawCall.transform, transform.lossyScale);
            drawCalls.Add(drawCall);
            nextDrawCall++;
        } else {
            drawCall = drawCalls[nextDrawCall++];
            MadGameObject.SetActive(drawCall.gameObject, true);
        }

        return drawCall;
    }

    private void DrawCallsFinalize() {
        var count = drawCalls.Count;

        // remove unused draw calls
        for (int i = nextDrawCall; i < count; ++i) {
            var lastIndex = drawCalls.Count - 1;
            var drawCall = drawCalls[lastIndex];
            drawCall.Destroy();
            drawCalls.RemoveAt(lastIndex);
        }

        nextDrawCall = 0;
    }

    void OnDisable() {
        for (int i = 0; i < drawCalls.Count; i++) {
            var drawCall = drawCalls[i];
            MadGameObject.SetActive(drawCall.gameObject, false);
        }
    }

    void Start() {
    }

    void Update() {
        for (int i = 0; i < drawCalls.Count; i++) {
            var drawCall = drawCalls[i];

            MadTransform.SetLocalScale(drawCall.transform, transform.lossyScale);

            drawCall.transform.position = transform.position;
            drawCall.transform.rotation = transform.rotation;

            drawCall.gameObject.layer = gameObject.layer;
        }
    }

    void LateUpdate() {
        if (panel == null) {
            panel = GetComponent<MadPanel>();
        }

        var visibleSprites = VisibleSprites(panel.sprites);

        switch (panel.renderMode) {
            case MadPanel.RenderMode.Legacy: {
                SortByGUIDepth(visibleSprites);
                var batchedSprites = Batch(visibleSprites);

                DrawOnSingleDrawCall(batchedSprites);
            }
                break;
            case MadPanel.RenderMode.DepthBased: {
                    SortByZ(visibleSprites);
                    var batchedSprites = Batch(visibleSprites);
            
                    DrawOnMultipleDrawCalls(batchedSprites);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        vertices.Clear();
        colors.Clear();
        uv.Clear();
        triangleList.Clear();
    }

    private void DrawOnMultipleDrawCalls(List<List<MadSprite>> batchedSprites) {
        try {
            for (int i = 0; i < batchedSprites.Count; i++) {
                var sprites = batchedSprites[i];

                var drawCall = NextDrawCall();
                var mesh = drawCall.mesh;
                mesh.Clear();
                mesh.subMeshCount = 1;

                MadList<int> triangles;

                if (!trianglesPool.CanTake()) {
                    trianglesPool.Add(new MadList<int>());
                }

                triangles = trianglesPool.Take();

                for (int j = 0; j < sprites.Count; j++) {
                    var sprite = sprites[j];
                    Material material;
                    sprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out material);
                    if (j == 0) {
                        drawCall.SetMaterial(material);
                    }
                }

                triangles.Trim();
                vertices.Trim();
                colors.Trim();
                uv.Trim();

                mesh.vertices = vertices.Array;
                mesh.colors32 = colors.Array;
                mesh.uv = uv.Array;
                mesh.SetTriangles(triangles.Array, 0);

#if !UNITY_METRO
                mesh.RecalculateNormals();
#endif
                triangles.Clear();
                trianglesPool.Release(triangles);

                vertices.Clear();
                colors.Clear();
                uv.Clear();
                triangleList.Clear();

            }
#if !MAD_EBT && (!UNITY_5 || !UNITY_WEBGL) // http://fogbugz.unity3d.com/default.asp?675175_r3deh2mdhmitddpj
        } finally {
#else
} finally {} {
#endif
            DrawCallsFinalize();

        }
    }

    private void DrawOnSingleDrawCall(List<List<MadSprite>> batchedSprites) {
        Material[] materials = new Material[batchedSprites.Count];

        var drawCall = NextDrawCall();

        try {
            var mesh = drawCall.mesh;
            mesh.Clear();
            mesh.subMeshCount = batchedSprites.Count;

            for (int i = 0; i < batchedSprites.Count; ++i) {
                List<MadSprite> sprites = batchedSprites[i];

                MadList<int> triangles;

                if (!trianglesPool.CanTake()) {
                    trianglesPool.Add(new MadList<int>());
                }

                triangles = trianglesPool.Take();

                for (int j = 0; j < sprites.Count; ++j) {
                    var sprite = sprites[j];
                    Material material;
                    sprite.DrawOn(ref vertices, ref colors, ref uv, ref triangles, out material);
                    materials[i] = material;
                }

                triangles.Trim();
                triangleList.Add(triangles);
            }

            vertices.Trim();
            colors.Trim();
            uv.Trim();
            triangleList.Trim();

            mesh.vertices = vertices.Array;
            mesh.colors32 = colors.Array;
            mesh.uv = uv.Array;

            // excluding for metro, because of a bug
#if !UNITY_METRO
            mesh.RecalculateNormals();
#endif

            for (int i = 0; i < triangleList.Count; ++i) {
                MadList<int> triangles = triangleList[i];
                mesh.SetTriangles(triangles.Array, i);
                triangles.Clear();
                trianglesPool.Release(triangles);
            }

            //renderer.sharedMaterials = materials;
            drawCall.SetMaterials(materials);
#if !MAD_EBT && (!UNITY_5 || !UNITY_WEBGL) // http://fogbugz.unity3d.com/default.asp?675175_r3deh2mdhmitddpj
        } finally {
#else
} finally {} {
#endif
            DrawCallsFinalize();
        }
    }

    void OnDestroy() {
        for (int i = 0; i < drawCalls.Count; i++) {
            var drawCall = drawCalls[i];
            drawCall.Destroy();
        }
        
        drawCalls.Clear();
    }

    List<MadSprite> VisibleSprites(ICollection<MadSprite> sprites) {
        List<MadSprite> output = new List<MadSprite>();
        
        foreach (var sprite in sprites) {
            if (SpriteVisible(sprite)) {
                output.Add(sprite);
            }
        }
        
        return output;
    }

    private Vector3[] cornersWorker = new Vector3[4];

    private bool SpriteVisible(MadSprite sprite) {
        bool active =
#if UNITY_3_5
        sprite.gameObject.active;
#else
        sprite.gameObject.activeInHierarchy;
#endif
        if (!active) {
            return false;
        }

        if (!sprite.visible) {
            return false;
        }

        if (Mathf.Approximately(sprite.tint.a, 0)) {
            return false;
        }

        if (!sprite.CanDraw()) {
            return false;
        }

        if (panel.hideInvisibleSprites && Application.isPlaying) {
            // execute camera visiblity check only if playing (not working in the edit mode)

            sprite.GetWorldCorners(ref cornersWorker);
            var cam = panel.currentCamera;
            if (!VisibleOnCameraAny(cornersWorker, cam)) {
                return false;
            }
        }

        return true;
    }

    private bool VisibleOnCameraAny(Vector3[] corners, Camera cam) {
        var topLeft = cam.WorldToViewportPoint(corners[0]);
        var bottomRight = cam.WorldToViewportPoint(corners[2]);

        float left = Mathf.Min(topLeft.x, bottomRight.x);
        float top = Mathf.Max(topLeft.y, bottomRight.y);
        float right = Mathf.Max(topLeft.x, bottomRight.x);
        float bottom = Mathf.Min(topLeft.y, bottomRight.y);

        if (right < 0 || left > 1 || top < 0 || bottom > 1) {
            return false;
        }

        return true;
    }

    private bool VisibleOnCamera(Vector3 corner, Camera cam) {
        var vp = cam.WorldToViewportPoint(corner);
        return vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1;
    }

    void SortByGUIDepth(List<MadSprite> sprites) {
        sprites.Sort((x, y) => x.guiDepth.CompareTo(y.guiDepth));
    }

    void SortByZ(List<MadSprite> sprites) {
        sprites.Sort((x, y) => x.transform.position.z.CompareTo(y.transform.position.z));
    }
    
    List<List<MadSprite>> Batch(List<MadSprite> sprites) {
        var output = new List<List<MadSprite>>();
        
        int count = sprites.Count;
        List<MadSprite> batched = null;
        for (int i = 0; i < count; ++i) {
            var currentSprite = sprites[i];
            if (batched == null) {
                batched = new List<MadSprite>();
            } else if (!CanBatch(currentSprite, batched[batched.Count - 1])) {
                output.Add(batched);
                batched = new List<MadSprite>();
            }
            
            batched.Add(currentSprite);
        }
        
        if (batched != null) {
            output.Add(batched);
        }
        
        return output;
    }
    
    bool CanBatch(MadSprite a, MadSprite b) {
        if (panel.renderMode == MadPanel.RenderMode.DepthBased) {
            if (a.guiDepth != b.guiDepth) {
                return false;
            }
        }

        var materialA = a.GetMaterial();
        var materialB = b.GetMaterial();
        
        return materialA.Equals(materialB);
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