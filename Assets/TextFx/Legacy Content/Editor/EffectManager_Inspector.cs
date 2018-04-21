using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace TextFx.LegacyContent
{

	[CustomEditor(typeof(EffectManager))]
	public class EffectManager_Inspector : Editor
	{
		bool m_previewing_anim = false;			// Denotes whether the animation is currently being previewed in the editor
		bool m_paused = false;
		EffectManager font_manager;
		float m_old_time = 0;
		
		float m_set_text_delay = 0;
		float m_time_delta = 0;

		int m_selected_animation_idx;

		string m_old_text;
		TextDisplayAxis m_old_display_axis;
		TextAnchor m_old_text_anchor;
		TextAlignment m_old_text_alignment;
		float m_old_char_size;
		float m_old_line_height;
		Vector2 m_old_px_offset;
		bool m_old_baseline_override;
		float m_old_font_baseline;
		float m_old_max_width;
		
	    void OnEnable()
	    {
			EditorApplication.update += UpdateFunction;

			m_selected_animation_idx = EditorPrefs.GetInt("SelectedTextFxAnim", 0);
	    }
		
		void OnDisable()
		{
			EditorApplication.update -= UpdateFunction;
		}
		
		void UpdateFunction()
		{
			m_time_delta = Time.realtimeSinceStartup - m_old_time;
			
			if(m_previewing_anim && !m_paused)
			{
				if(font_manager == null)
				{
					font_manager = (EffectManager)target;
				}
				
				if(m_time_delta > 0 && !font_manager.UpdateAnimation(m_time_delta) && font_manager.ParticleEffectManagers.Count == 0)
				{
					m_previewing_anim = false;
				}
				
				if(font_manager.ParticleEffectManagers.Count > 0)
					SceneView.RepaintAll();
			}
			
			if(m_set_text_delay > 0)
			{
				m_set_text_delay -= m_time_delta;
				
				if(m_set_text_delay <= 0)
				{
					m_set_text_delay = 0;
					
					font_manager.SetText(font_manager.m_text);
				}
			}
			
			m_old_time = Time.realtimeSinceStartup;
		}
		
		public override void OnInspectorGUI ()
		{
	//		DrawDefaultInspector();
			
			font_manager = (EffectManager)target;
			
			m_old_text = font_manager.m_text;
			m_old_display_axis = font_manager.m_display_axis;
			m_old_text_anchor = font_manager.m_text_anchor;
			m_old_text_alignment = font_manager.m_text_alignment;
			m_old_char_size = font_manager.m_character_size;
			m_old_line_height = font_manager.m_line_height_factor;
			m_old_px_offset = font_manager.m_px_offset;
			m_old_baseline_override = font_manager.m_override_font_baseline;
			m_old_font_baseline = font_manager.m_font_baseline_override;
			m_old_max_width = font_manager.m_max_width;
			
			if(GUI.changed)
			{
				return;
			}

			GUILayout.Space(10);

			EditorGUILayout.LabelField("Import Preset Effect", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			m_selected_animation_idx = EditorGUILayout.Popup(m_selected_animation_idx, TextFxAnimationConfigs.m_config_list.GetArrayOfFirstEntries());

			if(GUI.changed)
			{
				EditorPrefs.SetInt("SelectedTextFxAnim", m_selected_animation_idx);
			}

			if(GUILayout.Button("Apply"))// && EditorUtility.DisplayDialog("Import TextFx Animation", "Are you sure you want to import this \"" + 
			                              //                               TextFxAnimationConfigs.m_config_list.GetArrayOfFirstEntries()[m_selected_animation_idx] +
			                              //                               "\" effect?", "Import", "Cancel"))
			{
				if(!TextFxAnimationConfigs.m_config_list[m_selected_animation_idx,1].Equals(""))
				{
					font_manager.ImportData(TextFxAnimationConfigs.m_config_list[m_selected_animation_idx,1]);
					Debug.Log("TextFx animation '" + TextFxAnimationConfigs.m_config_list[m_selected_animation_idx,0] + "' imported");
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);


			
			EditorGUILayout.LabelField("Font Setup Data", EditorStyles.boldLabel);
			
	#if !UNITY_3_5
			font_manager.m_font = EditorGUILayout.ObjectField(new GUIContent("Font (.ttf, .dfont, .otf)", "Your font file to use for this text."), font_manager.m_font, typeof(Font), true) as Font;
			if(GUI.changed && font_manager.m_font != null)
			{
				font_manager.gameObject.GetComponent<Renderer>().material = font_manager.m_font.material;
				font_manager.m_font_material = font_manager.m_font.material;
				font_manager.SetText(font_manager.m_text, true);
			}
	#endif
			
			font_manager.m_font_data_file = EditorGUILayout.ObjectField(new GUIContent("Font Data File", "Your Bitmap font text data file."), font_manager.m_font_data_file, typeof(TextAsset), true) as TextAsset;
			if(GUI.changed && font_manager.m_font_data_file != null && font_manager.m_font_material != null)
			{
				// Wipe the old character data hashtable
				font_manager.ClearFontCharacterData();
				font_manager.SetText(font_manager.m_text, true);
				return;
			}
			font_manager.m_font_material = EditorGUILayout.ObjectField(new GUIContent("Font Material", "Your Bitmap font material"), font_manager.m_font_material, typeof(Material), true) as Material;
			if(GUI.changed && font_manager.m_font_data_file != null && font_manager.m_font_material != null)
			{
				// Reset the text with the new material assigned.
				font_manager.gameObject.GetComponent<Renderer>().material = font_manager.m_font_material;
				font_manager.SetText(font_manager.m_text, true);
				return;
			}
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField(new GUIContent("Text", "The text to display."), EditorStyles.boldLabel);
			font_manager.m_text = EditorGUILayout.TextArea(font_manager.m_text, GUILayout.Width(Screen.width - 25));
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
			font_manager.m_display_axis = (TextDisplayAxis) EditorGUILayout.EnumPopup(new GUIContent("Display Axis", "Denotes whether to render the text horizontally or vertically."), font_manager.m_display_axis);
			font_manager.m_text_anchor = (TextAnchor) EditorGUILayout.EnumPopup(new GUIContent("Text Anchor", "Defines the anchor point about which the text is rendered"), font_manager.m_text_anchor);
			font_manager.m_text_alignment = (TextAlignment) EditorGUILayout.EnumPopup(new GUIContent("Text Alignment", "Defines the alignment of the text, just like your favourite word processor."), font_manager.m_text_alignment);
			font_manager.m_character_size = EditorGUILayout.FloatField(new GUIContent("Character Size", "Specifies the size of the text."), font_manager.m_character_size);
			font_manager.m_line_height_factor = EditorGUILayout.FloatField(new GUIContent("Line Height", "Defines the height of the text lines, based on the tallest line. If value is 2, the lines will be spaced at double the height of the tallest line."), font_manager.m_line_height_factor);
			EditorGUILayout.BeginHorizontal();
			font_manager.m_override_font_baseline = EditorGUILayout.Toggle(new GUIContent("Override Font Baseline?", "Allows you to manually set a baseline y-offset for the font to be rendered to."), font_manager.m_override_font_baseline);
			if(font_manager.m_override_font_baseline)
			{
				font_manager.m_font_baseline_override = EditorGUILayout.FloatField(new GUIContent("Font Baseline Offset", ""), font_manager.m_font_baseline_override);
			}
			EditorGUILayout.EndHorizontal();
			font_manager.m_px_offset = EditorGUILayout.Vector2Field("Letter Spacing Offset", font_manager.m_px_offset);
			font_manager.m_max_width = EditorGUILayout.FloatField(new GUIContent("Max Width", "Defines the maximum width of the text, and breaks the text onto new lines to keep it within this maximum."), font_manager.m_max_width);
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Effect Settings", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			font_manager.m_begin_on_start = EditorGUILayout.Toggle(new GUIContent("Play On Start", "Should this effect be automatically triggered when it's first started in the scene?"), font_manager.m_begin_on_start);
			if(font_manager.m_begin_on_start)
			{
				font_manager.m_begin_delay = EditorGUILayout.FloatField(new GUIContent("Delay", "How much the effect is delayed after first being started."), font_manager.m_begin_delay);
				if(font_manager.m_begin_delay < 0)
				{
					font_manager.m_begin_delay = 0;
				}
			}
			EditorGUILayout.EndHorizontal();
			font_manager.m_animation_speed_factor = EditorGUILayout.FloatField("Animation Speed Factor", font_manager.m_animation_speed_factor);
			font_manager.m_on_finish_action = (ON_FINISH_ACTION) EditorGUILayout.EnumPopup(new GUIContent("On Finish Action", "What should happen when the effect finishes?"), font_manager.m_on_finish_action);
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button(!m_previewing_anim || m_paused ? "Play" : "Pause"))
			{
				if(m_previewing_anim)
				{
					m_paused = !m_paused;
					font_manager.Paused = m_paused;
				}
				else
				{
					m_previewing_anim = true;
					
					font_manager.PlayAnimation();
					m_paused = false;
					font_manager.Paused = false;
				}
			
				m_old_time = Time.realtimeSinceStartup;
			}
			if(GUILayout.Button("Reset"))
			{
				m_paused = false;
				m_previewing_anim = false;
				font_manager.ResetAnimation();
				
				SceneView.RepaintAll();
			}
			EditorGUILayout.EndHorizontal();
			
			// Render continue animation buttons
			if(font_manager.Playing)
			{
				EditorGUILayout.BeginHorizontal();
				
				if(font_manager.NumAnimations > 1)
				{
					int continue_count = 0;
					LetterAnimation animation;
					for(int anim_idx=0; anim_idx < font_manager.NumAnimations; anim_idx++)
					{
						animation = font_manager.GetAnimation(anim_idx);
						if(animation.CurrentAnimationState == LETTER_ANIMATION_STATE.WAITING)
						{
							if(GUILayout.Button("Continue[" + (continue_count+1) + "]"))
							{
								font_manager.ContinueAnimation(continue_count);
							}
						}
						continue_count ++;
					}
				}
				
				EditorGUILayout.EndHorizontal();
			}
			
			if (GUI.changed)
			{
				EditorUtility.SetDirty(font_manager);
			}
			
			if(m_old_char_size != font_manager.m_character_size ||
				m_old_display_axis != font_manager.m_display_axis ||
			   	m_old_line_height != font_manager.m_line_height_factor ||
				m_old_max_width != font_manager.m_max_width ||
				!m_old_text.Equals(font_manager.m_text)	||
				m_old_text_alignment != font_manager.m_text_alignment ||
				m_old_text_anchor != font_manager.m_text_anchor ||
				m_old_px_offset != font_manager.m_px_offset ||
				m_old_baseline_override != font_manager.m_override_font_baseline || 
				(font_manager.m_override_font_baseline && m_old_font_baseline != font_manager.m_font_baseline_override))
			{
				font_manager.SetText(font_manager.m_text);
			}

			GUILayout.Space(3);

			GUILayout.BeginHorizontal();
			GUIStyle style = new GUIStyle(EditorStyles.miniButton);

			if(GUILayout.Button(new GUIContent("Copy [S]", "Soft Copy this TextEffect animation configuration, not including any Text settings."), style))
			{
				string json_data = font_manager.ExportData();
				EditorGUIUtility.systemCopyBuffer = json_data;
				EditorPrefs.SetString("EffectExport", json_data);
				Debug.Log("Soft Copied " + font_manager.name);
			}
			if(GUILayout.Button(new GUIContent("Copy [H]", "Hard Copy this TextEffect animation configuration, including all Text settings."), style))
			{
				string json_data = font_manager.ExportData(hard_copy: true);
				EditorGUIUtility.systemCopyBuffer = json_data;
				EditorPrefs.SetString("EffectExport", json_data);
				Debug.Log("Hard Copied " + font_manager.name);
			}
			if(GUILayout.Button(new GUIContent("Paste", "Paste a copied TextEffect animation configuration onto this effect."), style))
			{
				if(EditorPrefs.HasKey("EffectExport"))
				{
					font_manager.ImportData(EditorPrefs.GetString("EffectExport"), true);
					Debug.Log("Pasted onto " + font_manager.name);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(3);
			
			if (GUILayout.Button("Open Animation Editor"))
			{
				EditorWindow.GetWindow(typeof(TextEffectsManager));
			}
			
			if(font_manager.HasAudioParticleChildInstances)
			{
				GUILayout.Space(15);
				
				if (GUILayout.Button("Clear Audio/Particle Instances"))
				{
					font_manager.ClearCachedAudioParticleInstances();
				}
			}
		}
		
		
		void OnSceneGUI()
		{
			if(font_manager == null)
				font_manager = (EffectManager) target;
			
			if(font_manager.NumAnimations == 0)
				return;
			
			bool edit_detected = false;
			int edited_action = -1;
			int start_end_state = 0;
			int action_idx = 0;
			Vector3 position_offset = font_manager.transform.position;
			
			LetterAnimation letter_animation;
			for(int anim_idx=0; anim_idx < font_manager.NumAnimations; anim_idx++)
			{
				letter_animation = font_manager.GetAnimation(anim_idx);
				action_idx= 0;
				
				LetterAction action;
				for(int idx=0; idx < letter_animation.NumActions; idx++)
				{
					action = letter_animation.GetAction(idx);
					
					if(action.m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX && action.m_start_pos.BezierCurve.EditorVisible)
					{
						action.m_start_pos.BezierCurve.OnSceneGUI(position_offset, font_manager.Scale);
		
						if(!edit_detected && GUI.changed)
						{
							edit_detected = true;
							
							edited_action = action_idx;
							start_end_state = 0;
						}
					}
					
					if(action.m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX && action.m_end_pos.BezierCurve.EditorVisible)
					{
						action.m_end_pos.BezierCurve.OnSceneGUI(position_offset, font_manager.Scale);
						
						if(!edit_detected && GUI.changed)
						{
							edit_detected = true;
							
							edited_action = action_idx;
							start_end_state = 1;
						}
					}
					
					action_idx++;
				}
			}
			
			if(edit_detected)
			{
				font_manager.SetAnimationState(edited_action, start_end_state, true);
			}
		}
	}
}