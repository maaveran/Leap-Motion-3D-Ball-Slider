#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace TextFx
{

	[AddComponentMenu("UI/TextFx Text", 12)]
	public class TextFxUGUI : UnityEngine.UI.Text , TextFxAnimationInterface
	{
		// Interface class for accessing the required text vertex data in the required format
		public class UGUITextDataHandler : TextFxAnimationManager.GuiTextDataHandler
		{
			List<UIVertex> m_vertData;
			
			public int NumVerts { get { return m_vertData.Count; } }
			
			public UGUITextDataHandler(List<UIVertex> vertData)
			{
				m_vertData = vertData;
			}
			
			public Vector3 GetVertPosition(int index)
			{
				return m_vertData [index].position;
			}
			
			public Color GetVertColour(int index)
			{
				return m_vertData [index].color;
			}
		}




		// Editor TextFx conversion menu options
#if UNITY_EDITOR
		[MenuItem ("Tools/TextFx/Convert UGUI Text to TextFx")]
		static void ConvertToTextFX ()
		{
			GameObject activeGO = Selection.activeGameObject;
			UnityEngine.UI.Text uguiText = activeGO.GetComponent<UnityEngine.UI.Text> ();
			TextFxUGUI textfxUGUI = activeGO.GetComponent<TextFxUGUI> ();

			if(textfxUGUI != null)
				return;

			GameObject tempObject = new GameObject("temp");
			textfxUGUI = tempObject.AddComponent<TextFxUGUI>();

			TextFxUGUI.CopyComponent(uguiText, textfxUGUI);

			DestroyImmediate (uguiText);

			TextFxUGUI newUGUIEffect = activeGO.AddComponent<TextFxUGUI> ();

			TextFxUGUI.CopyComponent (textfxUGUI, newUGUIEffect);

			DestroyImmediate (tempObject);

			Debug.Log (activeGO.name + "'s Text component converted into a TextFxUGUI component");
		}
		
		[MenuItem ("Tools/TextFx/Convert UGUI Text to TextFx", true)]
		static bool ValidateConvertToTextFX ()
		{
			if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<UnityEngine.UI.Text>() != null)
				return true;
			else
				return false;
		}


		static void CopyComponent(UnityEngine.UI.Text textFrom, UnityEngine.UI.Text textTo)
		{
			textTo.text = textFrom.text;
			textTo.font = textFrom.font;
			textTo.fontSize = textFrom.fontSize;
			textTo.fontStyle = textFrom.fontStyle;
			textTo.lineSpacing = textFrom.lineSpacing;
			textTo.supportRichText = textFrom.supportRichText;
			textTo.alignment = textFrom.alignment;
			textTo.resizeTextForBestFit = textFrom.resizeTextForBestFit;
			textTo.color = textFrom.color;
			textTo.material = textFrom.material;
			textTo.enabled = textFrom.enabled;
		}
#endif


		public string AssetNameSuffix { get { return "_UGUI"; } }
		public float MovementScale { get { return 26f; } }
		public int LayerOverride { get { return 5; } }			// Renders objects on the UI layer

		[HideInInspector, SerializeField]
		TextFxAnimationManager m_animation_manager;
		public TextFxAnimationManager AnimationManager { get { return m_animation_manager; } }

		[HideInInspector, SerializeField]
		GameObject m_gameobject_reference;
		public GameObject GameObject { get { if( m_gameobject_reference == null) m_gameobject_reference = gameObject; return m_gameobject_reference; } }

		[HideInInspector, SerializeField]
		string m_cachedText = string.Empty;
		[HideInInspector, SerializeField]
		TextGenerationSettings m_cachedTextSettings;
		[HideInInspector, SerializeField]
		List<UIVertex> m_cachedVerts;

		public string Text { get { return m_cachedText; } }

		[HideInInspector, SerializeField]
		Vector3[] m_forced_state_verts;		// Verts stored for a one time rendering call
		[HideInInspector, SerializeField]
		Color[] m_forced_state_cols;		// Verts stored for a one time rendering call

		protected override void OnEnable()
		{
			base.OnEnable ();

			if (m_animation_manager == null)
				m_animation_manager = new TextFxAnimationManager (new int[]{1,0,3,2});
			
			m_animation_manager.SetParentObjectReferences (gameObject, transform, this);

			if(!Application.isPlaying)
				// Call to update mesh rendering
				UpdateGeometry ();
		}

		protected override void Start()
		{
			if(!Application.isPlaying)
			{
				return;
			}

			m_animation_manager.OnStart ();
		}

		void Update()
		{
			if(!Application.isPlaying || !m_animation_manager.Playing)
			{
				return;
			} 

			if (m_animation_manager.UpdateAnimation () == false)
			{
				// Animation has ended. Populate forced_state_verts with end state
				SetForcedStateVerts (m_animation_manager.MeshVerts, m_animation_manager.MeshColours);
			}

			// Call to update mesh rendering
			UpdateGeometry ();
		}

		// Interface Method: To redraw the mesh with the provided mesh vertex positions
		public void UpdateTextFxMesh(Vector3[] verts, Color[] cols)
		{
			SetForcedStateVerts (verts, cols);

			// Call to update mesh rendering
			UpdateGeometry ();
		}

		// Interface Method: To set the text of the text renderer
		public void SetText(string new_text)
		{
			text = new_text;

			// Reset the text to avoid errors during animation
			AnimationManager.ResetAnimation ();
			UpdateTextFxMesh (AnimationManager.MeshVerts, AnimationManager.MeshColours);
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry ();
		}

		void SetForcedStateVerts(Vector3[] verts, Color[] cols)
		{
			if(m_forced_state_verts == null || m_forced_state_verts.Length != verts.Length)
				m_forced_state_verts = new Vector3[verts.Length];
			verts.CopyTo(m_forced_state_verts, 0);

			if(m_forced_state_cols == null || m_forced_state_cols.Length != cols.Length)
				m_forced_state_cols = new Color[cols.Length];
			cols.CopyTo(m_forced_state_cols, 0);
		}


		/// <summary>
		/// Draw the Text.
		/// </summary>
		
		protected override void OnFillVBO(List<UIVertex> vbo)
		{
			if (font == null)
				return;

			Vector2 extents = rectTransform.rect.size;
			TextGenerationSettings settings = GetGenerationSettings(extents); 

			if (!m_cachedTextSettings.Equals( settings ) || m_cachedText != text)
			{
				base.OnFillVBO (vbo);

//				Debug.Log("TextSetting changed, updating verts! vbo.Count : " + vbo.Count); 

				// Update caches
				m_cachedVerts = vbo.GetRange(0, vbo.Count);
				m_cachedTextSettings = settings;
				m_cachedText = text;

				// Call to update animation letter setups
				m_animation_manager.UpdateText (text, new UGUITextDataHandler(m_cachedVerts), white_space_meshes: true);
			}
			else
			{
				// Use cached vert data
//				Debug.Log("Use cached vert data, m_cachedVerts[" + m_cachedVerts.Count + "] ");

				UIVertex new_vert;

				// Add each cached vert into the VBO buffer. Verts seem to need to be added one by one using Add(), can't just copy the list over
				for(int idx=0; idx < m_cachedVerts.Count; idx++)
				{
					vbo.Add(m_cachedVerts[idx]);

					if(Application.isPlaying && m_animation_manager.Playing && m_animation_manager.MeshVerts != null && idx < m_animation_manager.MeshVerts.Length)
					{
						new_vert = vbo[vbo.Count - 1];
						new_vert.position = m_animation_manager.MeshVerts[idx];
						new_vert.color = m_animation_manager.MeshColours[idx];
						vbo[vbo.Count - 1] = new_vert;

						m_forced_state_verts[idx] = m_animation_manager.MeshVerts[idx];
						m_forced_state_cols[idx] = m_animation_manager.MeshColours[idx];
					}
					else if(m_forced_state_verts != null && idx < m_forced_state_verts.Length)
					{
	//					Debug.Log("UGUI Using forced_state_verts");
						new_vert = vbo[vbo.Count - 1];
						new_vert.position = m_forced_state_verts[idx];
						new_vert.color = m_forced_state_cols[idx];
						vbo[vbo.Count - 1] = new_vert;
					}
				}

#if UNITY_EDITOR
				if(m_forced_state_verts != null)
				{
					// Set object dirty to trigger sceneview redraw/update. Calling SceneView.RepaintAll() doesn't work for some reason.
					EditorUtility.SetDirty( GameObject );
				}
#endif
			}
		}

	}
}
#endif
