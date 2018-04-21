using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;

namespace TextFx.LegacyContent
{

	public enum LOOP_TYPE
	{
		LOOP,
		LOOP_REVERSE
	}

	[System.Serializable]
	public class ActionLoopCycle
	{
		public int m_start_action_idx = 0;
		public int m_end_action_idx = 0;
		public int m_number_of_loops = 0;
		public LOOP_TYPE m_loop_type = LOOP_TYPE.LOOP;
		public bool m_delay_first_only = false;
		
		bool m_first_pass = true;
		public bool FirstPass { get { return m_first_pass; } set { m_first_pass = value; } }
		
		public ActionLoopCycle(){}
		
		public ActionLoopCycle(int start, int end)
		{
			m_start_action_idx = start;
			m_end_action_idx = end;
		}
		
		public ActionLoopCycle(int start, int end, int num_loops, LOOP_TYPE loop_type)
		{
			m_start_action_idx = start;
			m_end_action_idx = end;
			m_number_of_loops = num_loops;
			m_loop_type = loop_type;
		}
		
		public ActionLoopCycle Clone()
		{
			ActionLoopCycle action_loop = new ActionLoopCycle(m_start_action_idx,m_end_action_idx);
			
			action_loop.m_number_of_loops = m_number_of_loops;
			action_loop.m_loop_type = m_loop_type;
			action_loop.m_delay_first_only = m_delay_first_only;
			
			return action_loop;
		}
		
		public int SpanWidth
		{
			get
			{
				return m_end_action_idx - m_start_action_idx;
			}
		}
		
		public JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			json_data["m_delay_first_only"] = m_delay_first_only;
			json_data["m_end_action_idx"] = m_end_action_idx;
			json_data["m_loop_type"] = (int) m_loop_type;
			json_data["m_number_of_loops"] = m_number_of_loops;
			json_data["m_start_action_idx"] = m_start_action_idx;
			
			return new JSONValue(json_data);
		}
		
