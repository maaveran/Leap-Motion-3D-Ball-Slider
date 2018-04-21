using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TextFx
{

	[System.Serializable]
	public class TextFxAnimationManager
	{
#if UNITY_EDITOR
		[System.Serializable]
		public class PresetAnimationSection
		{
			public List<PresetEffectSetting> m_preset_effect_settings;
			public bool m_active = false;
			public int m_start_action = 0;
			public int m_num_actions = 0;
			public int m_start_loop = 0;
			public int m_num_loops = 0;
			public bool m_exit_pause = false;
			public float m_exit_pause_duration = 1;
			public bool m_repeat = false;
			public int m_repeat_count = 0;

			public int ExitPauseIndex { get { return m_start_action + m_num_actions; } }

			public void Reset()
			{
				m_preset_effect_settings = new List<PresetEffectSetting> ();
				m_start_action = 0;
				m_num_actions = 0;
				m_start_loop = 0;
				m_num_loops = 0;
				m_exit_pause = false;
				m_exit_pause_duration = 1;
				m_repeat = false;
				m_repeat_count = 0;
				m_active = false;
			}
		}

		public enum PRESET_ANIMATION_SECTION
		{
			INTRO,
			MAIN,
			OUTRO,
			MAIN_OUTRO
		}

		public static string[] m_animation_section_names = new string[]{"Intro", "Main", "Outro", "Main -> Outro"};
		public static string[] m_animation_section_folders = new string[]{"Intros", "Mains", "Outros", "MainOutros"};
#endif

		public interface GuiTextDataHandler
		{
			int NumVerts { get; }
			Vector3 GetVertPosition(int vertIndex);
			Color GetVertColour(int vertIndex);
		}

		const int JSON_EXPORTER_VERSION = 1;

		public List<LetterAnimation> m_master_animations;

		public bool m_begin_on_start = false;
		public ON_FINISH_ACTION m_on_finish_action = ON_FINISH_ACTION.NONE;
		public float m_animation_speed_factor = 1;
		public float m_begin_delay = 0;
		public AnimatePerOptions m_animate_per = AnimatePerOptions.LETTER;
		public AnimationTime m_time_type = AnimationTime.GAME_TIME;
		
		[SerializeField]
		LetterSetup[] m_letters;
		[SerializeField]
		List<AudioSource> m_audio_sources;			// List of AudioSources used for sound effects
		[SerializeField]
		List<ParticleEmitter> m_particle_emitters;
		[SerializeField]
		List<ParticleSystem> m_particle_systems;
		[SerializeField]
		List<ParticleEffectInstanceManager> m_particle_effect_managers;
		[SerializeField]
		GameObject m_gameObect;
		[SerializeField]
		Transform m_transform;
		[SerializeField]
		TextFxAnimationInterface m_animation_interface_reference;
		[SerializeField]
		MonoBehaviour m_monobehaviour;
		[SerializeField]
		int m_num_meshes = 0;
		[SerializeField]
		int[] m_colourVertMapping;

		Vector3[] m_current_mesh_verts;
		Color[] m_current_mesh_colours;
		float m_last_time = 0;
		float m_animation_timer = 0;
		int m_lowest_action_progress = 0;
		float m_runtime_animation_speed_factor = 1;		// Set by continue anim speed override option
		bool m_running = false;
		bool m_paused = false;
		System.Action m_animation_callback = null;	// Callback called after animation has finished
		ANIMATION_DATA_TYPE m_what_just_changed = ANIMATION_DATA_TYPE.NONE;		// A record of the last thing to have been updated in this LetterAction
		System.Action<int> m_animation_continue_callback = null;

#if UNITY_EDITOR

		// Editor only variables
		int m_editor_action_idx = 0;
		float m_editor_action_progress = 0;
		
		public int EditorActionIdx { get { return m_editor_action_idx; } }
		public float EditorActionProgress { get { return m_editor_action_progress; } }

		public List<PresetEffectSetting> m_preset_effect_settings;

		public PresetAnimationSection m_preset_intro;
		public PresetAnimationSection m_preset_main;
		public PresetAnimationSection m_preset_outro;

		public int IntroRepeatLoopStartIndex { get { return m_preset_outro.m_start_loop + m_preset_outro.m_num_loops; } }
		public int MainRepeatLoopStartIndex { get { return IntroRepeatLoopStartIndex + (m_preset_intro.m_active && m_preset_intro.m_repeat ? 1  : 0); } }
		public int OutroRepeatLoopStartIndex { get { return MainRepeatLoopStartIndex + (m_preset_main.m_active && m_preset_main.m_repeat ? 1  : 0); } }
		public int GlobalRepeatLoopStartIndex { get { return OutroRepeatLoopStartIndex + (m_preset_outro.m_active && m_preset_outro.m_repeat ? 1  : 0); } }

		public bool m_using_quick_setup = false;
		public int m_selected_intro_animation_idx;
		public int m_selected_main_animation_idx;
		public int m_selected_outro_animation_idx;
		public bool m_intro_animation_foldout = false;
		public bool m_main_animation_foldout = false;
		public bool m_outro_animation_foldout = false;

		public bool m_repeat_all_sections = false;
		public int m_repeat_all_sections_count = 0;

		public string m_effect_name = "";		// A human readible name given to the effect when saved.
		public bool m_import_as_section = false;

		public bool WipeQuickSetupData(bool user_confirm = false)
		{
			if (!m_using_quick_setup)
				// quick setup data already wiped
				return true;

			if(user_confirm && m_using_quick_setup && !EditorUtility.DisplayDialog("Sure?", "This will break your current Quick Setup pairing, do you still want to continue?", "Yes", "No"))
			{
				return false;
			}

			m_preset_intro.Reset();
			m_preset_main.Reset();
			m_preset_outro.Reset();
			m_selected_intro_animation_idx = 0;
			m_selected_main_animation_idx = 0;
			m_selected_outro_animation_idx = 0;
			m_intro_animation_foldout = false;
			m_main_animation_foldout = false;
			m_outro_animation_foldout = false;
			m_using_quick_setup = false;
			
			Debug.Log("Wipe all Quick Setup data");

			return true;
		}

		public bool WipeFullEditorData(bool user_confirm = false)
		{
			if(m_using_quick_setup)
				// Already wiped Full Editor data
				return true;

			if(user_confirm &&
			   !m_using_quick_setup &&
			   m_master_animations != null &&
			   m_master_animations.Count > 0 &&
			   m_master_animations[0].NumActions > 0 &&
			   !EditorUtility.DisplayDialog("Sure?", "This will delete your existing animation setup in the Full Editor, do you still want to continue?", "Yes", "No"))
			{
				return false;
			}

			m_master_animations = new List<LetterAnimation> () {new LetterAnimation()};
			m_using_quick_setup = true;

			return true;
		}
#endif
		
		public TextFxAnimationInterface AnimationInterface { get { return m_animation_interface_reference; } }
		public Transform Transform { get { return m_transform; } }
		public GameObject GameObject { get { return m_gameObect; } }
		public Vector3[] MeshVerts { get { return m_current_mesh_verts; } }
		public Color[] MeshColours { get { return m_current_mesh_colours; } }
		public int NumAnimations { get { return m_master_animations == null ? 0 : m_master_animations.Count; } }
		public ANIMATION_DATA_TYPE WhatJustChanged { get { return m_what_just_changed; } set { m_what_just_changed = value; } }
		public List<LetterAnimation> LetterAnimations { get { 	if(m_master_animations == null)
				m_master_animations = new List<LetterAnimation>();
				return m_master_animations; } }
		public bool HasAudioParticleChildInstances { get { return (m_audio_sources != null && m_particle_emitters != null && m_particle_systems != null)
				&& (m_audio_sources.Count > 0 || m_particle_emitters.Count > 0 || m_particle_systems.Count > 0); } }
		public List<ParticleEffectInstanceManager> ParticleEffectManagers { get { return m_particle_effect_managers; } }

		public Vector3 Position { get { return m_transform.position; } }
		public Vector3 Scale { get { return m_transform.localScale; } }

		public float AnimationTimer { get { return m_animation_timer; } }

		public bool Playing { get { return m_running; } }
		public bool Paused
		{
			get
			{
				return m_paused;
			}
			set
			{
				m_paused = value;
				
				if(!m_paused && m_time_type == AnimationTime.REAL_TIME)
				{
					m_last_time = Time.realtimeSinceStartup;
				}
				
				PauseAllParticleEffects(m_paused);
			}
		}

		public TextFxAnimationManager()
		{
			m_colourVertMapping = new int[]{0,1,2,3};

			Init ();
		}

		public TextFxAnimationManager(int[] customColourVertMapping)
		{
			m_colourVertMapping = customColourVertMapping;

			Init ();
		}

		void Init()
		{
			if(m_master_animations == null)
				m_master_animations = new List<LetterAnimation>();

#if UNITY_EDITOR
			if(m_preset_intro == null)
			{
				// Initialise all anim section objects
				m_preset_intro = new PresetAnimationSection();
				m_preset_main = new PresetAnimationSection();
				m_preset_outro = new PresetAnimationSection();
			}
#endif
		}

		public void OnStart()
		{
			// Force text into animation starting state
			if(m_master_animations != null && m_master_animations.Count > 0)
			{
				SetAnimationState(0, 0, false, ANIMATION_DATA_TYPE.NONE, update_mesh: true);
			}

			if(m_begin_on_start)
			{
				PlayAnimation(m_begin_delay);
			}
		}

		public void SetParentObjectReferences(GameObject gameObject, Transform transform, TextFxAnimationInterface anim_interface)
		{
			m_gameObect = gameObject;
			m_transform = transform;
			m_animation_interface_reference = anim_interface;
			m_monobehaviour = (MonoBehaviour) anim_interface;

			Init ();
		}

		// Sets up LetterSetup instances for each letter in the current text
		public void UpdateText(string text_string, GuiTextDataHandler textData, bool white_space_meshes)
		{
			int numVerts = textData.NumVerts;

			m_num_meshes = numVerts / 4;

			List<LetterSetup> letter_list = new List<LetterSetup> ();
			LetterSetup new_letter_setup;
			char current_char;
			int letter_index = 0;
			int word_idx = 0;
			int line_idx = 0;
			bool visible_character;
			bool last_quad_visible = false;

	//		Debug.Log ("UpdateText, m_num_meshes: " + m_num_meshes + ", num_letters : " + num_letter_setups);

			for (int char_idx = 0; char_idx < text_string.Length; char_idx++)
			{
				current_char = text_string[char_idx];

				visible_character = ! ( current_char.Equals(' ') || current_char.Equals('\n') || current_char.Equals('\r') || current_char.Equals('\t'));

	//			Debug.Log("char '" + current_char + "' , visible : " + visible_character);

				if(current_char.Equals('\n'))
					line_idx++;

				if(last_quad_visible && !visible_character)
					word_idx++;

				last_quad_visible = visible_character;


				// Reuse an existing letter mesh or create a new one if none available
				if(m_letters != null && char_idx < m_letters.Length)
					new_letter_setup = m_letters[char_idx];
				else
					new_letter_setup = new LetterSetup(this);


				new_letter_setup.SetWordLineIndex(word_idx, line_idx);


				if(!white_space_meshes && !visible_character)
				{
					// Creating a white space character LetterSetup stub
					new_letter_setup.SetAsStubInstance();
				}
				else
				{
					if(letter_index * 4 >= numVerts)
					{
						// No mesh available for this letter; which can be caused by it being clipped off
						break;
					}

					new_letter_setup.SetLetterData(	new Vector3[] { textData.GetVertPosition(letter_index * 4), textData.GetVertPosition(letter_index * 4 + 1), textData.GetVertPosition(letter_index * 4 + 2), textData.GetVertPosition(letter_index * 4 + 3)},
													new Color[] { textData.GetVertColour(letter_index*4), textData.GetVertColour(letter_index*4 + 1), textData.GetVertColour(letter_index*4 + 2), textData.GetVertColour(letter_index*4 + 3)},
													letter_index);
					letter_index++;
				}

				// Set whether letter is a visible character or not
				new_letter_setup.VisibleCharacter = visible_character;

				letter_list.Add(new_letter_setup);
			}

			m_letters = letter_list.ToArray ();

			// Calculate action progression values
			PrepareAnimationData();

	//		Debug.LogError ("UpdateText () verts.length : " + verts.Length + ", m_letters.Length : " + m_letters.Length);
		}

		// Calculates values for all animation state progressions using current field values.
		public void PrepareAnimationData(ANIMATION_DATA_TYPE what_to_update = ANIMATION_DATA_TYPE.ALL)
		{
//			Debug.Log ("PrepareAnimationData () " + what_to_update);

			if(m_master_animations != null)
			{
				foreach(LetterAnimation animation in m_master_animations)
				{
					animation.PrepareData(this, m_letters, what_to_update, 1, 1, m_animate_per);	// Requires info about number letters, words, lines etc
				}

				if(Playing)
				{
					m_what_just_changed = what_to_update;
				}
			}
		}


		public void PlayAnimation(System.Action animation_callback)
		{
			m_animation_callback = animation_callback;
			
			PlayAnimation();
		}
		
		public void PlayAnimation(float delay, System.Action animation_callback)
		{
			m_animation_callback = animation_callback;
			
			PlayAnimation(delay);
		}

		public void PlayAnimation(float delay = 0, int starting_action_index = 0)
		{
			if(m_master_animations == null || m_master_animations.Count == 0)
			{
				Debug.LogWarning("PlayAnimation() called, but no animations defined on this EffectManager instance");
				return;
			}
			
			int num_letters = m_letters.Length;

			m_audio_sources = new List<AudioSource>(m_gameObect.GetComponentsInChildren<AudioSource>());
			m_particle_emitters = new List<ParticleEmitter>(m_gameObect.GetComponentsInChildren<ParticleEmitter>());
			m_particle_systems = new List<ParticleSystem>(m_gameObect.GetComponentsInChildren<ParticleSystem>());
			m_particle_effect_managers = new List<ParticleEffectInstanceManager>();
			
			// Stop all audio sources and particle effects
			foreach(AudioSource a_source in m_audio_sources)
			{
				a_source.Stop();
			}
			
			foreach(ParticleEmitter p_emitter in m_particle_emitters)
			{
				p_emitter.emit = false;
				p_emitter.particles = null;
				p_emitter.enabled = false;
			}
			
			foreach(ParticleSystem p_system in m_particle_systems)
			{
				p_system.Stop();
				p_system.Clear();
			}
			
			// Prepare Master Animations data and check for particle effect onstart reset
			bool reset_mesh = false;
			foreach(LetterAnimation animation in m_master_animations)
			{
				animation.CurrentAnimationState = LETTER_ANIMATION_STATE.PLAYING;

				foreach(int letter_idx in animation.m_letters_to_animate)
				{
					if(letter_idx < num_letters)
					{
						m_letters[letter_idx].Reset(animation, starting_action_index);
						m_letters[letter_idx].Active = true;
					}
				}

				// Force letter start positions reset before playing, to avoid onStart particle effect positioning errors
				if(!reset_mesh && animation.NumActions > 0 && animation.GetAction(starting_action_index).NumParticleEffectSetups > 0 )
				{
					foreach(ParticleEffectSetup effect_setup in animation.GetAction(starting_action_index).ParticleEffectSetups)
					{
						if(effect_setup.m_play_when == PLAY_ITEM_EVENTS.ON_START)
						{
							UpdateMesh(false, true, starting_action_index, 0);
							
							reset_mesh = true;
						}
					}
				}
			}
			
			m_lowest_action_progress = 0;
			m_animation_timer = 0;
			m_runtime_animation_speed_factor = 1;
			m_animation_continue_callback = null;
			
			if(delay > 0)
			{
				m_monobehaviour.StartCoroutine(PlayAnimationAfterDelay(delay));
			}
			else
			{
				if(m_time_type == AnimationTime.REAL_TIME)
				{
					m_last_time = Time.realtimeSinceStartup;
				}
				
				m_running = true;
				m_paused = false;
			}
		}

		IEnumerator PlayAnimationAfterDelay(float delay)
		{
			yield return m_monobehaviour.StartCoroutine(TimeDelay(delay, m_time_type));
			
			if(m_time_type == AnimationTime.REAL_TIME)
			{
				m_last_time = Time.realtimeSinceStartup;
			}
			
			m_running = true;
			m_paused = false;
		}
		
		// Reset animation to starting state
		public void ResetAnimation()
		{
			UpdateMesh(false, true, 0, 0);
			
			foreach(LetterSetup letter in m_letters)
			{
				letter.AnimStateVars.Reset();
			}
			
			m_running = false;
			m_paused = false;
			m_lowest_action_progress = 0;
			m_animation_timer = 0;
			m_runtime_animation_speed_factor = 1;
			m_animation_continue_callback = null;
			
			StopAllParticleEffects(true);
		}
		
		// Set Text Effect to its end state
		public void SetEndState()
		{
			m_running = false;
			m_paused = false;
			
			if(m_master_animations == null)
				return;
			
			int longest_action_list = 0;
			
			foreach(LetterAnimation animation in m_master_animations)
			{
				if(animation.NumActions > longest_action_list)
				{
					longest_action_list = animation.NumActions;
				}
			}
			
			SetAnimationState(longest_action_list-1, 1);
		}


		public bool UpdateAnimation()
		{
			float delta_time = m_time_type == AnimationTime.GAME_TIME && Application.isPlaying ? Time.deltaTime : Time.realtimeSinceStartup - m_last_time;

			// Catch massive delta_time values caused by pauses/breaks in playback, resulting in an old m_last_time being used.
			if(!Application.isPlaying && delta_time > 0.25f)
			{
				delta_time = 1f / 30f;
			}

			if (m_time_type == AnimationTime.REAL_TIME || !Application.isPlaying)
			{
				m_last_time = Time.realtimeSinceStartup;
			}

			// Adjust by animation speed factor value
			delta_time *= (m_runtime_animation_speed_factor * m_animation_speed_factor);

			m_animation_timer += delta_time;
			
			if(m_running && UpdateMesh(true, false, 0,0, delta_time))
			{
				m_running = false;

				// Call to the animation-complete callback if assigned
				if(m_animation_callback != null)
				{
					m_animation_callback();
				}
				
				// Execute on finish action requested
				if(Application.isPlaying)
				{
					if(m_on_finish_action == ON_FINISH_ACTION.DESTROY_OBJECT)
					{
						GameObject.Destroy(m_gameObect);
					}
					else if(m_on_finish_action == ON_FINISH_ACTION.DISABLE_OBJECT)
					{
	#if !UNITY_3_5
						m_gameObect.SetActive(false);
	#else
						m_gameObect.SetActiveRecursively(false);
	#endif
					}
					else if(m_on_finish_action == ON_FINISH_ACTION.RESET_ANIMATION)
					{
						ResetAnimation();
					}
				}
			}
			
			if(m_particle_effect_managers.Count > 0)
			{
				for(int idx=0; idx < m_particle_effect_managers.Count; idx++)
				{
					if(m_particle_effect_managers[idx].Update(delta_time))
					{
						// particle effect instance is complete
						// Remove from list
						
						m_particle_effect_managers.RemoveAt(idx);
						idx --;
					}
				}
			}
			
			return m_running;
		}


		
		public void SetAnimationState(int action_idx, float action_progress, bool update_action_values = false, ANIMATION_DATA_TYPE edited_data = ANIMATION_DATA_TYPE.ALL, bool update_mesh = false)
		{
			if(update_action_values)
			{
				// Calculate action progression values
				PrepareAnimationData(edited_data);
			}

			UpdateMesh(false, true, action_idx, action_progress);

			if(update_mesh)
			{
				m_animation_interface_reference.UpdateTextFxMesh( MeshVerts, MeshColours);
			}
		}

		// Continue all animations
		[System.Obsolete("ContinueAnimation has been deprecated. Please use ContinuePastBreak() and ContinuePastLoop()")]
		public void ContinueAnimation()
		{
			ContinueAnimation(-1);
		}

		// Continue specific animation with given index
		[System.Obsolete("ContinueAnimation has been deprecated. Please use ContinuePastBreak() and ContinuePastLoop()")]
		public void ContinueAnimation(int animation_index)
		{
			if(m_master_animations == null)
				return;
			
			if(animation_index >= 0)
			{
				// Animation index specified
				ContinuePastLoop(animation_index);
			}
			else
			{
				// Continue all animations
				for(int anim_idx=0; anim_idx < m_master_animations.Count; anim_idx++)
				{
					ContinuePastLoop(anim_idx);
				}
			}
		}

		// Continues current animation past any WAITING states
		public void ContinuePastBreak(bool onlyIfAllLettersWaiting = false)
		{
			ContinuePastBreak (0, onlyIfAllLettersWaiting);
		}

		public void ContinuePastBreak(int animationIndex, bool onlyIfAllLettersWaiting = false)
		{
			LetterAnimation animation = m_master_animations[animationIndex];

			if(onlyIfAllLettersWaiting && animation.CurrentAnimationState != LETTER_ANIMATION_STATE.WAITING && animation.CurrentAnimationState != LETTER_ANIMATION_STATE.WAITING_INFINITE)
				// Not all letters are in a waiting state at a BREAK action, so don't continue
				return;

			// Continue each waiting letter
			foreach(int letter_idx in animation.m_letters_to_animate)
			{
				// letter is in a waiting state. Continue it beyond this wait state.
				if(m_letters[letter_idx].CurrentAnimationState == LETTER_ANIMATION_STATE.WAITING || m_letters[letter_idx].CurrentAnimationState == LETTER_ANIMATION_STATE.WAITING_INFINITE)
					m_letters[letter_idx].ContinueAction(m_animation_timer, animation, m_animate_per);
			}
			
			return;

//			ContinuePastLoop(ContinueType.EndOfLoop, 
		}


		/// <summary>Continues the current animation out of a current loop</summary>
		/// <param name="continueType">Denotes the action to be taken in order to continue the animation.</param>
		/// <param name="lerpSyncDuration"> The duration of the Instant continue lerp into the next state. Ignored if <paramref name="continueType"/> is set to EndOfLoop</param>
		/// <param name="passNextInfiniteLoop"> If muliple loops are currently active, then the next Infinite loop will be the loop that is continued</para>
		/// <param name="trimInterimLoops"> Any loop within the infinite loop that is being skipped, will be set to it's last iteration </param>
		/// <param name="animationSpeedOverride"> If continuing once all letters are at the end of the loop, this can be used to override the animation speed during this period. Default value is 1. </param>
		public void ContinuePastLoop(ContinueType continueType = ContinueType.EndOfLoop,
		                             float lerpSyncDuration = 0.5f,
		                             bool passNextInfiniteLoop = true,
		                             bool trimInterimLoops = true,
		                             float animationSpeedOverride = 1,
		                             System.Action<int> finishedCallback = null)
		{
			ContinuePastLoop (0, continueType, lerpSyncDuration, passNextInfiniteLoop, trimInterimLoops, animationSpeedOverride, finishedCallback);
		}


		/// <summary>Continues the current animation out of a current loop</summary>
		/// <param name="animation_index">Index of the animation to continue. Most the time this is 0</param>
		/// <param name="continueType">Denotes the action to be taken in order to continue the animation.</param>
		/// <param name="lerpSyncDuration"> The duration of the Instant continue lerp into the next state. Ignored if <paramref name="continueType"/> is set to EndOfLoop</param>
		/// <param name="passNextInfiniteLoop"> If muliple loops are currently active, then the next Infinite loop will be the loop that is continued</para>
		/// <param name="trimInterimLoops"> Any loop within the infinite loop that is being skipped, will be set to it's last iteration </param>
		/// <param name="animationSpeedOverride"> If continuing once all letters are at the end of the loop, this can be used to override the animation speed during this period. Default value is 1. </param>
		public void ContinuePastLoop(int animation_index,
		                             ContinueType continueType = ContinueType.EndOfLoop,
		                             float lerpSyncDuration = 0.5f,
		                             bool passNextInfiniteLoop = true,
		                             bool trimInterimLoops = true,
		                             float animationSpeedOverride = 1,
		                             System.Action<int> finishedCallback = null)
		{
			LetterAnimation animation = m_master_animations[animation_index];

			int deepestLoopDepth = -1;
			ActionLoopCycle deepestLoopCycle = null;
			LetterSetup deepestLoopLetter = null;
			int furthestActionIndex = -1;
			int furthestActionIndexProgress = -1;
			int[] lowestActiveLoopIterations = new int[animation.ActionLoopCycles.Count];
			bool allLettersHaveLoops = true;

			LetterSetup letter;
			foreach(int letter_idx in animation.m_letters_to_animate)
			{
				letter = m_letters[letter_idx];

				if(furthestActionIndex == -1 || letter.ActionIndex > furthestActionIndex)
					furthestActionIndex = letter.ActionIndex;

				if(furthestActionIndexProgress == -1 || letter.ActionProgress > furthestActionIndexProgress)
					furthestActionIndexProgress = letter.ActionProgress;

				if(letter.ActiveLoopCycles.Count > 0)
				{
					// Record lowest active loop iteration counts for later syncing
					for(int loop_cycle_index = 0; loop_cycle_index < letter.ActiveLoopCycles.Count; loop_cycle_index++)
					{
						ActionLoopCycle loop_cycle = letter.ActiveLoopCycles[loop_cycle_index];

						if(lowestActiveLoopIterations[loop_cycle.m_active_loop_index] == 0 || loop_cycle.m_number_of_loops < lowestActiveLoopIterations[loop_cycle.m_active_loop_index])
						{
							lowestActiveLoopIterations[loop_cycle.m_active_loop_index] = loop_cycle.m_number_of_loops;
						}
					}
				}

				if(letter.ActiveLoopCycles.Count > 0 && (deepestLoopDepth == -1 || letter.ActiveLoopCycles.Count < deepestLoopDepth))
				{
					// Record this new deepest loop cycle
					deepestLoopDepth = letter.ActiveLoopCycles.Count;
					deepestLoopCycle = letter.ActiveLoopCycles[0];
					deepestLoopLetter = letter;
				}
				else if (letter.ActiveLoopCycles.Count == 0)
					allLettersHaveLoops = false;
			}

			int action_index_to_continue_to = -1;
			int action_progress = 0;

			if(deepestLoopCycle == null)
			{
				// These letters are not currently in any loops. Nothing to Continue Past
				return;
			}
			else
			{
				if(deepestLoopCycle.m_end_action_idx + 1 < animation.LetterActions.Count)
				{
					int offsetIdx = 1;
					while(animation.GetAction(deepestLoopCycle.m_end_action_idx + offsetIdx).m_action_type == ACTION_TYPE.BREAK)
					{
						if(deepestLoopCycle.m_end_action_idx + offsetIdx + 1 >= animation.LetterActions.Count)
						{
							break;
						}
						else
							offsetIdx++;

					}

					if(animation.GetAction(deepestLoopCycle.m_end_action_idx + offsetIdx).m_action_type != ACTION_TYPE.BREAK)
					{
						// There's a LetterAction defined beyond the end of this deepest loop.
						// Continue to the start of that letterAction
						action_index_to_continue_to = deepestLoopCycle.m_end_action_idx + offsetIdx;
						action_progress = 0;
					}
				}

				if(deepestLoopCycle.m_end_action_idx + 1 == animation.LetterActions.Count || action_index_to_continue_to == -1)
				{
					// This loop finishes with the last LetterAction, or it failed to find a non-break action beyond the end of the loop

					int start_action_index = (deepestLoopCycle.m_loop_type == LOOP_TYPE.LOOP || deepestLoopCycle.m_finish_at_end) ? deepestLoopCycle.m_end_action_idx : deepestLoopCycle.m_start_action_idx;
					int direction = (deepestLoopCycle.m_loop_type == LOOP_TYPE.LOOP || deepestLoopCycle.m_finish_at_end) ? -1 : 1;
					int idx = 0;

					while(start_action_index + (direction * idx) >= deepestLoopCycle.m_start_action_idx &&
					      start_action_index + (direction * idx) <= deepestLoopCycle.m_end_action_idx &&
					      animation.GetAction(start_action_index + (direction * idx)).m_action_type == ACTION_TYPE.BREAK)
					{
						idx++;
					}

					if(start_action_index + (direction * idx) >= deepestLoopCycle.m_start_action_idx &&
					   start_action_index + (direction * idx) <= deepestLoopCycle.m_end_action_idx)
					{
						action_index_to_continue_to = start_action_index + (direction * idx);
						action_progress = (deepestLoopCycle.m_loop_type == LOOP_TYPE.LOOP || deepestLoopCycle.m_finish_at_end) ? 1 : 0;
					}
				}

				if(!allLettersHaveLoops && furthestActionIndex > ((deepestLoopCycle.m_loop_type == LOOP_TYPE.LOOP || deepestLoopCycle.m_finish_at_end) ? deepestLoopCycle.m_end_action_idx : deepestLoopCycle.m_start_action_idx))
				{
					// Some letters are without loops and are beyond the letters with loops
					action_index_to_continue_to = furthestActionIndex;
					action_progress = 1;

					deepestLoopDepth = 0;
				}
				else
				{
					// Remove any loops that have been passed by stepping through to find an appropriate action to continue to
					for(int loop_idx = 1; loop_idx < deepestLoopLetter.ActiveLoopCycles.Count; loop_idx++)
					{
						if(action_index_to_continue_to > deepestLoopLetter.ActiveLoopCycles[loop_idx].m_end_action_idx)
						{
							// remove this loop cycle too
							deepestLoopDepth --;
						}
					}
				}

			
				if(passNextInfiniteLoop)// && deepestLoopCycle.m_number_of_loops > 0 && deepestLoopDepth > 1)
				{
					bool infiniteLoopAlreadyPassed = false;

					// Check if already passed an infinite loop
					for(int loopDepth = 0; loopDepth < (deepestLoopLetter.ActiveLoopCycles.Count - deepestLoopDepth) + 1; loopDepth ++)
					{
						if(deepestLoopLetter.ActiveLoopCycles[loopDepth].m_number_of_loops <= 0)
						{
							infiniteLoopAlreadyPassed = true;

							break;
						}
					}


					if(!infiniteLoopAlreadyPassed)
					{
						// Supposed to be exiting the next infinite loop, but deepest active loop found isn't infinite
						// Search for deeper infinite active loop
						for(int loopDepth = m_letters[0].ActiveLoopCycles.Count - deepestLoopDepth; loopDepth < m_letters[0].ActiveLoopCycles.Count; loopDepth++)
						{
							if(m_letters[0].ActiveLoopCycles[loopDepth].m_number_of_loops <= 0)
							{
								// Found an infinite loop deeper in the active loops list
								// Set as new deepestLoop
								deepestLoopCycle = m_letters[0].ActiveLoopCycles[loopDepth];
								deepestLoopDepth -= (loopDepth - (m_letters[0].ActiveLoopCycles.Count - deepestLoopDepth));
								deepestLoopLetter = m_letters[0];
							}
						}
					}
				}
			}


//			Debug.Log("action_index_to_continue_to : " +action_index_to_continue_to + ", action_progress : " + action_progress + ", furthestActionIndexProgress : " + furthestActionIndexProgress + ", deepestLoopDepth : "+ deepestLoopDepth);

			// Setup each letter to continue
			foreach(int letter_idx in animation.m_letters_to_animate)
			{
				letter = m_letters[letter_idx];

//				lerpSyncDuration = 0f;
				letter.ContinueFromCurrentToAction(animation,
				                                   action_index_to_continue_to,
				                                   action_progress == 0,
				                                   deepestLoopCycle == null ? action_index_to_continue_to : deepestLoopCycle.m_end_action_idx + 1,
				                                   m_animate_per,
				                                   m_animation_timer,
				                                   lerpSyncDuration,
				                                   furthestActionIndexProgress,
				                                   deepestLoopDepth,
				                                   continueType,
				                                   trimInterimLoops,
				                                   lowestActiveLoopIterations);
			}

			// Set animation speed override
			if(continueType == ContinueType.EndOfLoop)
				m_runtime_animation_speed_factor = animationSpeedOverride;
			
			if(finishedCallback != null)
				m_animation_continue_callback = finishedCallback;


			// Set state to CONTINUING
			animation.CurrentAnimationState = LETTER_ANIMATION_STATE.CONTINUING;
		}



		public bool UpdateMesh(bool use_timer, bool force_render, int action_idx, float action_progress, float delta_time = 0)
		{
			bool all_letter_anims_finished = true;

			if(m_current_mesh_verts == null || m_current_mesh_verts.Length != m_num_meshes * 4)
			{
				m_current_mesh_verts = new Vector3[m_num_meshes * 4];
				m_current_mesh_colours = new Color[m_num_meshes * 4];

				// Initialise values
				Vector3[] base_verts;
				int mesh_idx = 0;
				for (int letter_idx = 0; letter_idx < m_letters.Length; letter_idx++)
				{
					if(!m_letters[letter_idx].StubInstance)
					{
						base_verts = m_letters[letter_idx].BaseVertices;
						
						m_current_mesh_verts[mesh_idx * 4] = base_verts[0];
						m_current_mesh_verts[mesh_idx * 4 + 1] = base_verts[1];
						m_current_mesh_verts[mesh_idx * 4 + 2] = base_verts[2];
						m_current_mesh_verts[mesh_idx * 4 + 3] = base_verts[3];
						
						m_current_mesh_colours[mesh_idx * 4] = Color.white;
						m_current_mesh_colours[mesh_idx * 4 + 1] = Color.white;
						m_current_mesh_colours[mesh_idx * 4 + 2] = Color.white;
						m_current_mesh_colours[mesh_idx * 4 + 3] = Color.white;
						
						mesh_idx++;
					}
				}
			}

			LetterSetup letter_setup;

			if(m_master_animations != null)
			{
				bool all_letter_anims_waiting;
				bool all_letter_anims_waiting_infinitely;
				bool all_letter_anims_continuing_finished;
				int lowest_action_progress = -1;
				int last_letter_idx;

				foreach(LetterAnimation animation in m_master_animations)
				{
					last_letter_idx = -1;

					all_letter_anims_waiting = true;
					all_letter_anims_waiting_infinitely = true;
					all_letter_anims_continuing_finished = true;

					foreach(int letter_idx in animation.m_letters_to_animate)
					{
						// two of the same letter index next to each other. Or idx out of bounds.
						if(letter_idx == last_letter_idx || letter_idx >= m_letters.Length)
						{
							continue;
						}

						letter_setup = m_letters[letter_idx];
						
						if(lowest_action_progress == -1 || letter_setup.ActionProgress < lowest_action_progress)
						{
							lowest_action_progress = letter_setup.ActionProgress;
						}

						// Initialise values with exisiting mesh data
						Vector3[] letter_verts = new Vector3[]{	m_current_mesh_verts[letter_setup.MeshIndex*4],
																m_current_mesh_verts[letter_setup.MeshIndex*4 + 1],
																m_current_mesh_verts[letter_setup.MeshIndex*4 + 2],
																m_current_mesh_verts[letter_setup.MeshIndex*4 + 3]};
						Color[] letter_colours = new Color[]{	m_current_mesh_colours[letter_setup.MeshIndex*4],
																m_current_mesh_colours[letter_setup.MeshIndex*4 + 1],
																m_current_mesh_colours[letter_setup.MeshIndex*4 + 2],
																m_current_mesh_colours[letter_setup.MeshIndex*4 + 3]};

						if(use_timer)
						{
							letter_setup.AnimateMesh(this,
							                         force_render,
							                         m_animation_timer,
							                         m_lowest_action_progress,
							                         animation,
							                         m_animate_per,
							                         delta_time,
							                         ref letter_verts,
							                         ref letter_colours);

							LETTER_ANIMATION_STATE anim_state = letter_setup.CurrentAnimationState;

							if(anim_state != LETTER_ANIMATION_STATE.CONTINUING_FINISHED)
								all_letter_anims_continuing_finished = false;

							if(anim_state == LETTER_ANIMATION_STATE.STOPPED)
							{
								lowest_action_progress = letter_setup.ActionProgress;
							}
							else
							{
								all_letter_anims_finished = false;
							}

							if(anim_state != LETTER_ANIMATION_STATE.WAITING_INFINITE)
							{
								all_letter_anims_waiting_infinitely = false;
							}

							if(anim_state != LETTER_ANIMATION_STATE.WAITING && anim_state != LETTER_ANIMATION_STATE.WAITING_INFINITE)
							{
								all_letter_anims_waiting = false;
							}
						}
						else
						{
							letter_setup.SetMeshState(this, Mathf.Clamp(action_idx, 0, animation.NumActions-1), action_progress, animation, m_animate_per, ref letter_verts, ref letter_colours);
						}

						// Update the verts for this letter
						for(int idx=0; idx < 4; idx++)
							m_current_mesh_verts[letter_setup.MeshIndex*4 + idx] = letter_verts[idx];

	//					m_colourVertMapping = new int[]{1,0,3,2};

						// Set Colours
						for(int idx=0; idx < 4; idx++)
							m_current_mesh_colours[letter_setup.MeshIndex*4 + idx] = letter_colours[m_colourVertMapping[idx]];

						last_letter_idx = letter_idx;
					}
					
					// Set animation state
					if(animation.m_letters_to_animate.Count > 0)
					{
						if(use_timer)
						{
							if(animation.CurrentAnimationState == LETTER_ANIMATION_STATE.CONTINUING)
							{
								if(all_letter_anims_continuing_finished)
								{
									animation.CurrentAnimationState = LETTER_ANIMATION_STATE.PLAYING;

									m_runtime_animation_speed_factor = 1;

									// Set all letters to PLAYING state
									foreach(int letter_idx in animation.m_letters_to_animate)
									{
										m_letters[letter_idx].SetPlayingState();
									}

									// Important to update the lowest action progress to the same consistent value, so that subsequent ForceSameStart actions will work properly
									m_lowest_action_progress = m_letters[0].ActionProgress;

									// Fire off callback
									if(m_animation_continue_callback != null)
										m_animation_continue_callback(m_letters[0].ActionIndex);

									m_animation_continue_callback = null;
								}
							}
							else if(all_letter_anims_waiting_infinitely)
								animation.CurrentAnimationState = LETTER_ANIMATION_STATE.WAITING_INFINITE;
							else if(all_letter_anims_waiting)
								animation.CurrentAnimationState = LETTER_ANIMATION_STATE.WAITING;
							else if(!all_letter_anims_finished)
								animation.CurrentAnimationState = LETTER_ANIMATION_STATE.PLAYING;
						}
					}
					else
					{
						// No letters in this animation, so mark as STOPPED
						animation.CurrentAnimationState = LETTER_ANIMATION_STATE.STOPPED;
					}
					
					if(lowest_action_progress > m_lowest_action_progress)
					{
						m_lowest_action_progress = lowest_action_progress;
					}
				}
			}

			m_what_just_changed = ANIMATION_DATA_TYPE.NONE;

			return all_letter_anims_finished;
		}




		void PauseAllParticleEffects(bool paused)
		{
			if(m_particle_effect_managers != null)
			{
				foreach(ParticleEffectInstanceManager particle_effect in m_particle_effect_managers)
				{
					particle_effect.Pause(paused);
				}
			}
		}
		
		void StopAllParticleEffects(bool force_stop = false)
		{
			if(m_particle_effect_managers != null)
			{
				foreach(ParticleEffectInstanceManager particle_effect in m_particle_effect_managers)
				{
					particle_effect.Stop(force_stop);
				}
				
				m_particle_effect_managers = new List<ParticleEffectInstanceManager>();
			}
			
			if(m_particle_systems != null)
			{
				foreach(ParticleSystem p_system in m_particle_systems)
				{
					if(p_system == null)
						continue;
					
					p_system.Stop();
					p_system.Clear();
				}
			}
			if(m_particle_emitters != null)
			{
				foreach(ParticleEmitter p_emit in m_particle_emitters)
				{
					if(p_emit == null)
						continue;
					
					p_emit.emit = false;
					p_emit.ClearParticles();
				}
			}
		}
		
		public void ClearCachedAudioParticleInstances(bool refresh_latest = false)
		{
			if(refresh_latest)
			{
				m_audio_sources = new List<AudioSource>(m_gameObect.GetComponentsInChildren<AudioSource>());
				m_particle_emitters = new List<ParticleEmitter>(m_gameObect.GetComponentsInChildren<ParticleEmitter>());
				m_particle_systems = new List<ParticleSystem>(m_gameObect.GetComponentsInChildren<ParticleSystem>());
			}
			
			foreach(AudioSource a_source in m_audio_sources)
			{
				if(a_source != null && a_source.gameObject != null)
					GameObject.DestroyImmediate(a_source.gameObject);
			}
			m_audio_sources = new List<AudioSource>();
			
			foreach(ParticleEmitter p_emitter in m_particle_emitters)
			{
				if(p_emitter != null && p_emitter.gameObject != null)
					GameObject.DestroyImmediate(p_emitter.gameObject);
			}
			m_particle_emitters = new List<ParticleEmitter>();
			
			foreach(ParticleSystem p_system in m_particle_systems)
			{
				if(p_system != null && p_system.gameObject != null)
					GameObject.DestroyImmediate(p_system.gameObject);
			}
			m_particle_systems = new List<ParticleSystem>();
			
			m_particle_effect_managers = new List<ParticleEffectInstanceManager>();
		}


		AudioSource AddNewAudioChild()
		{
			GameObject new_audio_source = new GameObject("TextFx_AudioSource");
			new_audio_source.transform.parent = m_transform;
			
			AudioSource a_source = new_audio_source.AddComponent<AudioSource>();
			
			a_source.playOnAwake = false;
			
			if(m_audio_sources == null)
			{
				m_audio_sources = new List<AudioSource>();
			}
			
			m_audio_sources.Add(a_source);
			
			return a_source;
		}
		
		void PlayClip(AudioSource a_source, AudioClip clip, float delay, float start_time, float volume, float pitch)
		{
			a_source.clip = clip;
			a_source.time = start_time;
			a_source.volume = volume;
			a_source.pitch = pitch;
			
	#if !UNITY_3_5 && !UNITY_4_0
			a_source.PlayDelayed(delay);
	#else
			a_source.Play((ulong)( delay * 44100));
	#endif
		}
		
		public void PlayAudioClip(AudioEffectSetup effect_setup, AnimationProgressionVariables progression_vars, AnimatePerOptions animate_per)
		{
			bool sound_played = false;
			AudioSource source = null;
			
			if(m_audio_sources != null)
			{
				foreach(AudioSource a_source in m_audio_sources)
				{
					if(!a_source.isPlaying)
					{
						// audio source free to play a sound
						source= a_source;
						
						sound_played = true;
						break;
					}
				}
				
				if(!sound_played)
				{
					source = AddNewAudioChild();
				}
			}
			else
			{
				source = AddNewAudioChild();
			}
			
			PlayClip(
				source,
				effect_setup.m_audio_clip,
				effect_setup.m_delay.GetValue(progression_vars, animate_per),
				effect_setup.m_offset_time.GetValue(progression_vars, animate_per),
				effect_setup.m_volume.GetValue(progression_vars, animate_per),
				effect_setup.m_pitch.GetValue(progression_vars, animate_per));
		}
		
		public void PlayParticleEffect(LetterSetup letter_setup, ParticleEffectSetup effect_setup, AnimationProgressionVariables progression_vars, AnimatePerOptions animate_per)
		{
			bool effect_played = false;
			
			if(effect_setup.m_legacy_particle_effect != null)
			{
				if(m_particle_emitters == null)
				{
					m_particle_emitters = new List<ParticleEmitter>();
				}
				
				foreach(ParticleEmitter p_emitter in m_particle_emitters)
				{
					if(!p_emitter.emit && p_emitter.particleCount == 0 && p_emitter.name.Equals(effect_setup.m_legacy_particle_effect.name + "(Clone)"))
					{
						m_particle_effect_managers.Add(new ParticleEffectInstanceManager(this, letter_setup, effect_setup, progression_vars, animate_per, particle_emitter : p_emitter));
						
						effect_played = true;
						break;
					}
				}
				
				if(!effect_played)
				{
					ParticleEmitter p_emitter = GameObject.Instantiate(effect_setup.m_legacy_particle_effect) as ParticleEmitter;
					m_particle_emitters.Add(p_emitter);
	#if !UNITY_3_5
					p_emitter.gameObject.SetActive(true);
	#else
					p_emitter.gameObject.SetActiveRecursively(true);
	#endif
					p_emitter.emit = false;
					p_emitter.transform.parent = m_transform;
					
					m_particle_effect_managers.Add(new ParticleEffectInstanceManager(this, letter_setup, effect_setup, progression_vars, animate_per, particle_emitter : p_emitter));
				}
			}
			else if(effect_setup.m_shuriken_particle_effect != null)
			{
				if(m_particle_systems == null)
					m_particle_systems = new List<ParticleSystem>();
				
				foreach(ParticleSystem p_system in m_particle_systems)
				{
					// check if particle system instance is currently not being used, and if it's the same type of effect that we're looking for.
					if(!p_system.isPlaying && p_system.particleCount == 0 && p_system.name.Equals(effect_setup.m_shuriken_particle_effect.name + "(Clone)"))
					{
						m_particle_effect_managers.Add(new ParticleEffectInstanceManager(this, letter_setup, effect_setup, progression_vars, animate_per, particle_system : p_system));
						
						effect_played = true;
						break;
					}
				}
				
				if(!effect_played)
				{
					// Make a new instance of the particleSystem effect and add to pool
					ParticleSystem p_system = GameObject.Instantiate(effect_setup.m_shuriken_particle_effect) as ParticleSystem;
					m_particle_systems.Add(p_system);
	#if !UNITY_3_5
					p_system.gameObject.SetActive(true);
	#else
					p_system.gameObject.SetActiveRecursively(true);
	#endif
					p_system.playOnAwake = false;
					p_system.Stop();
					p_system.transform.parent = m_transform;
					
					m_particle_effect_managers.Add(new ParticleEffectInstanceManager(this, letter_setup, effect_setup, progression_vars, animate_per, particle_system : p_system));
				}
			}
		}





		public void AddAnimation()
		{
			if(m_master_animations == null)
				m_master_animations = new List<LetterAnimation>();
			
			m_master_animations.Add(new LetterAnimation());
		}
		
		public void RemoveAnimation(int index)
		{
			if(m_master_animations != null && index >= 0 && index < NumAnimations)
				m_master_animations.RemoveAt(index);
		}
		
		public LetterAnimation GetAnimation(int index)
		{
			if(m_master_animations != null && m_master_animations.Count > index && index >= 0)
				return m_master_animations[index];
			else
				return null;
		}

		public LetterSetup GetLetter(int letterIdx)
		{
			if(m_letters != null && letterIdx < m_letters.Length)
				return m_letters[letterIdx];
			return null;
		}

		IEnumerator TimeDelay(float delay, AnimationTime time_type)
		{
			if(time_type == AnimationTime.GAME_TIME)
			{
				yield return new WaitForSeconds(delay);
			}
			else
			{
				float timer = 0;
				float last_time = Time.realtimeSinceStartup;
				float delta_time;
				while(timer < delay)
				{
					delta_time = Time.realtimeSinceStartup - last_time;
					if(delta_time > 0.1f)
					{
						delta_time = 0.1f;
					}
					timer += delta_time;
					last_time = Time.realtimeSinceStartup;
					yield return false;
				}
			}
		}


	//	public void ExportData(Boomlagoon.JSON.JSONObject json_data, bool hard_copy = false)
	//	{
	//		json_data["TEXTFX_EXPORTER_VERSION"] = JSON_EXPORTER_VERSION;
	//		json_data["m_animate_per"] = (int) m_animate_per;
	//
	//		if (hard_copy)
	//		{
	//			json_data["m_begin_delay"] = m_begin_delay;
	//			json_data["m_begin_on_start"] = m_begin_on_start;
	//			json_data["m_on_finish_action"] = (int) m_on_finish_action;
	//			json_data["m_time_type"] = (int) m_time_type;
	//		}
	//
	//		Boomlagoon.JSON.JSONArray letter_animations_data = new Boomlagoon.JSON.JSONArray();
	//		if(m_master_animations != null)
	//			foreach(LetterAnimation anim in m_master_animations)
	//		{
	//			letter_animations_data.Add(anim.ExportData());
	//		}
	//		json_data["LETTER_ANIMATIONS_DATA"] = letter_animations_data;
	//	}



#if UNITY_EDITOR
		public string ExportDataAsPresetSection(bool saveSampleTextInfo = true)
		{
			if(m_master_animations == null || m_master_animations.Count == 0)
			{
				Debug.LogError("There's no animation to export");
				return "";
			}

			Boomlagoon.JSON.JSONObject json_data = new Boomlagoon.JSON.JSONObject();
			
			json_data["TEXTFX_EXPORTER_VERSION"] = JSON_EXPORTER_VERSION;
			json_data["LETTER_ANIMATIONS_DATA"] = m_master_animations[0].ExportDataAsPresetSection(saveSampleTextInfo);
			
			
			// Handle exporting quick setup configuration fields
			Boomlagoon.JSON.JSONArray effect_settings_options = new Boomlagoon.JSON.JSONArray();
			if(m_preset_effect_settings != null)
			{
				foreach(PresetEffectSetting effect_setting in m_preset_effect_settings)
				{
					effect_settings_options.Add(effect_setting.ExportData());
				}
			}
			json_data["PRESET_EFFECT_SETTINGS"] = effect_settings_options;
			
			return json_data.ToString();
		}

		public string ExportData(bool hard_copy = false)
		{
			Boomlagoon.JSON.JSONObject json_data = new Boomlagoon.JSON.JSONObject();

			json_data["TEXTFX_EXPORTER_VERSION"] = JSON_EXPORTER_VERSION;
			json_data["m_animate_per"] = (int) m_animate_per;
			
			if (hard_copy)
			{
				json_data["m_begin_delay"] = m_begin_delay;
				json_data["m_begin_on_start"] = m_begin_on_start;
				json_data["m_on_finish_action"] = (int) m_on_finish_action;
				json_data["m_time_type"] = (int) m_time_type;
			}
			
			Boomlagoon.JSON.JSONArray letter_animations_data = new Boomlagoon.JSON.JSONArray();
			if(m_master_animations != null)
			{
				foreach(LetterAnimation anim in m_master_animations)
				{
					letter_animations_data.Add(anim.ExportData());
				}
			}
			json_data["LETTER_ANIMATIONS_DATA"] = letter_animations_data;


			// Handle exporting quick setup configuration fields
			Boomlagoon.JSON.JSONArray effect_settings_options = new Boomlagoon.JSON.JSONArray();
			if(m_preset_effect_settings != null)
			{
				foreach(PresetEffectSetting effect_setting in m_preset_effect_settings)
				{
					effect_settings_options.Add(effect_setting.ExportData());
				}
			}
			json_data["PRESET_EFFECT_SETTINGS"] = effect_settings_options;

			
			return json_data.ToString();
		}


		// Used for importing a preset animation section's data, to have its animation edited in the editor, or have its quick-edit settings edited
		public void ImportPresetAnimationSectionData(string data, bool force_clear_old_audio_particles = false)
		{
			if(force_clear_old_audio_particles)
				ClearCachedAudioParticleInstances(true);
			
			Boomlagoon.JSON.JSONObject json_data = Boomlagoon.JSON.JSONObject.Parse(data, true);
			
			if(json_data != null)
			{
				m_master_animations = new List<LetterAnimation>();
				LetterAnimation letter_anim = new LetterAnimation();
				letter_anim.ImportPresetSectionData(json_data["LETTER_ANIMATIONS_DATA"].Obj, m_letters, m_animation_interface_reference.AssetNameSuffix);

				m_master_animations.Add(letter_anim);
				
				m_preset_effect_settings = new List<PresetEffectSetting>();
				
				// Import any Quick setup settings info
				if(json_data.ContainsKey("PRESET_EFFECT_SETTINGS"))
				{
					PresetEffectSetting effectSetting;
					foreach(Boomlagoon.JSON.JSONValue effectSettingData in json_data["PRESET_EFFECT_SETTINGS"].Array)
					{
						effectSetting = new PresetEffectSetting();
						effectSetting.ImportData(effectSettingData.Obj);
						m_preset_effect_settings.Add(effectSetting);
					}
				}
			}
			else
			{
				// Import string is not valid JSON, therefore assuming it is in the legacy data import format.
				Debug.LogError("TextFx animation import failed. Non-valid JSON data provided");
			}
			
			if(!Application.isPlaying && m_animation_interface_reference.Text.Equals(""))
				m_animation_interface_reference.SetText("TextFx");
			
			PrepareAnimationData ();
			
			ResetAnimation();
			
			// Update mesh
			m_animation_interface_reference.UpdateTextFxMesh(MeshVerts, MeshColours);
			
			SceneView.RepaintAll();
		}
		
		
		public void ImportData(string data, TextFxAnimationManager.PresetAnimationSection animationSection, PRESET_ANIMATION_SECTION section, bool force_clear_old_audio_particles = false)
		{
			if(force_clear_old_audio_particles)
				ClearCachedAudioParticleInstances(true);
			
			int num_actions_added = 0;
			int num_loops_added = 0;
			int insert_action_index = 0;
			int insert_loop_index = 0;
			
			if(section == PRESET_ANIMATION_SECTION.MAIN)
			{
				insert_action_index = m_preset_intro.m_start_action + m_preset_intro.m_num_actions + (m_preset_intro.m_active ? 1 : 0);
				insert_loop_index = m_preset_intro.m_start_loop + m_preset_intro.m_num_loops;
			}
			else if(section == PRESET_ANIMATION_SECTION.OUTRO)
			{
				insert_action_index = m_preset_main.m_start_action + m_preset_main.m_num_actions + (m_preset_main.m_active ? 1 : 0);
				insert_loop_index = m_preset_main.m_start_loop + m_preset_main.m_num_loops;
			}
			
			
			Boomlagoon.JSON.JSONObject json_data = Boomlagoon.JSON.JSONObject.Parse(data, true);
			
			if(json_data != null)
			{
				if(m_master_animations == null || m_master_animations.Count == 0)
				m_master_animations = new List<LetterAnimation>(){ new LetterAnimation() };
				
				
				m_master_animations[0].ImportPresetSectionData(json_data["LETTER_ANIMATIONS_DATA"].Obj, m_letters, insert_action_index, insert_loop_index, ref num_actions_added, ref num_loops_added, m_animation_interface_reference.AssetNameSuffix);
				
				animationSection.m_preset_effect_settings = new List<PresetEffectSetting>();
				
				// Import any Quick setup settings info
				if(json_data.ContainsKey("PRESET_EFFECT_SETTINGS"))
				{
					PresetEffectSetting effectSetting;
					foreach(Boomlagoon.JSON.JSONValue effectSettingData in json_data["PRESET_EFFECT_SETTINGS"].Array)
					{
						effectSetting = new PresetEffectSetting();
						effectSetting.ImportData(effectSettingData.Obj);
						animationSection.m_preset_effect_settings.Add(effectSetting);
					}
				}
			}
			else
			{
				// Import string is not valid JSON, therefore assuming it is in the legacy data import format.
				Debug.LogError("TextFx animation import failed. Non-valid JSON data provided");
				
//				this.ImportLegacyData(data);
//				
//				m_preset_effect_settings = new List<PresetEffectSetting>();
			}
			
			
			if(!Application.isPlaying && m_animation_interface_reference.Text.Equals(""))
				m_animation_interface_reference.SetText("TextFx");
			
			PrepareAnimationData ();
			
			ResetAnimation();
			
			// Update mesh
			m_animation_interface_reference.UpdateTextFxMesh(MeshVerts, MeshColours);
			
			SceneView.RepaintAll();
			
			animationSection.m_num_actions = num_actions_added;
			animationSection.m_num_loops = num_loops_added;
		}
#endif


		public void ImportData(string data, bool force_clear_old_audio_particles = false)
		{
			if(force_clear_old_audio_particles)
				ClearCachedAudioParticleInstances(true);
			
			Boomlagoon.JSON.JSONObject json_data = Boomlagoon.JSON.JSONObject.Parse(data, true);
			
			if(json_data != null)
			{
				m_animate_per = (AnimatePerOptions) (int) json_data["m_animate_per"].Number;
				
				if(json_data.ContainsKey("m_begin_delay")) m_begin_delay = (float) json_data["m_begin_delay"].Number;
				if(json_data.ContainsKey("m_begin_on_start")) m_begin_on_start = json_data["m_begin_on_start"].Boolean;
				if(json_data.ContainsKey("m_on_finish_action")) m_on_finish_action = (ON_FINISH_ACTION) (int) json_data["m_on_finish_action"].Number;
				if(json_data.ContainsKey("m_time_type")) m_time_type = (AnimationTime) (int) json_data["m_time_type"].Number;
				
				m_master_animations = new List<LetterAnimation>();
				LetterAnimation letter_anim;
				foreach(Boomlagoon.JSON.JSONValue animation_data in json_data["LETTER_ANIMATIONS_DATA"].Array)
				{
					letter_anim = new LetterAnimation();
					letter_anim.ImportData(animation_data.Obj, m_animation_interface_reference.AssetNameSuffix);
					m_master_animations.Add(letter_anim);
				}

#if UNITY_EDITOR
				m_preset_effect_settings = new List<PresetEffectSetting>();

				// Import any Quick setup settings info
				if(json_data.ContainsKey("PRESET_EFFECT_SETTINGS"))
				{
					PresetEffectSetting effectSetting;
					foreach(Boomlagoon.JSON.JSONValue effectSettingData in json_data["PRESET_EFFECT_SETTINGS"].Array)
					{
						effectSetting = new PresetEffectSetting();
						effectSetting.ImportData(effectSettingData.Obj);
						m_preset_effect_settings.Add(effectSetting);
					}
				}
#endif
			}
			else
			{
				// Import string is not valid JSON, therefore assuming it is in the legacy data import format.
				Debug.LogError("TextFx animation import failed. Non-valid JSON data provided");

				this.ImportLegacyData(data);

#if UNITY_EDITOR
				m_preset_effect_settings = new List<PresetEffectSetting>();
#endif
			}


			if(!Application.isPlaying && m_animation_interface_reference.Text.Equals(""))
				m_animation_interface_reference.SetText("TextFx");

			PrepareAnimationData ();

			ResetAnimation();

			// Update mesh
			m_animation_interface_reference.UpdateTextFxMesh(MeshVerts, MeshColours);

#if UNITY_EDITOR
			SceneView.RepaintAll();
#endif
		}
	}
}