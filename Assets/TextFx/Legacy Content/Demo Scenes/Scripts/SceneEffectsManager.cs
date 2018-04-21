//#define USE_EDITOR_GUI_NAVIGATION

using UnityEngine;
using System.Collections;

namespace TextFx.LegacyContent.Demo
{

	[System.Serializable]
	public class SceneEffectData
	{
		public EffectManager m_effect_sync;
		public string m_name;
		public Vector3 m_position_offset;
	}


	public class SceneEffectsManager : MonoBehaviour {
		
		public bool m_force_effects_to_origin = true;
		public SceneEffectData[] m_effects;
		
		string[] m_effect_names;
		int m_effect_index = 0;
		EffectManager m_current_active_effect;
		
		
		void Start ()
		{
			m_effect_names = new string[m_effects.Length];
			
			int idx = 0;
			foreach(SceneEffectData effect_data in m_effects)
			{
				m_effect_names[idx] = effect_data.m_name;
					
				idx ++;
			}
			
			PlayEffect(0,0.5f);
		}
		
		void PlayEffect(int effect_idx, float delay = 0)
		{
			if(m_current_active_effect != null)
			{
	#if !UNITY_3_5
				m_current_active_effect.gameObject.SetActive(false);
	#else			
				m_current_active_effect.gameObject.SetActiveRecursively(false);
	#endif
			}
				
			m_current_active_effect = m_effects[effect_idx].m_effect_sync;
			
	#if !UNITY_3_5
				m_current_active_effect.gameObject.SetActive(true);
	#else		
			m_current_active_effect.gameObject.SetActiveRecursively(true);
	#endif
			
			if(m_force_effects_to_origin)
			{
				m_current_active_effect.transform.localPosition = m_effects[effect_idx].m_position_offset;
			}
			
			m_current_active_effect.PlayAnimation(delay);
		}
		
		void OnGUI()
		{
			m_effect_index = GUI.SelectionGrid(new Rect((Screen.width/2f) - (Screen.width/4f), 5f * (Screen.height/6f), Screen.width/2f, 1.5f * (Screen.height/13f)), m_effect_index, m_effect_names, 3);
			
			if(GUI.changed)
			{
				// Effect change requested
				PlayEffect(m_effect_index);
			}
			
	#if !UNITY_EDITOR || USE_EDITOR_GUI_NAVIGATION
			if(GUI.Button(new Rect((Screen.width/28f), 10.5f * (Screen.height/12f), Screen.width/7f, (Screen.height/13f)), "Back"))
			{
				PlayerPrefs.SetInt("TextFx_Skip_Intro_Anim", 1);
				Application.LoadLevel("TitleScene");
			}
	#endif
		}
	}
}