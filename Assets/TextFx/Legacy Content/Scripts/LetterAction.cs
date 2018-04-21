using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;

namespace TextFx.LegacyContent
{

	public enum ACTION_TYPE
	{
		ANIM_SEQUENCE,
		BREAK
	}

	public enum PLAY_ITEM_EVENTS
	{
		ON_START,
		ON_FINISH
	}

	public enum PARTICLE_EFFECT_TYPE
	{
		SHURIKEN,
		LEGACY
	}

	public enum PLAY_ITEM_ASSIGNMENT
	{
		PER_LETTER,
		CUSTOM
	}

	[System.Serializable]
	public class ParticleEffectSetup : EffectItemSetup
	{
		public PARTICLE_EFFECT_TYPE m_effect_type;
		public ParticleEmitter m_legacy_particle_effect;
		public ParticleSystem m_shuriken_particle_effect;
		public ActionFloatProgression m_duration = new ActionFloatProgression(0);
		public bool m_follow_mesh = false;
		public ActionVector3Progression m_position_offset = new ActionVector3Progression(Vector3.zero);
		public ActionVector3Progression m_rotation_offset = new ActionVector3Progression(Vector3.zero);
		public bool m_rotate_relative_to_letter = true;
		
		
		public JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_effect_type"] = (int) m_effect_type;
			if(m_effect_type == PARTICLE_EFFECT_TYPE.LEGACY)
				json_data["m_legacy_particle_effect"] = m_legacy_particle_effect.ToPath();
			else
				json_data["m_shuriken_particle_effect"] = m_shuriken_particle_effect.ToPath();
			json_data["m_duration"] = m_duration.ExportData();
			json_data["m_follow_mesh"] = m_follow_mesh;
			json_data["m_position_offset"] = m_position_offset.ExportData();
			json_data["m_rotation_offset"] = m_rotation_offset.ExportData();
			json_data["m_rotate_relative_to_letter"] = m_rotate_relative_to_letter;
			
			return new JSONValue(json_data);
		}
		
