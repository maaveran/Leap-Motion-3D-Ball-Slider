using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using Boomlagoon.JSON;
#endif

#if UNITY_EDITOR 
namespace TextFx
{
	[System.Serializable]
	public class PresetEffectSetting
	{
		public ANIMATION_DATA_TYPE m_data_type;
		public bool m_startState = true;
		public string m_setting_name;
		public int m_animation_idx = 0;
		public int m_action_idx = 0;
		public float m_action_progress_state_override = -1;

		const float LINE_HEIGHT = 20;

		public bool DrawGUISetting(TextFxAnimationManager animation_manager, float gui_x_offset, ref float gui_y_offset, bool gui_already_changed, int action_start_offset = 0, int loop_start_offset = 0)
		{
			LetterAnimation letterAnimation = animation_manager.GetAnimation(m_animation_idx);
			LetterAction letterAction = letterAnimation != null ? letterAnimation.GetAction(m_action_idx + action_start_offset) : null;
			
			if(letterAction == null)
				return false;


			if((m_data_type == ANIMATION_DATA_TYPE.POSITION && m_startState && letterAction.m_start_pos.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.POSITION && !m_startState && letterAction.m_end_pos.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.LOCAL_ROTATION && m_startState && letterAction.m_start_euler_rotation.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.LOCAL_ROTATION && !m_startState && letterAction.m_end_euler_rotation.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.GLOBAL_ROTATION && m_startState && letterAction.m_global_start_euler_rotation.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.GLOBAL_ROTATION && !m_startState && letterAction.m_global_end_euler_rotation.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.LOCAL_SCALE && m_startState && letterAction.m_start_scale.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.LOCAL_SCALE && !m_startState && letterAction.m_end_scale.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.GLOBAL_SCALE && m_startState && letterAction.m_global_start_scale.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.GLOBAL_SCALE && !m_startState && letterAction.m_global_end_scale.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.COLOUR && m_startState && letterAction.m_start_colour.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)) ||
			   (m_data_type == ANIMATION_DATA_TYPE.COLOUR && !m_startState && letterAction.m_end_colour.DrawQuickEditorGUI(this, gui_x_offset, ref gui_y_offset, gui_already_changed)))
			{
				animation_manager.PrepareAnimationData(m_data_type);

				if(!animation_manager.Playing)
				{
					// Set the current state of the animation to show effect of changes
					animation_manager.SetAnimationState(m_action_idx,
					                                    m_startState ? 0 : 1,
					                                    update_mesh: true);
				}
			}
			else if(m_data_type == ANIMATION_DATA_TYPE.DURATION && letterAction.m_duration_progression.DrawQuickEditorGUI("Lerp Duration", gui_x_offset, ref gui_y_offset, gui_already_changed))
			{
				animation_manager.PrepareAnimationData(m_data_type);

				animation_manager.PrepareAnimationData(ANIMATION_DATA_TYPE.DELAY);
			}
			else if(m_data_type == ANIMATION_DATA_TYPE.DELAY && letterAction.m_delay_progression.DrawQuickEditorGUI("Delay Easing (Seconds)", gui_x_offset, ref gui_y_offset, gui_already_changed))
			{
				animation_manager.PrepareAnimationData(m_data_type);
			}
//			else if(m_data_type == ANIMATION_DATA_TYPE.COLOUR_START_END && letterAction.m_start_colour.DrawQuickEditorGUI(this, ref gui_y_offset, gui_already_changed))
//			{
//				// Set end colour to be the same at start colour
//				if(letterAction.m_start_colour.Progression == (int) ValueProgression.Constant)
//					letterAction.m_end_colour.SetConstant(letterAction.m_start_colour.ValueFrom);
//				else if(letterAction.m_start_colour.Progression == (int) ValueProgression.Eased)
//				{
//					if(letterAction.m_start_colour.UsingThirdValue)
//						letterAction.m_end_colour.SetEased(letterAction.m_start_colour.ValueFrom, letterAction.m_start_colour.ValueTo, letterAction.m_start_colour.ValueThen);
//					else
//						letterAction.m_end_colour.SetEased(letterAction.m_start_colour.ValueFrom, letterAction.m_start_colour.ValueTo);
//				}
//				else if(letterAction.m_start_colour.Progression == (int) ValueProgression.EasedCustom)
//					letterAction.m_end_colour.SetEasedCustom(letterAction.m_start_colour.ValueFrom, letterAction.m_start_colour.ValueTo);
//				else if(letterAction.m_start_colour.Progression == (int) ValueProgression.Random)
//					letterAction.m_end_colour.SetRandom(letterAction.m_start_colour.ValueFrom, letterAction.m_start_colour.ValueTo);
//
//				animation_manager.PrepareAnimationData(ANIMATION_DATA_TYPE.COLOUR_END);
//
//				animation_manager.PrepareAnimationData(ANIMATION_DATA_TYPE.COLOUR_START);
//				
//				if(!animation_manager.Playing)
//				{
//					// Set the current state of the animation to show effect of changes
//					animation_manager.SetAnimationState(m_action_idx, Mathf.Clamp(m_action_progress_state_override, 0f, 1f), update_mesh: true);
//				}
//			}
			else if(m_data_type == ANIMATION_DATA_TYPE.EASE_TYPE)
			{
				letterAction.m_ease_type = (EasingEquation) EditorGUI.EnumPopup(new Rect(gui_x_offset, gui_y_offset, 350, LINE_HEIGHT), m_setting_name, letterAction.m_ease_type);

				if(!gui_already_changed && GUI.changed)
					animation_manager.PrepareAnimationData(ANIMATION_DATA_TYPE.ALL);

				gui_y_offset += LINE_HEIGHT;
			}
			else if(m_data_type == ANIMATION_DATA_TYPE.DELAY_EASED_RANDOM_SWITCH)
			{
				bool newSelection = EditorGUI.Toggle(new Rect(gui_x_offset, gui_y_offset, 250, LINE_HEIGHT), "Randomised?", letterAction.m_delay_progression.Progression == (int) ValueProgression.Random);

				if(!gui_already_changed && GUI.changed)
				{
					if(newSelection)
						letterAction.m_delay_progression.SetRandom(letterAction.m_delay_progression.ValueFrom, letterAction.m_delay_progression.ValueTo, letterAction.m_delay_progression.UniqueRandomRaw);
					else
						letterAction.m_delay_progression.SetEased(letterAction.m_delay_progression.ValueFrom, letterAction.m_delay_progression.ValueTo);

					animation_manager.PrepareAnimationData(ANIMATION_DATA_TYPE.DELAY);
				}

				gui_y_offset += LINE_HEIGHT;
			}
			else if(m_data_type == ANIMATION_DATA_TYPE.NUM_LOOP_ITERATIONS)
			{
				ActionLoopCycle loop_cycle = letterAnimation.GetLoop(m_action_idx + loop_start_offset);

				if(loop_cycle != null)
					loop_cycle.m_number_of_loops = EditorGUI.IntField(new Rect(gui_x_offset, gui_y_offset, 250, LINE_HEIGHT), m_setting_name, loop_cycle.m_number_of_loops);


				gui_y_offset += LINE_HEIGHT;
			}
			
			return !gui_already_changed && GUI.changed;
		}

		public JSONValue ExportData()
		{
			JSONObject jsonData = new JSONObject ();

			jsonData ["name"] = m_setting_name;
			jsonData ["data_type"] = (int) m_data_type;
			jsonData ["anim_idx"] = m_animation_idx;
			jsonData ["action_idx"] = m_action_idx;
			jsonData ["startState"] = m_startState;

			return new JSONValue(jsonData);
		}

		public void ImportData(JSONObject jsonData)
		{
			m_setting_name = jsonData ["name"].Str;
			m_data_type = (ANIMATION_DATA_TYPE) ((int) jsonData ["data_type"].Number);
			m_animation_idx = (int) jsonData ["anim_idx"].Number;
			m_action_idx = (int) jsonData ["action_idx"].Number;

			if(jsonData.ContainsKey("startState"))
				m_startState = jsonData["startState"].Boolean;
		}
	}
}
#endif