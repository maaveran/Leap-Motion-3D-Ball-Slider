#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;


/// <summary>
/// This script adds the UI menu options to the Unity Editor.
/// </summary>
	
namespace TextFx
{
	static internal class MenuOptionsTextFxUGUI
	{
		private const string kUILayerName = "UI";
		private const float  kWidth       = 160f;
		private const float  kThickHeight = 30f;
		
		private static Vector2 s_ThickGUIElementSize    = new Vector2(kWidth, kThickHeight);
		
		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;
			
			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;
			
			// Create world space Plane from canvas position.
			Vector2 localPlanePosition;
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
				
				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);
				
				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;
				
				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;
				
				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;
				
				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}
			
			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}
		
		private static GameObject CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
		{
			GameObject parent = menuCommand.context as GameObject;
			if (parent == null || FindInParents<Canvas>(parent) == null)
			{
				parent = GetParentActiveCanvasInSelection(true);
			}
			GameObject child = new GameObject(name);
			
			Undo.RegisterCreatedObjectUndo(child, "Create " + name);
			Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
			GameObjectUtility.SetParentAndAlign(child, parent);
			
			RectTransform rectTransform = child.AddComponent<RectTransform>();
			rectTransform.sizeDelta = size;
			if (parent != menuCommand.context) // not a context click, so center in sceneview
			{
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), rectTransform);
			}
			Selection.activeGameObject = child;
			return child;
		}

		
		[MenuItem("GameObject/UI/TextFx Text", false, 2002)]
		static public void AddText(MenuCommand menuCommand)
		{
			GameObject go = CreateUIElementRoot("Text", menuCommand, s_ThickGUIElementSize);
			
			TextFxUGUI lbl = go.AddComponent<TextFxUGUI>();
			lbl.text = "New Text";
			SetDefaultTextValues(lbl);
		}
		
		private static void SetDefaultTextValues(Text lbl)
		{
			lbl.color = new Color(0.1953125f, 0.1953125f, 0.1953125f, 1f);
		}
		
		static public GameObject CreateNewUI()
		{
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
			
			// if there is no event system add one...
			CreateEventSystem(false);
			return root;
		}
		
		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}
		
		private static void CreateEventSystem(bool select, GameObject parent)
		{
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null)
			{
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();
				eventSystem.AddComponent<TouchInputModule>();
				
				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}
			
			if (select && esys != null)
			{
				Selection.activeGameObject = esys.gameObject;
			}
		}
		
		static public T FindInParents<T>(GameObject go) where T : Component
		{
			if (go == null)
				return null;
			
			T comp = null;
			Transform t = go.transform;
			while (t != null && comp == null)
			{
				comp = t.GetComponent<T>();
				t = t.parent;
			}
			return comp;
		}
		
		// Helper function that returns the selected root object.
		static public GameObject GetParentActiveCanvasInSelection(bool createIfMissing)
		{
			GameObject go = Selection.activeGameObject;
			
			// Try to find a gameobject that is the selected GO or one if ots parents
			Canvas p = (go != null) ? FindInParents<Canvas>(go) : null;
			// Only use active objects
			if (p != null && p.gameObject.activeInHierarchy)
				go = p.gameObject;
			
			// No canvas in selection or its parents? Then use just any canvas.
			if (go == null)
			{
				Canvas canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
				if (canvas != null)
					go = canvas.gameObject;
			}
			
			// No canvas present? Create a new one.
			if (createIfMissing && go == null)
				go = MenuOptionsTextFxUGUI.CreateNewUI();
			
			return go;
		}

	}
}
#endif