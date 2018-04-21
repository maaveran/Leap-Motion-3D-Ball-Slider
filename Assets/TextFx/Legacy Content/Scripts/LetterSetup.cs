using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TextFx.LegacyContent
{
	[System.Serializable]
	public class AnimationStateVariables
	{
		public bool m_active;
		public bool m_waiting_to_sync;
		public bool m_started_action;			// triggered when action starts (after initial delay)
		public float m_break_delay;
		public float m_timer_offset;
		public int m_action_index;
		public bool m_reverse;
		public int m_action_index_progress;		// Used to track progress through a loop cycle
		public int m_prev_action_index;
		public float m_linear_progress;
		public float m_action_progress;
		public List<ActionLoopCycle> m_active_loop_cycles;
		
		public AnimationStateVariables Clone()
		{
			return new AnimationStateVariables(){
				m_active = m_active,
				m_waiting_to_sync = m_waiting_to_sync,
				m_started_action = m_started_action,
				m_break_delay = m_break_delay,
				m_timer_offset = m_timer_offset,
				m_action_index = m_action_index,
				m_reverse = m_reverse,
				m_action_index_progress = m_action_index_progress,
				m_prev_action_index = m_prev_action_index,
				m_linear_progress = m_linear_progress,
				m_action_progress = m_action_progress,
				m_active_loop_cycles = m_active_loop_cycles
			};
		}
		
		public void Reset()
		{
			m_active = false;
			m_waiting_to_sync = false;
			m_started_action = false;
			m_break_delay = 0;
			m_timer_offset = 0;
			m_action_index = 0;
			m_reverse = false;
			m_action_index_progress = 0;
			m_prev_action_index = -1;
			m_linear_progress = 0;
			m_action_progress = 0;
			m_active_loop_cycles.Clear();
		}
	}

	[System.Serializable]
	public class LetterSetup
	{
		public string m_character;
		public bool m_flipped = false;
		public float m_offset_width = 0;
		public float m_width = 0;
		public float m_height = 0;
		public Vector3[] m_base_vertices;
		public Vector3 m_base_offset;
		public float m_text_anchoring_y_offset = 0;		// letter y-positional offset based on text anchoring settings. This value is not used when using a belzier curve
		public Mesh m_mesh;
		public float m_x_offset = 0;
		public float m_y_offset = 0;
		public bool m_base_offsets_setup = false;
		public AnimationProgressionVariables m_progression_variables;
		public EffectManager m_effect_manager_handle;
		
		[SerializeField]
		AnimationStateVariables m_anim_state_vars;
		
		LetterAction m_current_letter_action = null;
		float m_action_timer;
		float m_action_delay;
		float m_action_duration;
		VertexColour start_colour, end_colour;			// Used to store the colours for each frame
		AnimatePerOptions m_last_animate_per;
		Vector3 from_vec, to_vec;
		Vector3 m_letter_position, m_letter_scale, m_letter_center;
		Quaternion m_letter_rotation;
		Vector3 m_anchor_offset;
		Vector3[] mesh_verts = null;
		
		// Getter / Setters
		public Vector3 BaseOffset { get { return m_base_offset + new Vector3(0,m_text_anchoring_y_offset,0); } }
		public AnimationStateVariables AnimStateVars { get { return m_anim_state_vars; } }
		public List<ActionLoopCycle> ActiveLoopCycles { get { return m_anim_state_vars.m_active_loop_cycles; } }
		public bool WaitingToSync { get { return m_anim_state_vars.m_waiting_to_sync; } }
		public int ActionIndex { get { return m_anim_state_vars.m_action_index; } }
		public bool InReverse { get { return m_anim_state_vars.m_reverse; } }
		public int ActionProgress { get { return m_anim_state_vars.m_action_index_progress; } }
		public bool Active { get { return m_anim_state_vars.m_active; } set { m_anim_state_vars.m_active = value; } }
		public Vector3 Center { get { return 
				((m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * Vector3.Scale(m_letter_center, (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)))
					+ (m_effect_manager_handle != null ? m_effect_manager_handle.Position : Vector3.zero); } }
		public Vector3 CenterLocal { get { return m_letter_center; } }
		public Vector3 TopLeft { get { return
				((m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * Vector3.Scale((mesh_verts != null ? mesh_verts[1] : Vector3.zero), (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)))
					+ (m_effect_manager_handle != null ? m_effect_manager_handle.Position : Vector3.zero); } }
		public Vector3 TopLeftLocal { get { return (mesh_verts != null ? mesh_verts[1] : Vector3.zero); } }
		public Vector3 TopRight { get { return
				((m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * Vector3.Scale((mesh_verts != null ? mesh_verts[0] : Vector3.zero), (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)))
					+ (m_effect_manager_handle != null ? m_effect_manager_handle.Position : Vector3.zero); } }
		public Vector3 TopRightLocal { get { return (mesh_verts != null ? mesh_verts[0] : Vector3.zero); } }
		public Vector3 BottomLeft { get { return
				((m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * Vector3.Scale((mesh_verts != null ? mesh_verts[2] : Vector3.zero), (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)))
				+ (m_effect_manager_handle != null ? m_effect_manager_handle.Position : Vector3.zero); } }
		public Vector3 BottomLeftLocal { get { return (mesh_verts != null ? mesh_verts[2] : Vector3.zero); } }
		public Vector3 BottomRight { get { return
				((m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * Vector3.Scale((mesh_verts != null ? mesh_verts[3] : Vector3.zero), (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)))
				+ (m_effect_manager_handle != null ? m_effect_manager_handle.Position : Vector3.zero); } }
		public Vector3 BottomRightLocal { get { return (mesh_verts != null ? mesh_verts[3] : Vector3.zero); } }
		public Quaternion Rotation { get { return (m_effect_manager_handle != null ? m_effect_manager_handle.Rotation : Quaternion.identity) * m_letter_rotation; } }
		public Quaternion RotationLocal { get { return m_letter_rotation; } }
		public Vector3 Scale { get { return Vector3.Scale(m_letter_scale, (m_effect_manager_handle != null ? m_effect_manager_handle.Scale : Vector3.one)); } }
		public Vector3 ScaleLocal { get { return m_letter_scale; } }
		
		public LetterSetup(string character, int letter_idx, Mesh mesh, Vector3 base_offset, ref CustomCharacterInfo char_info, int line_num, int word_idx, EffectManager effect_manager)
		{
			m_character = character;
			m_mesh = mesh;
			m_base_offset = base_offset;
			m_effect_manager_handle = effect_manager;
			
			m_progression_variables = new AnimationProgressionVariables(letter_idx, word_idx, line_num);
			
			m_anim_state_vars = new AnimationStateVariables();
			m_anim_state_vars.m_active_loop_cycles = new List<ActionLoopCycle>();
			
			SetupLetterMesh(ref char_info);
			
			if(m_flipped)
			{
				// flip UV coords in x axis.
				m_mesh.uv = new Vector2[] {mesh.uv[3], mesh.uv[2], mesh.uv[1], mesh.uv[0]};
			}
		}
		
		public void Recycle(string character, int letter_idx, Mesh mesh, Vector3 base_offset, ref CustomCharacterInfo char_info, int line_num, int word_idx, EffectManager effect_manager)
		{
			m_character = character;
			m_mesh = mesh;
			m_base_offset = base_offset;
			m_effect_manager_handle = effect_manager;
			
			m_progression_variables = new AnimationProgressionVariables(letter_idx, word_idx, line_num);
			
			SetupLetterMesh(ref char_info);
			
			if(m_flipped)
			{
				// flip UV coords in x axis.
				m_mesh.uv = new Vector2[] {mesh.uv[3], mesh.uv[2], mesh.uv[1], mesh.uv[0]};
			}
			
			m_current_letter_action = null;
		}
		
		public void Init(EffectManager effect_manager)
		{
			if(m_mesh != null)
				mesh_verts = m_mesh.vertices;
			
			m_effect_manager_handle = effect_manager;
			
			// Set default values
			if(mesh_verts != null && mesh_verts.Length == 4)
			{
				m_letter_center = (mesh_verts[0] + mesh_verts[1] + mesh_verts[2] + mesh_verts[3]) / 4;
			}
			
			m_letter_scale = Vector3.one;
			m_letter_rotation = Quaternion.identity;
		}
		
		public void SetupLetterMesh(ref CustomCharacterInfo char_info)
		{
			m_offset_width = char_info.width;
			m_width = char_info.vert.width;
			m_height = char_info.vert.height;
			m_flipped = char_info.flipped;
			
			// Setup base vertices
			m_x_offset = char_info.vert.x;
			m_y_offset = char_info.vert.y;
			
			
			
			if(!m_flipped)
			{
				// TR, TL, BL, BR
				m_base_vertices = new Vector3[] { new Vector3(m_width, (m_effect_manager_handle.FontBaseLine + m_y_offset), 0), new Vector3(0, (m_effect_manager_handle.FontBaseLine + m_y_offset), 0), new Vector3(0, m_height + m_effect_manager_handle.FontBaseLine + m_y_offset, 0), new Vector3(m_width, m_height + m_effect_manager_handle.FontBaseLine + m_y_offset, 0)};
			}
			else
			{
				// rotate order of vertices by one.
				// TL, BL, BR, TR
				m_base_vertices = new Vector3[] {new Vector3(0, (m_effect_manager_handle.FontBaseLine + m_y_offset), 0), new Vector3(0, m_height + m_effect_manager_handle.FontBaseLine + m_y_offset, 0), new Vector3( m_width, m_height + m_effect_manager_handle.FontBaseLine + m_y_offset, 0), new Vector3(m_width, (m_effect_manager_handle.FontBaseLine + m_y_offset), 0)};
			}
		}
		
		public void SetAnimationVars(LetterSetup master_letter)
		{
			m_anim_state_vars = master_letter.AnimStateVars.Clone();
			
			m_current_letter_action = null;
			
			// clone the list of active loop cycles, so that the list reference is not shared between two letters
			m_anim_state_vars.m_active_loop_cycles = new List<ActionLoopCycle>();
			foreach(ActionLoopCycle loop_cycle in master_letter.AnimStateVars.m_active_loop_cycles)
			{
				m_anim_state_vars.m_active_loop_cycles.Add(loop_cycle.Clone());
			}
		}
		
		public void Reset(LetterAnimation animation)
		{
			m_anim_state_vars.Reset();
			
			if(animation.NumLoops > 0)
			{
				UpdateLoopList(animation);
			}
		}
		
		public void SetBaseOffset(TextAnchor anchor, TextDisplayAxis display_axis, TextAlignment alignment, List<TextSizeData> text_datas)
		{
			TextSizeData text_data = text_datas[m_progression_variables.m_line_value];
				
			m_base_offsets_setup = true;
			if(display_axis == TextDisplayAxis.HORIZONTAL)
			{
				m_base_offset += new Vector3(m_x_offset, -m_progression_variables.m_line_value * m_effect_manager_handle.LineHeight, 0);
			}
			else
			{
				m_base_offset += new Vector3(m_progression_variables.m_line_value * m_effect_manager_handle.LineHeight, 0, 0);
			}
			
			// Handle text y offset
			if(anchor == TextAnchor.MiddleLeft || anchor == TextAnchor.MiddleCenter || anchor == TextAnchor.MiddleRight)
			{
				m_text_anchoring_y_offset = (text_data.m_total_text_height / 2) - m_effect_manager_handle.FontBaseLine;
			}
			else if(anchor == TextAnchor.LowerLeft || anchor == TextAnchor.LowerCenter || anchor == TextAnchor.LowerRight)
			{
				m_text_anchoring_y_offset = text_data.m_total_text_height - text_data.m_text_line_height;
			}
			else
			{
				m_text_anchoring_y_offset = -m_effect_manager_handle.FontBaseLine;
			}
			
			
			float alignment_offset = 0;
			if(display_axis == TextDisplayAxis.HORIZONTAL)
			{
				if(alignment == TextAlignment.Center)
				{
					alignment_offset = (text_data.m_total_text_width - text_data.m_text_line_width) / 2;
				}
				else if(alignment == TextAlignment.Right)
				{
					alignment_offset = (text_data.m_total_text_width - text_data.m_text_line_width);
				}
			}
			else
			{
				if(alignment == TextAlignment.Center)
				{
					m_base_offset.y -= (text_data.m_total_text_height - text_data.m_text_line_height) / 2;
				}
				else if(alignment == TextAlignment.Right)
				{
					m_base_offset.y -= (text_data.m_total_text_height - text_data.m_text_line_height);
				}
			}
			
			// Handle text x offset
			if(anchor == TextAnchor.LowerRight || anchor == TextAnchor.MiddleRight || anchor == TextAnchor.UpperRight)
			{
				m_base_offset.x -= text_data.m_total_text_width - alignment_offset;
			}
			else if(anchor == TextAnchor.LowerCenter || anchor == TextAnchor.MiddleCenter || anchor == TextAnchor.UpperCenter)
			{
				m_base_offset.x -= (text_data.m_total_text_width/2) - alignment_offset;
			}
			else
			{
				m_base_offset.x += alignment_offset;
			}
		}
		
		public void SetMeshState(int action_idx, float action_progress, LetterAnimation animation, AnimatePerOptions animate_per, EffectManager effect_manager)
		{
			if(action_idx >= 0 && action_idx < animation.NumActions)
			{
				SetupMesh(animation.GetAction(action_idx), action_idx > 0 ? animation.GetAction(action_idx-1) : null, Mathf.Clamp(action_progress, 0,1), m_progression_variables, animate_per, Mathf.Clamp(action_progress, 0,1), effect_manager);
			}
			else
			{
				// action not found for this letter. Position letter in its default position
				
				if(mesh_verts == null || mesh_verts.Length == 0)
					mesh_verts = new Vector3[4];
				
				for(int idx=0; idx < 4; idx++)
				{
					mesh_verts[idx] = m_base_vertices[idx] + m_base_offset;
				}
				m_mesh.vertices = mesh_verts;
				m_mesh.colors = new Color[]{Color.white, Color.white, Color.white, Color.white};
			}
		}
		
		void SetNextActionIndex(LetterAnimation animation)
		{
			// based on current active loop list, return the next action index
			
			// increment action progress count
			m_anim_state_vars.m_action_index_progress++;
			
			ActionLoopCycle current_loop;
			for(int loop_idx=0; loop_idx < m_anim_state_vars.m_active_loop_cycles.Count; loop_idx++)
			{
				current_loop = m_anim_state_vars.m_active_loop_cycles[loop_idx];
				
				if((current_loop.m_loop_type == LOOP_TYPE.LOOP && m_anim_state_vars.m_action_index == current_loop.m_end_action_idx) ||
					(current_loop.m_loop_type == LOOP_TYPE.LOOP_REVERSE && ((m_anim_state_vars.m_reverse && m_anim_state_vars.m_action_index == current_loop.m_start_action_idx) || (!m_anim_state_vars.m_reverse && m_anim_state_vars.m_action_index == current_loop.m_end_action_idx)))
				)
				{
					
					// Reached end of loop cycle. Deduct one cycle from loop count.
					bool end_of_loop_cycle = current_loop.m_loop_type == LOOP_TYPE.LOOP || m_anim_state_vars.m_reverse;
					
					if(end_of_loop_cycle)
					{
						current_loop.m_number_of_loops--;
					}
					
					// Switch reverse status
					if(current_loop.m_loop_type == LOOP_TYPE.LOOP_REVERSE)
					{
						m_anim_state_vars.m_reverse = !m_anim_state_vars.m_reverse;
					}
					
					current_loop.FirstPass = false;
					
					if(end_of_loop_cycle && current_loop.m_number_of_loops == 0)
					{
						// loop cycle finished
						// Remove this loop from active loop list
						m_anim_state_vars.m_active_loop_cycles.RemoveAt(loop_idx);
						loop_idx--;
						
						if(current_loop.m_loop_type == LOOP_TYPE.LOOP_REVERSE)
						{
							// Don't allow anim to progress back through actions, skip to action beyond end of reverse loop
							m_anim_state_vars.m_action_index = current_loop.m_end_action_idx;
						}
					}
					else
					{
						if(current_loop.m_number_of_loops < 0)
						{
							current_loop.m_number_of_loops = -1;
						}
						
						// return to the start of this loop again
						if(current_loop.m_loop_type == LOOP_TYPE.LOOP)
						{
							m_anim_state_vars.m_action_index = current_loop.m_start_action_idx;
						}
						
						return;
					}
				}
				else
				{
					break;
				}
			}
			
			m_anim_state_vars.m_action_index += (m_anim_state_vars.m_reverse ? -1 : 1);
			
			// check for animation reaching end
			if(m_anim_state_vars.m_action_index >= animation.NumActions)
			{
				m_anim_state_vars.m_active = false;
				m_anim_state_vars.m_action_index = animation.NumActions -1;
			}
			
			return;
		}
		
		// Only called if action_idx has changed since last time
		void UpdateLoopList(LetterAnimation animation)
		{
			// add any new loops from the next action index to the loop list
			ActionLoopCycle loop;
			for(int idx=0; idx < animation.NumLoops; idx++)
			{
				loop = animation.GetLoop(idx);
				
				if(loop.m_start_action_idx == m_anim_state_vars.m_action_index)
				{
					// add this new loop into the ordered active loop list
					int new_loop_cycle_span = loop.SpanWidth;
					
					int loop_idx = 0;
					foreach(ActionLoopCycle active_loop in m_anim_state_vars.m_active_loop_cycles)
					{
						if(loop.m_start_action_idx == active_loop.m_start_action_idx && loop.m_end_action_idx == active_loop.m_end_action_idx)
						{
							// This loop is already in the active loop list, don't re-add
							loop_idx = -1;
							break;
						}
						
						if(new_loop_cycle_span < active_loop.SpanWidth)
						{
							break;
						}
							
						loop_idx++;
					}
					
					if(loop_idx >= 0)
					{
						m_anim_state_vars.m_active_loop_cycles.Insert(loop_idx, loop.Clone());
					}
				}
			}
		}
		
		public void ContinueAction(float animation_timer, LetterAnimation animation, AnimatePerOptions animate_per)
		{
			if(m_anim_state_vars.m_waiting_to_sync)
			{
				m_anim_state_vars.m_break_delay = 0;
				m_anim_state_vars.m_waiting_to_sync= false;
				
				// reset timer offset to compensate for the sync-up wait time
				m_anim_state_vars.m_timer_offset = animation_timer;
				
				// Progress letter animation index to next, and break out of the loop
				int prev_action_idx = m_anim_state_vars.m_action_index;
				
				// Set next action index
				SetNextActionIndex(animation);
				
				if(m_anim_state_vars.m_active)
				{
					if(!m_anim_state_vars.m_reverse && m_anim_state_vars.m_action_index_progress > m_anim_state_vars.m_action_index)
					{
						// Repeating the action again; check for unqiue random variable requests.
						animation.GetAction(m_anim_state_vars.m_action_index).SoftReset(animation.GetAction(prev_action_idx), m_progression_variables, animate_per);
					}
					
					if(prev_action_idx != m_anim_state_vars.m_action_index)
					{
						UpdateLoopList(animation);
					}
				}
			}		
		}
		
		void SetCurrentLetterAction(LetterAction letter_action, AnimatePerOptions animate_per)
		{	
			m_current_letter_action = letter_action;

			if(letter_action.m_action_type == ACTION_TYPE.ANIM_SEQUENCE)
			{
				m_action_delay = Mathf.Max(m_current_letter_action.m_delay_progression.GetValue(m_progression_variables, m_last_animate_per), 0);
			}
			m_action_duration = Mathf.Max(m_current_letter_action.m_duration_progression.GetValue(m_progression_variables, m_last_animate_per), 0);
			
			// Check if action is in a loopreverse_onetime delay case. If so, set delay to 0.
			if(	m_anim_state_vars.m_active_loop_cycles != null &&
				m_anim_state_vars.m_active_loop_cycles.Count > 0 &&
				m_anim_state_vars.m_active_loop_cycles[0].m_delay_first_only &&
				!m_anim_state_vars.m_active_loop_cycles[0].FirstPass &&
				m_current_letter_action.m_delay_progression.Progression != (int) ValueProgression.Constant)
			{
				if(m_anim_state_vars.m_reverse || !m_current_letter_action.m_force_same_start_time)
				{
					m_action_delay = 0;
				}
			}
		}
		
		// Animates the letter mesh and return the current action index in use
		public LETTER_ANIMATION_STATE AnimateMesh(	bool force_render,
													float timer,
													TextAnchor text_anchor,
													int lowest_action_progress,
													LetterAnimation animation,
													AnimatePerOptions animate_per,
													float delta_time,
													EffectManager effect_manager)
		{
			
			m_last_animate_per = animate_per;
			m_effect_manager_handle = effect_manager;
			
			if(animation.NumActions > 0 && m_anim_state_vars.m_action_index < animation.NumActions)
			{
				if(!m_anim_state_vars.m_active && !force_render)
				{
					return LETTER_ANIMATION_STATE.STOPPED;
				}
				
				if(m_anim_state_vars.m_action_index != m_anim_state_vars.m_prev_action_index)
				{
					SetCurrentLetterAction(animation.GetAction(m_anim_state_vars.m_action_index), animate_per);
					
					m_anim_state_vars.m_started_action = false;
				}
				else if(m_current_letter_action == null)
				{
					SetCurrentLetterAction(animation.GetAction(m_anim_state_vars.m_action_index), animate_per);
				}
				
				m_anim_state_vars.m_prev_action_index = m_anim_state_vars.m_action_index;
				
				if(force_render)
				{
					SetupMesh(m_current_letter_action, m_anim_state_vars.m_action_index > 0 ? animation.GetAction(m_anim_state_vars.m_action_index-1) : null, m_anim_state_vars.m_action_progress, m_progression_variables, animate_per, m_anim_state_vars.m_linear_progress, m_effect_manager_handle);
				}
				
				if(m_anim_state_vars.m_waiting_to_sync)
				{
					if(m_current_letter_action.m_action_type == ACTION_TYPE.BREAK)
					{
						if(!force_render && m_anim_state_vars.m_break_delay > 0)
						{
							m_anim_state_vars.m_break_delay -= delta_time;
							
							if(m_anim_state_vars.m_break_delay <= 0)
							{
								ContinueAction(timer, animation, animate_per);
								
								return LETTER_ANIMATION_STATE.PLAYING;
							}
						}
						
						return LETTER_ANIMATION_STATE.WAITING;
					}
					else if(lowest_action_progress < m_anim_state_vars.m_action_index_progress)
					{
						return LETTER_ANIMATION_STATE.PLAYING;
					}
					else if(!force_render)
					{
						m_anim_state_vars.m_waiting_to_sync = false;
						
						// reset timer offset to compensate for the sync-up wait time
						m_anim_state_vars.m_timer_offset = timer;
					}
				}
				else if(!force_render && (m_current_letter_action.m_action_type == ACTION_TYPE.BREAK || (!m_anim_state_vars.m_reverse && m_current_letter_action.m_force_same_start_time && lowest_action_progress < m_anim_state_vars.m_action_index_progress)))
				{
					// Force letter to wait for rest of letters to be in sync
					m_anim_state_vars.m_waiting_to_sync = true;
					
					m_anim_state_vars.m_break_delay = Mathf.Max(m_current_letter_action.m_duration_progression.GetValue(m_progression_variables, animate_per), 0);
					
					return LETTER_ANIMATION_STATE.PLAYING;
				}
				
				
				if(force_render)
				{
					return m_anim_state_vars.m_active ? LETTER_ANIMATION_STATE.PLAYING : LETTER_ANIMATION_STATE.STOPPED;
				}
				
				m_anim_state_vars.m_action_progress = 0;
				m_anim_state_vars.m_linear_progress = 0;
				
				m_action_timer = timer - m_anim_state_vars.m_timer_offset;
				
				if((m_anim_state_vars.m_reverse || m_action_timer > m_action_delay))
				{
					m_anim_state_vars.m_linear_progress = (m_action_timer - (m_anim_state_vars.m_reverse ? 0 : m_action_delay)) / m_action_duration;
					
					if(m_anim_state_vars.m_reverse)
					{
						if(m_action_timer >= m_action_duration)
						{
							m_anim_state_vars.m_linear_progress = 0;
						}
						else
						{
							m_anim_state_vars.m_linear_progress = 1 - m_anim_state_vars.m_linear_progress;
						}
					}
					
					
					if(!m_anim_state_vars.m_started_action)
					{
						// Trigger any action onStart audio or particle effects
						
						TriggerAudioEffect(animate_per, PLAY_ITEM_EVENTS.ON_START);
						
						TriggerParticleEffects(animate_per, PLAY_ITEM_EVENTS.ON_START);
						
						m_anim_state_vars.m_started_action = true;
					}
					
					
					m_anim_state_vars.m_action_progress = EasingManager.GetEaseProgress(m_current_letter_action.m_ease_type, m_anim_state_vars.m_linear_progress);
					
					if((!m_anim_state_vars.m_reverse && m_anim_state_vars.m_linear_progress >= 1) || (m_anim_state_vars.m_reverse && m_action_timer >= m_action_duration + m_action_delay))
					{
						m_anim_state_vars.m_action_progress = m_anim_state_vars.m_reverse ? 0 : 1;
						m_anim_state_vars.m_linear_progress = m_anim_state_vars.m_reverse ? 0 : 1;
						
						if(!m_anim_state_vars.m_reverse && m_anim_state_vars.m_action_index != -1)
						{
							TriggerParticleEffects(animate_per, PLAY_ITEM_EVENTS.ON_FINISH);
							
							TriggerAudioEffect(animate_per, PLAY_ITEM_EVENTS.ON_FINISH);
						}
						
						int prev_action_idx = m_anim_state_vars.m_action_index;
						float prev_delay = m_action_delay;
						
						// Set next action index
						SetNextActionIndex(animation);
						
						if(m_anim_state_vars.m_active)
						{
							if(!m_anim_state_vars.m_reverse)
							{
								m_anim_state_vars.m_started_action = false;
							}
							
							if(!m_anim_state_vars.m_reverse && m_anim_state_vars.m_action_index_progress > m_anim_state_vars.m_action_index)
							{
								// Repeating the action again; check for unqiue random variable requests.
								animation.GetAction(m_anim_state_vars.m_action_index).SoftReset(animation.GetAction(prev_action_idx), m_progression_variables, animate_per, m_anim_state_vars.m_action_index == 0);
							}
							else if(m_anim_state_vars.m_reverse)
							{
								animation.GetAction(m_anim_state_vars.m_action_index).SoftResetStarts(animation.GetAction(prev_action_idx), m_progression_variables, animate_per);
							}
							
							// Add to the timer offset
							m_anim_state_vars.m_timer_offset += prev_delay + m_action_duration;
							
							if(prev_action_idx != m_anim_state_vars.m_action_index)
							{
								UpdateLoopList(animation);
							}
							else
							{
								SetCurrentLetterAction(animation.GetAction(m_anim_state_vars.m_action_index), animate_per);
							}
						}
					}
				}
				
				SetupMesh(m_current_letter_action, m_anim_state_vars.m_action_index > 0 ? animation.GetAction(m_anim_state_vars.m_action_index-1) : null, m_anim_state_vars.m_action_progress, m_progression_variables, animate_per, m_anim_state_vars.m_linear_progress, m_effect_manager_handle);
			}
			else
			{
				// no actions found for this letter. Position letter in its default position
				if(mesh_verts == null || mesh_verts.Length == 0)
					mesh_verts = new Vector3[4];
				
				for(int idx=0; idx < 4; idx++)
				{
					mesh_verts[idx] = m_base_vertices[idx] + m_base_offset;
				}
				m_mesh.vertices = mesh_verts;
				
				m_anim_state_vars.m_active = false;
			}
			
			return m_anim_state_vars.m_active ? LETTER_ANIMATION_STATE.PLAYING : LETTER_ANIMATION_STATE.STOPPED;
		}
		
		void TriggerAudioEffect(AnimatePerOptions animate_per, PLAY_ITEM_EVENTS play_when)
		{
			if(m_current_letter_action.NumAudioEffectSetups > 0)
			{
				foreach(AudioEffectSetup effect_setup in m_current_letter_action.AudioEffectSetups)
				{
					if(effect_setup.m_play_when == play_when
					   && (effect_setup.m_effect_assignment == PLAY_ITEM_ASSIGNMENT.PER_LETTER || effect_setup.m_effect_assignment_custom_letters.Contains(m_progression_variables.m_letter_value)))
					{
						if(	!effect_setup.m_loop_play_once ||
							m_anim_state_vars.m_active_loop_cycles == null ||
							m_anim_state_vars.m_active_loop_cycles.Count == 0 ||
							m_anim_state_vars.m_active_loop_cycles[0].FirstPass)
						{
							m_effect_manager_handle.PlayAudioClip(effect_setup, m_progression_variables, animate_per);
						}
					}
				}
			}
		}
		
		void TriggerParticleEffects(AnimatePerOptions animate_per, PLAY_ITEM_EVENTS play_when)
		{
			if(m_current_letter_action.NumParticleEffectSetups > 0)
			{
				foreach(ParticleEffectSetup effect_setup in m_current_letter_action.ParticleEffectSetups)
				{
					if(effect_setup.m_play_when == play_when
					   && (effect_setup.m_effect_assignment == PLAY_ITEM_ASSIGNMENT.PER_LETTER || effect_setup.m_effect_assignment_custom_letters.Contains(m_progression_variables.m_letter_value)))
					{
						
						if(	!effect_setup.m_loop_play_once ||
							m_anim_state_vars.m_active_loop_cycles == null ||
							m_anim_state_vars.m_active_loop_cycles.Count == 0 ||
							m_anim_state_vars.m_active_loop_cycles[0].FirstPass)
						{
							m_effect_manager_handle.PlayParticleEffect(
								m_mesh,
								m_flipped,
								effect_setup,
								m_progression_variables,
								animate_per
							);
						}
					}
				}
			}
		}
		
		void SetupMesh(LetterAction letter_action, LetterAction prev_action, float action_progress, AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per, float linear_progress, EffectManager effect_manager)
		{	
			// construct current anchor offset vector
			m_anchor_offset = letter_action.AnchorOffsetStart;
			m_anchor_offset = new Vector3(	m_anchor_offset.x * m_width, 
											letter_action.m_letter_anchor_start == (int) TextfxTextAnchor.BaselineLeft || letter_action.m_letter_anchor_start == (int) TextfxTextAnchor.BaselineCenter || letter_action.m_letter_anchor_start == (int) TextfxTextAnchor.BaselineRight 
												? 0		// zero because letters are based around the baseline already.
												: (effect_manager.IsFontBaseLineSet 
														? (effect_manager.FontBaseLine + m_y_offset) - (m_anchor_offset.y * -m_height)
														: (m_anchor_offset.y * m_height)),	// Legacy effect support when baseline isn't already set.
											0);
			
			if(letter_action.m_letter_anchor_2_way)
			{
				m_anchor_offset = letter_action.AnchorOffsetEnd;
				m_anchor_offset = Vector3.Lerp(	m_anchor_offset,
												new Vector3(
													m_anchor_offset.x * m_width,
													letter_action.m_letter_anchor_end == (int) TextfxTextAnchor.BaselineLeft || letter_action.m_letter_anchor_end == (int) TextfxTextAnchor.BaselineCenter || letter_action.m_letter_anchor_end == (int) TextfxTextAnchor.BaselineRight
														? 0		// zero because letters are based around the baseline already.
														: (effect_manager.IsFontBaseLineSet
																? (effect_manager.FontBaseLine + m_y_offset) - (m_anchor_offset.y * -m_height)
																: (m_anchor_offset.y * m_height)),	// Legacy effect support when baseline isn't already set.
													0),
												action_progress);
			}
			
			
			// Calculate Scale Vector
			from_vec = letter_action.m_start_scale.GetValue(progression_variables, animate_per);
			to_vec = letter_action.m_end_scale.GetValue(progression_variables, animate_per);
			
			if(letter_action.m_scale_axis_ease_data.m_override_default)
			{
				m_letter_scale = new Vector3(	EffectManager.FloatLerp(from_vec.x, to_vec.x, EasingManager.GetEaseProgress(letter_action.m_scale_axis_ease_data.m_x_ease, linear_progress)),
												EffectManager.FloatLerp(from_vec.y, to_vec.y, EasingManager.GetEaseProgress(letter_action.m_scale_axis_ease_data.m_y_ease, linear_progress)),
												EffectManager.FloatLerp(from_vec.z, to_vec.z, EasingManager.GetEaseProgress(letter_action.m_scale_axis_ease_data.m_z_ease, linear_progress)));
			}
			else
			{
				m_letter_scale = EffectManager.Vector3Lerp(
												from_vec,
												to_vec,
												action_progress);
			}
			
			// Calculate Rotation
			from_vec = letter_action.m_start_euler_rotation.GetValue(progression_variables, animate_per);
			to_vec = letter_action.m_end_euler_rotation.GetValue(progression_variables, animate_per);
			
			if(letter_action.m_rotation_axis_ease_data.m_override_default)
			{
				m_letter_rotation =	Quaternion.Euler
									(
										EffectManager.FloatLerp(from_vec.x, to_vec.x, EasingManager.GetEaseProgress(letter_action.m_rotation_axis_ease_data.m_x_ease, linear_progress)),
										EffectManager.FloatLerp(from_vec.y, to_vec.y, EasingManager.GetEaseProgress(letter_action.m_rotation_axis_ease_data.m_y_ease, linear_progress)),
										EffectManager.FloatLerp(from_vec.z, to_vec.z, EasingManager.GetEaseProgress(letter_action.m_rotation_axis_ease_data.m_z_ease, linear_progress))
									);
			}
			else
			{
				m_letter_rotation = Quaternion.Euler(
											EffectManager.Vector3Lerp(
												from_vec,
												to_vec,
												action_progress)
											);
			}
			
			
			// Calculate Position
			if(letter_action.m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX || (letter_action.m_offset_from_last && prev_action != null && prev_action.m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX))
				from_vec = new Vector3(-m_anchor_offset.x,m_base_offset.y,0);
			else if(letter_action.m_start_pos.ForcePositionOverride)
				from_vec = new Vector3(-m_anchor_offset.x,0,0);
			else
				from_vec = BaseOffset;
			
			from_vec += letter_action.m_start_pos.GetValue(progression_variables, animate_per);
			
			if(letter_action.m_end_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX || (letter_action.m_end_pos.IsOffsetFromLast && letter_action.m_start_pos.Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX))
				to_vec = new Vector3(-m_anchor_offset.x,m_base_offset.y,0);
			else if(letter_action.m_end_pos.ForcePositionOverride)
				to_vec = new Vector3(-m_anchor_offset.x,0,0);
			else
				to_vec = BaseOffset;
			
			to_vec += letter_action.m_end_pos.GetValue(progression_variables, animate_per);
			
			if(letter_action.m_position_axis_ease_data.m_override_default)
			{
				m_letter_position = new Vector3(	EffectManager.FloatLerp(from_vec.x, to_vec.x, EasingManager.GetEaseProgress(letter_action.m_position_axis_ease_data.m_x_ease, linear_progress)),
													EffectManager.FloatLerp(from_vec.y, to_vec.y, EasingManager.GetEaseProgress(letter_action.m_position_axis_ease_data.m_y_ease, linear_progress)),
													EffectManager.FloatLerp(from_vec.z, to_vec.z, EasingManager.GetEaseProgress(letter_action.m_position_axis_ease_data.m_z_ease, linear_progress)));
			}
			else
			{
				m_letter_position = EffectManager.Vector3Lerp(
					from_vec, 
					to_vec,
					action_progress);
			}
			
			// Calculate letter center position
			m_letter_center = new Vector3(m_width / 2, m_height/2, 0);
			m_letter_center -= m_anchor_offset;
			m_letter_center = Vector3.Scale(m_letter_center, m_letter_scale);
			m_letter_center = m_letter_rotation	* m_letter_center;
			m_letter_center += m_anchor_offset + m_letter_position;
			
			
			if(mesh_verts == null || mesh_verts.Length == 0)
				mesh_verts = new Vector3[4];
			for(int idx=0; idx < 4; idx++)
			{
				mesh_verts[idx] = m_base_vertices[idx];
				
				// normalise vert position to the anchor point before scaling and rotating.
				mesh_verts[idx] -= m_anchor_offset;
				
				// Scale verts
				mesh_verts[idx] = Vector3.Scale(mesh_verts[idx], m_letter_scale);
				
				// Rotate vert
				mesh_verts[idx] = m_letter_rotation	* mesh_verts[idx];
				
				mesh_verts[idx] += m_anchor_offset;
				
				// translate vert
				mesh_verts[idx] += m_letter_position;
			}
			m_mesh.vertices = mesh_verts;
			
			
			
			// Sort out letters colour
			if(letter_action.m_use_gradient_start)
			{
				start_colour = letter_action.m_start_vertex_colour.GetValue(progression_variables, animate_per);
			}
			else
			{
				start_colour = new VertexColour(letter_action.m_start_colour.GetValue(progression_variables, animate_per));
			}
			
			if(letter_action.m_use_gradient_end)
			{
				end_colour = letter_action.m_end_vertex_colour.GetValue(progression_variables, animate_per);
			}
			else
			{
				end_colour = new VertexColour(letter_action.m_end_colour.GetValue(progression_variables, animate_per));
			}
			
			if(!m_flipped)
			{
				m_mesh.colors = new Color[]{ 
					Color.Lerp(start_colour.top_right, end_colour.top_right, action_progress), 
					Color.Lerp(start_colour.top_left, end_colour.top_left, action_progress), 
					Color.Lerp(start_colour.bottom_left, end_colour.bottom_left, action_progress), 
					Color.Lerp(start_colour.bottom_right, end_colour.bottom_right, action_progress)};
			}
			else
			{
				m_mesh.colors = new Color[]{
					Color.Lerp(start_colour.top_left, end_colour.top_left, action_progress),
					Color.Lerp(start_colour.bottom_left, end_colour.bottom_left, action_progress),
					Color.Lerp(start_colour.bottom_right, end_colour.bottom_right, action_progress),
					Color.Lerp(start_colour.top_right, end_colour.top_right, action_progress)
				};
			}
		}
	}
}