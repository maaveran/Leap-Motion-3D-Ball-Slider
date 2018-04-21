//#define USE_EDITOR_GUI_NAVIGATION

using UnityEngine;
using System.Collections;

namespace TextFx.LegacyContent.Demo
{
	public class TitleSceneManager : MonoBehaviour {
		
		public EffectManager m_title_effect;
		
		
	#if !UNITY_EDITOR || USE_EDITOR_GUI_NAVIGATION
		string[] m_demo_scenes = new string[] {"Basics", "Large Texts", "Audio & Particles", "Curves", "Runtime Dynamic Setup", "Other Bits"};
		int m_scene_select_idx = -1;
	#endif	
		Rect m_version_display_rect = new Rect(Screen.width - 30, Screen.height - 20, 35,20);
		bool m_display_buttons = false;
		
		// Use this for initialization
		void Start ()
		{
			if(PlayerPrefs.HasKey("TextFx_Skip_Intro_Anim"))
			{
				// Skip intro anim. Came from another scene and don't need to see it again!
				PlayerPrefs.DeleteKey("TextFx_Skip_Intro_Anim");
				
				m_title_effect.SetEndState();
				m_display_buttons = true;
			}
			else 
			{
				m_title_effect.PlayAnimation(0.5f, TitleEffectFinished);
			}
		}
				
		void TitleEffectFinished()
		{
			m_display_buttons = true;
		}
		
		void Update()
		{
			if(m_display_buttons && Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();
				if(GetComponent<Collider>().Raycast(ray, out hit, 10000))
				{
					// Replay Animation
					m_title_effect.ResetAnimation();
					m_title_effect.PlayAnimation(0.5f);
				}
			}
		}
		
		void OnGUI()
		{
	#if !UNITY_EDITOR || USE_EDITOR_GUI_NAVIGATION
			if(m_display_buttons)
			{
				m_scene_select_idx = GUI.SelectionGrid(new Rect((Screen.width/2f) - (Screen.width/4f), 5f * (Screen.height/8f), Screen.width/2, Screen.height/6), m_scene_select_idx, m_demo_scenes, 2);
							
				if(GUI.changed)
				{
					// Load scene
					Application.LoadLevel(m_demo_scenes[m_scene_select_idx]);
				}
	#if UNITY_ANDROID			
				if(GUI.Button(new Rect((Screen.width/2f) - (Screen.width/8f), 77f * (Screen.height/96f), Screen.width/4, Screen.height/12), "Exit"))
				{
					// Exit app
					Application.Quit();
				}
	#endif			
			}
	#endif
			
			GUI.Label(m_version_display_rect, EffectManager.m_version);
		}
	}
}