		public void ImportData(JSONObject json_data)
		{
			m_effect_type = (PARTICLE_EFFECT_TYPE) (int) json_data["m_effect_type"].Number;
			if(m_effect_type == PARTICLE_EFFECT_TYPE.LEGACY)
				m_legacy_particle_effect = json_data["m_legacy_particle_effect"].Str.PathToParticleEmitter();
			else
				m_shuriken_particle_effect = json_data["m_shuriken_particle_effect"].Str.PathToParticleSystem();
			m_duration.ImportData(json_data["m_duration"].Obj);
			m_follow_mesh = json_data["m_follow_mesh"].Boolean;
			m_position_offset.ImportData(json_data["m_position_offset"].Obj);
			m_rotation_offset.ImportData(json_data["m_rotation_offset"].Obj);
			m_rotate_relative_to_letter = json_data["m_rotate_relative_to_letter"].Boolean;
			
			ImportBaseData(json_data);
		}
	}

	[System.Serializable]
	public class AudioEffectSetup : EffectItemSetup
	{
		public AudioClip m_audio_clip;
		public ActionFloatProgression m_offset_time = new ActionFloatProgression(0);
		public ActionFloatProgression m_volume = new ActionFloatProgression(1);
		public ActionFloatProgression m_pitch = new ActionFloatProgression(1);
		
		public JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_audio_clip"] = m_audio_clip.ToPath();
			json_data["m_offset_time"] = m_offset_time.ExportData();
			json_data["m_volume"] = m_volume.ExportData();
			json_data["m_pitch"] = m_pitch.ExportData();
			
			return new JSONValue(json_data);
		}
		
		public void ImportData(JSONObject json_data)
		{
			m_audio_clip = json_data["m_audio_clip"].Str.PathToAudioClip();
			m_offset_time.ImportData(json_data["m_offset_time"].Obj);
			m_volume.ImportData(json_data["m_volume"].Obj);
			m_pitch.ImportData(json_data["m_pitch"].Obj);
			
			ImportBaseData(json_data);
		}
	}

	[System.Serializable]
	public class EffectItemSetup
	{
		public bool m_editor_display;
		public PLAY_ITEM_EVENTS m_play_when;
		public PLAY_ITEM_ASSIGNMENT m_effect_assignment;
		public bool m_loop_play_once = false;
		public Vector2 CUSTOM_LETTERS_LIST_POS = Vector2.zero;
		public List<int> m_effect_assignment_custom_letters;
		public ActionFloatProgression m_delay = new ActionFloatProgression(0);
		
		public void ExportBaseData(ref JSONObject json_data)
		{
			json_data["m_play_when"] = (int) m_play_when;
			json_data["m_effect_assignment"] = (int) m_effect_assignment;
			json_data["m_loop_play_once"] = m_loop_play_once;
			json_data["m_effect_assignment_custom_letters"] = m_effect_assignment_custom_letters.ExportData();
			json_data["m_delay"] = m_delay.ExportData();
		}
		
		public void ImportBaseData(JSONObject json_data)
		{
			m_play_when = (PLAY_ITEM_EVENTS) (int) json_data["m_play_when"].Number;
			m_effect_assignment = (PLAY_ITEM_ASSIGNMENT) (int) json_data["m_effect_assignment"].Number;
			m_loop_play_once = json_data["m_loop_play_once"].Boolean;
			m_delay.ImportData(json_data["m_delay"].Obj);
			
			m_effect_assignment_custom_letters = json_data["m_effect_assignment_custom_letters"].Array.JSONtoListInt();
			m_loop_play_once = json_data["m_loop_play_once"].Boolean;
		}
	}

	[System.Serializable]
	public class LetterAction
	{
		bool m_editor_folded = false;
		public bool FoldedInEditor { get { return m_editor_folded; } set { m_editor_folded = value; } }
		
		public bool m_offset_from_last = false;
		
		public ACTION_TYPE m_action_type = ACTION_TYPE.ANIM_SEQUENCE;
		
		public bool m_use_gradient_start = false;
		public ActionColorProgression m_start_colour = new ActionColorProgression(Color.white);
		public ActionVertexColorProgression m_start_vertex_colour = new ActionVertexColorProgression(new VertexColour(Color.white));
		public bool m_use_gradient_end = false;
		public ActionColorProgression m_end_colour = new ActionColorProgression(Color.white);
		public ActionVertexColorProgression m_end_vertex_colour = new ActionVertexColorProgression(new VertexColour(Color.white));

		public AxisEasingOverrideData  m_position_axis_ease_data = new AxisEasingOverrideData();
		public ActionPositionVector3Progression m_start_pos = new ActionPositionVector3Progression(Vector3.zero);
		public ActionPositionVector3Progression m_end_pos = new ActionPositionVector3Progression(Vector3.zero);
		public AxisEasingOverrideData m_rotation_axis_ease_data = new AxisEasingOverrideData();
		public ActionVector3Progression m_start_euler_rotation = new ActionVector3Progression(Vector3.zero);
		public ActionVector3Progression m_end_euler_rotation = new ActionVector3Progression(Vector3.zero);
		public AxisEasingOverrideData m_scale_axis_ease_data = new AxisEasingOverrideData();
		public ActionVector3Progression m_start_scale = new ActionVector3Progression(Vector3.one);
		public ActionVector3Progression m_end_scale = new ActionVector3Progression(Vector3.one);
		
		public bool m_force_same_start_time = false;
		
		public ActionFloatProgression m_delay_progression = new ActionFloatProgression(0);
		public ActionFloatProgression m_duration_progression = new ActionFloatProgression(1);
		
		public EasingEquation m_ease_type = EasingEquation.Linear;
		public int m_letter_anchor_start = -1;
		public int m_letter_anchor_end = (int) TextfxTextAnchor.MiddleCenter;
		public bool m_letter_anchor_2_way = false;
		
		[SerializeField]
		List<ParticleEffectSetup> m_particle_effects = new List<ParticleEffectSetup>();
		[SerializeField]
		List<AudioEffectSetup> m_audio_effects = new List<AudioEffectSetup>();
		
		[SerializeField]
		Vector3 m_anchor_offset;
		[SerializeField]
		Vector3 m_anchor_offset_end;
		
		public Vector3 AnchorOffsetStart { get { return m_anchor_offset; } }
		public Vector3 AnchorOffsetEnd { get { return m_anchor_offset_end; } }
		public int NumParticleEffectSetups { get { return m_particle_effects != null ? m_particle_effects.Count : 0; } }
		public int NumAudioEffectSetups { get { return m_audio_effects != null ? m_audio_effects.Count : 0; } }
		public List<ParticleEffectSetup> ParticleEffectSetups { get { return m_particle_effects; } }
		public List<AudioEffectSetup> AudioEffectSetups { get { return m_audio_effects; } }
		public bool ParticleEffectsEditorDisplay { get; set; }
		public bool AudioEffectsEditorDisplay { get; set; }
		
		public ParticleEffectSetup GetParticleEffectSetup(int index)
		{
			if(index >= 0 && index < m_particle_effects.Count)
				return m_particle_effects[index];
			else
				return null;
		}
		
		public ParticleEffectSetup AddParticleEffectSetup()
		{
			if(m_particle_effects == null)
				m_particle_effects = new List<ParticleEffectSetup>();
			
			ParticleEffectSetup new_particle_effect = new ParticleEffectSetup();
			m_particle_effects.Add(new_particle_effect);
			
			return new_particle_effect;
		}
		
		public void AddParticleEffectSetup(ParticleEffectSetup particle_setup)
		{
			if(m_particle_effects == null)
				m_particle_effects = new List<ParticleEffectSetup>();
			
			m_particle_effects.Add(particle_setup);
		}
		
		public void RemoveParticleEffectSetup(int index)
		{
			if(m_particle_effects != null && index >= 0 && index < m_particle_effects.Count)
				m_particle_effects.RemoveAt(index);
		}
		
		public void ClearParticleEffectSetups()
		{
			m_particle_effects.Clear();
		}
		
		public AudioEffectSetup GetAudioEffectSetup(int index)
		{
			if(index >= 0 && index < m_audio_effects.Count)
				return m_audio_effects[index];
			else
				return null;
		}
		
		public AudioEffectSetup AddAudioEffectSetup()
		{
			if(m_audio_effects == null)
				m_audio_effects = new List<AudioEffectSetup>();
			
			AudioEffectSetup new_audio_effect = new AudioEffectSetup();
			m_audio_effects.Add(new_audio_effect);
			
			return new_audio_effect;
		}
		
		public void AddAudioEffectSetup(AudioEffectSetup audio_setup)
		{
			if(m_audio_effects == null)
				m_audio_effects = new List<AudioEffectSetup>();
			
			m_audio_effects.Add(audio_setup);
		}
		
		public void RemoveAudioEffectSetup(int index)
		{
			if(m_audio_effects != null && index >= 0 && index < m_audio_effects.Count)
				m_audio_effects.RemoveAt(index);
		}
		
		public void ClearAudioEffectSetups()
		{
			m_audio_effects.Clear();
		}
		
		public void SoftReset(LetterAction prev_action, AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per, bool first_action = false)
		{
			if(m_use_gradient_start)
			{
				if(!m_offset_from_last && m_start_vertex_colour.UniqueRandom && !first_action)
				{
					m_start_vertex_colour.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_vertex_colour.Values : null);
				}
			}
			else
			{
				if(!m_offset_from_last && m_start_colour.UniqueRandom && !first_action)
				{
					m_start_colour.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_colour.Values : null);
				}
			}
			
			if(!m_offset_from_last && !first_action)
			{
				if(m_start_pos.UniqueRandom)
				{
					m_start_pos.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_pos.Values : null);
				}
				if(m_start_euler_rotation.UniqueRandom)
				{
					m_start_euler_rotation.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_euler_rotation.Values : null);
				}
				if(m_start_scale.UniqueRandom)
				{
					m_start_scale.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_scale.Values : null);
				}
			}
			
			// End State Unique Randoms
			if(m_use_gradient_end)
			{
				if(m_end_vertex_colour.UniqueRandom)
				{
					m_end_vertex_colour.CalculateUniqueRandom(progression_variables, animate_per, m_start_vertex_colour.Values);
				}
			}
			else
			{
				if(m_end_colour.UniqueRandom)
				{
					m_end_colour.CalculateUniqueRandom(progression_variables, animate_per, m_start_colour.Values);
				}
			}
			if(m_end_pos.UniqueRandom)
			{
				m_end_pos.CalculateUniqueRandom(progression_variables, animate_per, m_start_pos.Values);
			}
			if(m_end_euler_rotation.UniqueRandom)
			{
				m_end_euler_rotation.CalculateUniqueRandom(progression_variables, animate_per, m_start_euler_rotation.Values);
			}
			if(m_end_scale.UniqueRandom)
			{
				m_end_scale.CalculateUniqueRandom(progression_variables, animate_per, m_start_scale.Values);
			}
			
			
			// Timing unique randoms
			if(m_delay_progression.UniqueRandom)
			{
				m_delay_progression.CalculateUniqueRandom(progression_variables, animate_per);
			}
			if(m_duration_progression.UniqueRandom)
			{
				m_duration_progression.CalculateUniqueRandom(progression_variables, animate_per);
			}
			
			if(m_audio_effects != null)
			{
				foreach(AudioEffectSetup effect_setup in m_audio_effects)
				{
					if(effect_setup.m_delay.UniqueRandom)
						effect_setup.m_delay.CalculateUniqueRandom(progression_variables, animate_per);
					if(effect_setup.m_offset_time.UniqueRandom)
						effect_setup.m_offset_time.CalculateUniqueRandom(progression_variables, animate_per);
					if(effect_setup.m_volume.UniqueRandom)
						effect_setup.m_volume.CalculateUniqueRandom(progression_variables, animate_per);
					if(effect_setup.m_pitch.UniqueRandom)
						effect_setup.m_pitch.CalculateUniqueRandom(progression_variables, animate_per);
				}
			}
			
			if(m_particle_effects != null)
			{
				foreach(ParticleEffectSetup effect_setup in m_particle_effects)
				{
					if(effect_setup.m_position_offset.UniqueRandom)
						effect_setup.m_position_offset.CalculateUniqueRandom(progression_variables, animate_per, null);
					if(effect_setup.m_rotation_offset.UniqueRandom)
						effect_setup.m_rotation_offset.CalculateUniqueRandom(progression_variables, animate_per, null);
					if(effect_setup.m_delay.UniqueRandom)
						effect_setup.m_delay.CalculateUniqueRandom(progression_variables, animate_per);
					if(effect_setup.m_duration.UniqueRandom)
						effect_setup.m_duration.CalculateUniqueRandom(progression_variables, animate_per);
				}
			}
		}
		
		public void SoftResetStarts(LetterAction prev_action, AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per)
		{
			if(m_use_gradient_start)
			{
				if(!m_offset_from_last && m_start_vertex_colour.UniqueRandom)
				{
					m_start_vertex_colour.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_vertex_colour.Values : null);
				}
			}
			else
			{
				if(!m_offset_from_last && m_start_colour.UniqueRandom)
				{
					m_start_colour.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_colour.Values : null);
				}
			}
			
			if(!m_offset_from_last)
			{
				if(m_start_pos.UniqueRandom)
				{
					m_start_pos.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_pos.Values : null);
				}
				if(m_start_euler_rotation.UniqueRandom)
				{
					m_start_euler_rotation.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_euler_rotation.Values : null);
				}
				if(m_start_scale.UniqueRandom)
				{
					m_start_scale.CalculateUniqueRandom(progression_variables, animate_per, prev_action != null ? prev_action.m_end_scale.Values : null);
				}
			}
		}
		
		public LetterAction ContinueActionFromThis()
		{
			LetterAction letter_action = new LetterAction();
			
			// Default to offset from previous and not be folded in editor
			letter_action.m_offset_from_last = true;
			letter_action.m_editor_folded = true;
			
			letter_action.m_use_gradient_start = m_use_gradient_start;
			letter_action.m_use_gradient_end = m_use_gradient_end;
			
			letter_action.m_position_axis_ease_data = m_position_axis_ease_data.Clone();
			letter_action.m_rotation_axis_ease_data = m_rotation_axis_ease_data.Clone();
			letter_action.m_scale_axis_ease_data = m_scale_axis_ease_data.Clone();
			
			letter_action.m_start_colour = m_end_colour.Clone();
			letter_action.m_end_colour = m_end_colour.Clone();
			letter_action.m_start_vertex_colour = m_end_vertex_colour.Clone();
			letter_action.m_end_vertex_colour = m_end_vertex_colour.Clone();
			
			letter_action.m_start_pos = m_end_pos.CloneThis();
			letter_action.m_end_pos = m_end_pos.CloneThis();
			
			letter_action.m_start_euler_rotation = m_end_euler_rotation.Clone();
			letter_action.m_end_euler_rotation = m_end_euler_rotation.Clone();
			
			letter_action.m_start_scale = m_end_scale.Clone();
			letter_action.m_end_scale = m_end_scale.Clone();
			
			letter_action.m_delay_progression = new ActionFloatProgression(0);
			letter_action.m_duration_progression = new ActionFloatProgression(1);
			
			letter_action.m_letter_anchor_start = m_letter_anchor_2_way ? m_letter_anchor_end : m_letter_anchor_start;
			
			letter_action.m_ease_type = m_ease_type;
			
			return letter_action;
		}
		
		int GetProgressionTotal(int num_letters, int num_words, int num_lines, AnimatePerOptions animate_per_default, AnimatePerOptions animate_per_override, bool overriden)
		{
			switch(overriden ? animate_per_override : animate_per_default)
			{
				case AnimatePerOptions.LETTER:
					return num_letters;
				case AnimatePerOptions.WORD:
					return num_words;
				case AnimatePerOptions.LINE:
					return num_lines;
			}
			
			return num_letters;
		}
		
		public void PrepareData(ref LetterSetup[] letters, int num_letters, int num_words, int num_lines, LetterAction prev_action, AnimatePerOptions animate_per, bool prev_action_end_state = true)
		{
			m_duration_progression.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_duration_progression.AnimatePer, m_duration_progression.OverrideAnimatePerOption));
			
			
			if(m_audio_effects != null)
			{
				foreach(AudioEffectSetup effect_setup in m_audio_effects)
				{
					effect_setup.m_delay.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_delay.AnimatePer, effect_setup.m_delay.OverrideAnimatePerOption));
					effect_setup.m_offset_time.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_offset_time.AnimatePer, effect_setup.m_offset_time.OverrideAnimatePerOption));
					effect_setup.m_volume.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_volume.AnimatePer, effect_setup.m_volume.OverrideAnimatePerOption));
					effect_setup.m_pitch.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_pitch.AnimatePer, effect_setup.m_pitch.OverrideAnimatePerOption));
				}
			}

			if(m_particle_effects != null)
			{
				foreach(ParticleEffectSetup effect_setup in m_particle_effects)
				{
					effect_setup.m_position_offset.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_position_offset.AnimatePer, effect_setup.m_position_offset.OverrideAnimatePerOption), null);
					effect_setup.m_rotation_offset.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_rotation_offset.AnimatePer, effect_setup.m_rotation_offset.OverrideAnimatePerOption), null);
					effect_setup.m_delay.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_delay.AnimatePer, effect_setup.m_delay.OverrideAnimatePerOption));
					effect_setup.m_duration.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, effect_setup.m_duration.AnimatePer, effect_setup.m_duration.OverrideAnimatePerOption));
				}
			}
			
			if(m_action_type == ACTION_TYPE.BREAK)
			{
				return;
			}
			
			m_delay_progression.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_delay_progression.AnimatePer, m_delay_progression.OverrideAnimatePerOption));
			
			
			if(m_offset_from_last && prev_action != null)
			{
				m_use_gradient_start = prev_action.m_use_gradient_end;
				
				if(prev_action_end_state)
				{
					if(m_use_gradient_start)
					{
						m_start_vertex_colour.Values = prev_action.m_end_vertex_colour.Values;
					}
					else
					{
						m_start_colour.Values = prev_action.m_end_colour.Values;
					}
				}
				else
				{
					if(m_use_gradient_start)
					{
						m_start_vertex_colour.Values = prev_action.m_start_vertex_colour.Values;
					}
					else
					{
						m_start_colour.Values = prev_action.m_start_colour.Values;
					}
				}
			}
			else
			{
				if(m_use_gradient_start || (prev_action != null && (prev_action.m_use_gradient_end)))
				{
					if(!m_use_gradient_start)
					{
						// Need to convert flat colour into a vertex colour
						m_use_gradient_start = true;
						
						m_start_vertex_colour.ConvertFromFlatColourProg(m_start_colour);
					}
					
					// add this colour to previous state
					m_start_vertex_colour.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_start_vertex_colour.AnimatePer, m_start_vertex_colour.OverrideAnimatePerOption),
																prev_action != null && (prev_action.m_use_gradient_end) ? prev_action.m_end_vertex_colour.Values : null,
																prev_action != null && (!prev_action.m_use_gradient_end) ? prev_action.m_end_colour.Values : null);
				}
				else
				{
					m_start_colour.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_start_colour.AnimatePer, m_start_colour.OverrideAnimatePerOption), 
														prev_action != null ? prev_action.m_end_colour.Values : null);
				}
			}
			
			if(m_use_gradient_end || m_use_gradient_start)
			{
				if(!m_use_gradient_end)
				{
					// Need to convert flat colour into a vertex colour
					m_use_gradient_end = true;
					
					m_end_vertex_colour.ConvertFromFlatColourProg(m_end_colour);
				}
				
				m_end_vertex_colour.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_end_vertex_colour.AnimatePer, m_end_vertex_colour.OverrideAnimatePerOption),
															(m_use_gradient_start) ? m_start_vertex_colour.Values : null,
															(!m_use_gradient_start) ? m_start_colour.Values : null);
			}
			else
			{
				m_end_colour.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_end_colour.AnimatePer, m_end_colour.OverrideAnimatePerOption),
													m_start_colour.Values);
			}
			
			
			if(m_offset_from_last && prev_action != null)
			{
				m_start_pos.Values = prev_action_end_state ? prev_action.m_end_pos.Values : prev_action.m_start_pos.Values;
				m_start_euler_rotation.Values = prev_action_end_state ? prev_action.m_end_euler_rotation.Values : prev_action.m_start_euler_rotation.Values;
				m_start_scale.Values = prev_action_end_state ? prev_action.m_end_scale.Values : prev_action.m_start_scale.Values;
			}
			else
			{
				float[] start_pos_curve_letter_progressions = null;
				if(m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX)
				{
					// Pre calculate letter progression values based on letter spacing
					start_pos_curve_letter_progressions = m_start_pos.BezierCurve.GetLetterProgressions(ref letters, m_letter_anchor_start);
				}
				
				m_start_pos.CalculatePositionProgressions(ref start_pos_curve_letter_progressions, GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_start_pos.AnimatePer, m_start_pos.OverrideAnimatePerOption),
													prev_action != null ? prev_action.m_end_pos.Values : new Vector3[]{Vector3.zero});
				m_start_euler_rotation.CalculateRotationProgressions(ref start_pos_curve_letter_progressions, GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_start_euler_rotation.AnimatePer, m_start_euler_rotation.OverrideAnimatePerOption),
													prev_action != null ? prev_action.m_end_euler_rotation.Values : new Vector3[]{Vector3.zero},
													m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX ? m_start_pos.BezierCurve : null);
				m_start_scale.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_start_scale.AnimatePer, m_start_scale.OverrideAnimatePerOption),
													prev_action != null ? prev_action.m_end_scale.Values : new Vector3[]{Vector3.zero});
			}
			
			float[] end_pos_curve_letter_progressions = null;
			if(m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX)
			{
				// Pre calculate letter progression values based on letter spacing
				end_pos_curve_letter_progressions = m_end_pos.BezierCurve.GetLetterProgressions(ref letters, m_letter_anchor_end);
			}
			
			m_end_pos.CalculatePositionProgressions(ref end_pos_curve_letter_progressions, GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_end_pos.AnimatePer, m_end_pos.OverrideAnimatePerOption), m_start_pos.Values);
			m_end_euler_rotation.CalculateRotationProgressions(ref end_pos_curve_letter_progressions, GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_end_euler_rotation.AnimatePer, m_end_euler_rotation.OverrideAnimatePerOption),
														m_start_euler_rotation.Values,
														m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX ? m_end_pos.BezierCurve : null);
			m_end_scale.CalculateProgressions(GetProgressionTotal(num_letters, num_words, num_lines, animate_per, m_end_scale.AnimatePer, m_end_scale.OverrideAnimatePerOption), m_start_scale.Values);
			
			CalculateLetterAnchorOffset();
		}
		
		public void CalculateLetterAnchorOffset()
		{
			// Force curve actions to use Baseline anchorings
			if(m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX)
				m_letter_anchor_start = Mathf.Max(m_letter_anchor_start, (int) TextfxTextAnchor.BaselineCenter);
			if(m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX)
				if(m_letter_anchor_2_way)
					m_letter_anchor_end = Mathf.Max(m_letter_anchor_end, (int) TextfxTextAnchor.BaselineCenter);
				else
					m_letter_anchor_start = Mathf.Max(m_letter_anchor_start, (int) TextfxTextAnchor.BaselineCenter);
			
			// Calculate letters anchor offset vector
			m_anchor_offset = AnchorOffsetToVector3((TextfxTextAnchor) m_letter_anchor_start);
			
			m_anchor_offset_end = m_letter_anchor_2_way ? AnchorOffsetToVector3((TextfxTextAnchor) m_letter_anchor_end) : m_anchor_offset;
		}
		
		Vector3 AnchorOffsetToVector3(TextfxTextAnchor anchor)
		{
			Vector3 anchor_vec = Vector3.zero;
			if(anchor == TextfxTextAnchor.UpperRight || anchor == TextfxTextAnchor.MiddleRight || anchor == TextfxTextAnchor.LowerRight || anchor == TextfxTextAnchor.BaselineRight)
			{
				anchor_vec.x = 1;
			}
			else if(anchor == TextfxTextAnchor.UpperCenter || anchor == TextfxTextAnchor.MiddleCenter || anchor == TextfxTextAnchor.LowerCenter || anchor == TextfxTextAnchor.BaselineCenter)
			{
				anchor_vec.x = 0.5f;
			}
			
			// handle letter anchor y-offset
			if(anchor == TextfxTextAnchor.MiddleLeft || anchor == TextfxTextAnchor.MiddleCenter || anchor == TextfxTextAnchor.MiddleRight)
			{
				anchor_vec.y = 0.5f;
			}
			else if(anchor == TextfxTextAnchor.LowerLeft || anchor == TextfxTextAnchor.LowerCenter || anchor == TextfxTextAnchor.LowerRight)
			{
				anchor_vec.y = 1;
			}
			return anchor_vec;
		}

		public JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			json_data["m_action_type"] = (int) m_action_type;
			json_data["m_ease_type"] = (int) m_ease_type;
			json_data["m_use_gradient_start"] = m_use_gradient_start;
			json_data["m_use_gradient_end"] = m_use_gradient_end;
			json_data["m_force_same_start_time"] = m_force_same_start_time;
			json_data["m_letter_anchor_start"] = m_letter_anchor_start;
			json_data["m_letter_anchor_end"] = m_letter_anchor_end;
			json_data["m_letter_anchor_2_way"] = m_letter_anchor_2_way;
			json_data["m_offset_from_last"] = m_offset_from_last;
			json_data["m_position_axis_ease_data"] = m_position_axis_ease_data.ExportData();
			json_data["m_rotation_axis_ease_data"] = m_rotation_axis_ease_data.ExportData();
			json_data["m_scale_axis_ease_data"] = m_scale_axis_ease_data.ExportData();
			
			if(m_use_gradient_start)
				json_data["m_start_vertex_colour"] = m_start_vertex_colour.ExportData();
			else
				json_data["m_start_colour"] = m_start_colour.ExportData();
			json_data["m_start_euler_rotation"] = m_start_euler_rotation.ExportData();
			json_data["m_start_pos"] = m_start_pos.ExportData();
			json_data["m_start_scale"] = m_start_scale.ExportData();
			
			if(m_use_gradient_end)
				json_data["m_end_vertex_colour"] = m_end_vertex_colour.ExportData();
			else
				json_data["m_end_colour"] = m_end_colour.ExportData();
			json_data["m_end_euler_rotation"] = m_end_euler_rotation.ExportData();
			json_data["m_end_pos"] = m_end_pos.ExportData();
			json_data["m_end_scale"] = m_end_scale.ExportData();
			
			json_data["m_delay_progression"] = m_delay_progression.ExportData();
			json_data["m_duration_progression"] = m_duration_progression.ExportData();
			
			
			JSONArray audio_effects_data = new JSONArray();
			foreach(AudioEffectSetup effect_setup in m_audio_effects)
			{
				if(effect_setup.m_audio_clip == null)
					continue;
				
				audio_effects_data.Add(effect_setup.ExportData());
			}
			json_data["AUDIO_EFFECTS_DATA"] = audio_effects_data;
			
			JSONArray particle_effects_data = new JSONArray();
			foreach(ParticleEffectSetup effect_setup in m_particle_effects)
			{
				if(effect_setup.m_legacy_particle_effect == null && effect_setup.m_shuriken_particle_effect == null)
					continue;
				
				particle_effects_data.Add(effect_setup.ExportData());
			}
			json_data["PARTICLE_EFFECTS_DATA"] = particle_effects_data;
			
			return new JSONValue(json_data);
		}

		public void ImportData(JSONObject json_data)
		{	
			m_action_type = (ACTION_TYPE) (int) json_data["m_action_type"].Number;
			m_ease_type = (EasingEquation) (int) json_data["m_ease_type"].Number;
			m_use_gradient_start = json_data["m_use_gradient_start"].Boolean;
			m_use_gradient_end = json_data["m_use_gradient_end"].Boolean;
			m_force_same_start_time = json_data["m_force_same_start_time"].Boolean;
			m_letter_anchor_start = (int) json_data["m_letter_anchor_start"].Number;
			m_letter_anchor_end = (int) json_data["m_letter_anchor_end"].Number;
			m_letter_anchor_2_way = json_data["m_letter_anchor_2_way"].Boolean;
			m_offset_from_last = json_data["m_offset_from_last"].Boolean;
			m_position_axis_ease_data.ImportData(json_data["m_position_axis_ease_data"].Obj);
			m_rotation_axis_ease_data.ImportData(json_data["m_rotation_axis_ease_data"].Obj);
			m_scale_axis_ease_data.ImportData(json_data["m_scale_axis_ease_data"].Obj);
			
			if(m_use_gradient_start)
				m_start_vertex_colour.ImportData(json_data["m_start_vertex_colour"].Obj);
			else
				m_start_colour.ImportData(json_data["m_start_colour"].Obj);
			if(m_use_gradient_end)
				m_end_vertex_colour.ImportData(json_data["m_end_vertex_colour"].Obj);
			else
				m_end_colour.ImportData(json_data["m_end_colour"].Obj);
			
			m_start_euler_rotation.ImportData(json_data["m_start_euler_rotation"].Obj);
			m_end_euler_rotation.ImportData(json_data["m_end_euler_rotation"].Obj);
			m_start_pos.ImportData(json_data["m_start_pos"].Obj);
			m_end_pos.ImportData(json_data["m_end_pos"].Obj);
			m_start_scale.ImportData(json_data["m_start_scale"].Obj);
			m_end_scale.ImportData(json_data["m_end_scale"].Obj);
			m_delay_progression.ImportData(json_data["m_delay_progression"].Obj);
			m_duration_progression.ImportData(json_data["m_duration_progression"].Obj);
			
			
			m_audio_effects = new List<AudioEffectSetup>();
			AudioEffectSetup audio_effect;
			foreach(JSONValue audio_data in json_data["AUDIO_EFFECTS_DATA"].Array)
			{
				audio_effect = new AudioEffectSetup();
				audio_effect.ImportData(audio_data.Obj);
				m_audio_effects.Add(audio_effect);
			}
			
			m_particle_effects = new List<ParticleEffectSetup>();
			ParticleEffectSetup particle_effect;
			foreach(JSONValue particle_data in json_data["PARTICLE_EFFECTS_DATA"].Array)
			{
				particle_effect = new ParticleEffectSetup();
				particle_effect.ImportData(particle_data.Obj);
				m_particle_effects.Add(particle_effect);
			}
		}
	}
}