		public void ImportData(JSONObject json_data)
		{
			m_delay_first_only = json_data["m_delay_first_only"].Boolean;
			m_end_action_idx = (int) json_data["m_end_action_idx"].Number;
			m_loop_type = (LOOP_TYPE) (int) json_data["m_loop_type"].Number;
			m_number_of_loops = (int) json_data["m_number_of_loops"].Number;
			m_start_action_idx = (int) json_data["m_start_action_idx"].Number;
		}
	}

	[System.Serializable]
	public class LetterAnimation
	{
		const char DELIMITER_CHAR = '|';
		
		[SerializeField]
		List<LetterAction> m_letter_actions = new List<LetterAction>();
		[SerializeField]
		List<ActionLoopCycle> m_loop_cycles = new List<ActionLoopCycle>();
		
		public LETTERS_TO_ANIMATE m_letters_to_animate_option = LETTERS_TO_ANIMATE.ALL_LETTERS;
		public List<int> m_letters_to_animate;
		public int m_letters_to_animate_custom_idx = 1;
		
		public int NumActions { get { return m_letter_actions.Count; } }
		public int NumLoops { get { return m_loop_cycles.Count; } }
		public List<LetterAction> LetterActions { get { return m_letter_actions; } }
		public List<ActionLoopCycle> ActionLoopCycles { get { return m_loop_cycles; } }
		
		LETTER_ANIMATION_STATE m_animation_state = LETTER_ANIMATION_STATE.PLAYING;
		public LETTER_ANIMATION_STATE CurrentAnimationState { get { return m_animation_state; } set { m_animation_state = value; } }
		
		public void AddAction(LetterAction letter_action)
		{
			if(m_letter_actions == null)
				m_letter_actions = new List<LetterAction>();
			
			m_letter_actions.Add(letter_action);
		}
		
		public void AddAction()
		{
			if(m_letter_actions == null)
				m_letter_actions = new List<LetterAction>();
			
			m_letter_actions.Add(new LetterAction());
		}
		
		public void InsertAction(int index, LetterAction action)
		{
			if(index >= 0 && index <= m_letter_actions.Count)
				m_letter_actions.Insert(index, action);
		}
		
		public void RemoveAction(int index)
		{
			if(m_letter_actions != null && index >= 0 && index < m_letter_actions.Count)
				m_letter_actions.RemoveAt(index);
		}
		
		public void RemoveActions(int index, int count)
		{
			if(m_letter_actions != null && index >= 0 && index + count < m_letter_actions.Count)
				m_letter_actions.RemoveRange(index, count);
		}
		
		public LetterAction GetAction(int index)
		{
			if(m_letter_actions != null && index >= 0 && index < m_letter_actions.Count)
				return m_letter_actions[index];
			else
				return null;
		}
		
		public void AddLoop()
		{
			if(m_loop_cycles == null)
				m_loop_cycles = new List<ActionLoopCycle>();
			
			m_loop_cycles.Add(new ActionLoopCycle());
		}
		
		public void RemoveLoop(int index)
		{
			if(m_loop_cycles != null && index >= 0 && index < m_loop_cycles.Count)
				m_loop_cycles.RemoveAt(index);
		}
		
		public void RemoveLoops(int index, int count)
		{
			if(m_loop_cycles != null && index >= 0 && index + count < m_loop_cycles.Count)
				m_loop_cycles.RemoveRange(index, count);
		}
		
		public ActionLoopCycle GetLoop(int index)
		{
			if(m_loop_cycles != null && index >= 0 && index < m_loop_cycles.Count)
				return m_loop_cycles[index];
			else
				return null;
		}
		
		
		public void AddLoop(int start_idx, int end_idx, bool change_type)
		{
			bool valid_loop_addition = true;
			int insert_at_idx = 0;
			
			if(end_idx >= start_idx && start_idx >= 0 && start_idx < m_letter_actions.Count && end_idx >= 0 && end_idx < m_letter_actions.Count)
			{
				int new_loop_width = end_idx - start_idx;
				int count = 1;
				foreach(ActionLoopCycle loop in m_loop_cycles)
				{
					if((start_idx < loop.m_start_action_idx && (end_idx >loop.m_start_action_idx && end_idx < loop.m_end_action_idx))
						|| (end_idx > loop.m_end_action_idx && (start_idx > loop.m_start_action_idx && start_idx < loop.m_end_action_idx)))
					{
						// invalid loop
						valid_loop_addition = false;
						Debug.LogWarning("Invalid Loop Added: Loops can not intersect other loops.");
						break;
					}
					else if(start_idx == loop.m_start_action_idx && end_idx == loop.m_end_action_idx)
					{
						// Entry already exists, so either add to it, or change its type
						valid_loop_addition = false;
						if(change_type)
						{
							loop.m_loop_type = loop.m_loop_type == LOOP_TYPE.LOOP ? LOOP_TYPE.LOOP_REVERSE : LOOP_TYPE.LOOP;
						}
						else
						{
							loop.m_number_of_loops ++;
						}
						break;
					}
					else
					{
						if(new_loop_width >= loop.SpanWidth)
						{
							insert_at_idx = count;
						}
					}
							
					count++;
				}
			}
			else
			{
				valid_loop_addition = false;
				Debug.LogWarning("Invalid Loop Added: Check that start/end index are in bounds.");
			}
			
			
			if(valid_loop_addition)
			{
				m_loop_cycles.Insert(insert_at_idx, new ActionLoopCycle(start_idx, end_idx));
			}
		}
		
		public void PrepareData(LetterSetup[] letters, int num_words, int num_lines, AnimatePerOptions animate_per)
		{
			if(letters == null || letters.Length == 0)
			{
				return;
			}
			
			int num_letters = letters.Length;
			
			// Populate list of letters to animate by index, and set Letter indexes accordingly
			if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.ALL_LETTERS)
			{
				m_letters_to_animate = new List<int>();
				for(int letter_idx=0; letter_idx < num_letters; letter_idx++)
				{
					m_letters_to_animate.Add(letter_idx);
					
					letters[letter_idx].m_progression_variables.m_letter_value = letter_idx;
				}
			}
			else if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LETTER || m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER)
			{
				m_letters_to_animate = new List<int>();
				m_letters_to_animate.Add(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LETTER ? 0 : letters.Length -1);
				
				letters[m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LETTER ? 0 : letters.Length - 1].m_progression_variables.m_letter_value = 0;
			}
			else if(m_letters_to_animate_option != LETTERS_TO_ANIMATE.CUSTOM)
			{
				m_letters_to_animate = new List<int>();
				
				int line_idx = m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_LINES ? 0 : -1;
				int word_idx = m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_WORDS ? 0 : -1;
				int target_idx = 0;
				
				if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_WORD)
					target_idx = letters[letters.Length-1].m_progression_variables.m_word_value;
				else if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LINE)
					target_idx = letters[letters.Length-1].m_progression_variables.m_line_value;
				else if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.NTH_WORD || m_letters_to_animate_option == LETTERS_TO_ANIMATE.NTH_LINE)
					target_idx = m_letters_to_animate_custom_idx - 1;
				
				int letter_idx = 0;
				int progression_idx = 0;
				foreach(LetterSetup letter in letters)
				{
					if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LINE || m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LINE || m_letters_to_animate_option == LETTERS_TO_ANIMATE.NTH_LINE)
					{
						if(letter.m_progression_variables.m_line_value == target_idx)
						{
							letter.m_progression_variables.m_letter_value = progression_idx;
							m_letters_to_animate.Add(letter_idx);
							progression_idx ++;
						}
					}
					else if(letter.m_progression_variables.m_line_value > line_idx)
					{
						if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LETTER_LINES)
						{
							letter.m_progression_variables.m_letter_value = progression_idx;
							m_letters_to_animate.Add(letter_idx);
							progression_idx ++;
							
						}
						else if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_LINES)
						{
							letter.m_progression_variables.m_letter_value = progression_idx - 1;
							m_letters_to_animate.Add(letter_idx - 1);
							progression_idx ++;
						}
						line_idx = letter.m_progression_variables.m_line_value;
					}
					
					if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_WORD || m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_WORD || m_letters_to_animate_option == LETTERS_TO_ANIMATE.NTH_WORD)
					{
						if(letter.m_progression_variables.m_word_value == target_idx)
						{
							letter.m_progression_variables.m_letter_value = progression_idx;
							m_letters_to_animate.Add(letter_idx);
							progression_idx ++;
						}
					}
					else if(letter.m_progression_variables.m_word_value > word_idx)
					{
						if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.FIRST_LETTER_WORDS)
						{
							letter.m_progression_variables.m_letter_value = progression_idx;
							m_letters_to_animate.Add(letter_idx);
							progression_idx ++;
						}
						else if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_WORDS)
						{
							letter.m_progression_variables.m_letter_value = progression_idx;
							m_letters_to_animate.Add(letter_idx - 1);
							progression_idx ++;
						}
						word_idx = letter.m_progression_variables.m_word_value;
					}
					
					letter_idx++;
				}
				
				if(m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_WORDS || m_letters_to_animate_option == LETTERS_TO_ANIMATE.LAST_LETTER_LINES)
				{
					letters[num_letters - 1].m_progression_variables.m_letter_value = letter_idx - 1;
					m_letters_to_animate.Add(letter_idx - 1);
				}
			}
			else
			{
				int progression_idx = 0;
				for(int letter_idx=0; letter_idx < num_letters; letter_idx++)
				{	
					if(m_letters_to_animate.Contains(letter_idx))
					{
						letters[letter_idx].m_progression_variables.m_letter_value = progression_idx;
						
						progression_idx ++;
					}
				}
			}
			
			// Prepare data progression data in all actions
			LetterAction letter_action;
			LetterAction prev_action = null;
			bool prev_action_end_state = true;
			for(int action_idx = 0; action_idx < m_letter_actions.Count; action_idx ++)
			{
				letter_action = m_letter_actions[action_idx];
				
				letter_action.PrepareData(ref letters, m_letters_to_animate.Count, num_words, num_lines, prev_action, animate_per, prev_action_end_state);
				
				
				if(letter_action.m_action_type == ACTION_TYPE.ANIM_SEQUENCE)
				{
					// Set default previous action settings
					prev_action_end_state = true;
					prev_action = letter_action;
				}
				
				// Check for reverse loops, and how the animation should progress from there
				foreach(ActionLoopCycle loop_cycle in m_loop_cycles)
				{
					if(loop_cycle.m_end_action_idx == action_idx && loop_cycle.m_loop_type == LOOP_TYPE.LOOP_REVERSE)
					{
						prev_action = m_letter_actions[loop_cycle.m_start_action_idx];
						prev_action_end_state = false;
					}
				}
			}
		}

		public JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			json_data["m_letters_to_animate"] = m_letters_to_animate.ExportData();
			json_data["m_letters_to_animate_custom_idx"] = m_letters_to_animate_custom_idx;
			json_data["m_letters_to_animate_option"] = (int) m_letters_to_animate_option;
			
			if(m_loop_cycles.Count > 0)
			{
				JSONArray loops_data = new JSONArray();
				
				foreach(ActionLoopCycle action_loop in m_loop_cycles)
				{
					loops_data.Add(action_loop.ExportData());
				}
				
				json_data["LOOPS_DATA"] = loops_data;
			}
			
			JSONArray actions_data = new JSONArray();
			foreach(LetterAction action in m_letter_actions)
			{
				actions_data.Add(action.ExportData());
			}
			json_data["ACTIONS_DATA"] = actions_data;
			
			return new JSONValue(json_data);
		}

		public void ImportData(JSONObject json_data)
		{
			
			m_letters_to_animate = json_data["m_letters_to_animate"].Array.JSONtoListInt();
			m_letters_to_animate_custom_idx = (int) json_data["m_letters_to_animate_custom_idx"].Number;
			m_letters_to_animate_option = (LETTERS_TO_ANIMATE) (int) json_data["m_letters_to_animate_option"].Number;
			
			
			m_loop_cycles = new List<ActionLoopCycle>();
			
			if(json_data.ContainsKey("LOOPS_DATA"))
			{
				ActionLoopCycle loop_cycle;
				
				foreach(JSONValue loop_data in json_data["LOOPS_DATA"].Array)
				{
					loop_cycle = new ActionLoopCycle();
					loop_cycle.ImportData(loop_data.Obj);
					m_loop_cycles.Add(loop_cycle);
				}
			}
			
			m_letter_actions = new List<LetterAction>();
			LetterAction letter_action;
			foreach(JSONValue action_data in json_data["ACTIONS_DATA"].Array)
			{
				letter_action = new LetterAction();
				letter_action.ImportData(action_data.Obj);
				m_letter_actions.Add(letter_action);
			}
		}
	}
}