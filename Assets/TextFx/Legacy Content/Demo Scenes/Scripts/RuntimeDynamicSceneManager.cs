//#define USE_EDITOR_GUI_NAVIGATION
using UnityEngine;
using System.Collections;

namespace TextFx.LegacyContent.Demo
{
	public class RuntimeDynamicSceneManager : MonoBehaviour {

		const int EFFECT_POOL_SIZE = 10;
		
		public static System.Action m_reset_event;		// Action event called to trigger all listening ShootTarget's to reset.
		static RuntimeDynamicSceneManager m_instance;
		
		public EffectManager m_hit_effect_prefab;		// Reference to text animation prefab being used on target hit.
		public AudioClip m_boom_clip;					
		public AudioClip m_ding_clip;
		public Texture2D m_cursor_texture;				// Texture used for cursor crosshair
		public PointsTextManager m_points_manager;
		
		EffectManager[] m_hit_effects;					// Cached pool of text animation instances.
		int m_hit_effect_index = 0;
		Vector2 m_cursor_hot_spot = new Vector2(16,16);
		
		void Start ()
		{
			m_instance = this;
			
			// Set cursor texture
			StartCoroutine(SetCustomCursor());
			
			// Populate array pool of text animations
			m_hit_effects = new EffectManager[EFFECT_POOL_SIZE];
			for(int idx=0; idx < EFFECT_POOL_SIZE; idx++)
			{
				m_hit_effects[idx] = Instantiate(m_hit_effect_prefab) as EffectManager;
			}
		}
		
		IEnumerator SetCustomCursor()
		{
			yield return new WaitForSeconds(0.1f);
			Cursor.SetCursor(m_cursor_texture, m_cursor_hot_spot, CursorMode.Auto);
		}

		// Called by ShootTarget instances when a valid mouseDown is detected
		public static void TargetHit(Vector3 position, TARGET_COLOUR colour)
		{
			// Grab next available text animation instance from object pool
			EffectManager hit_effect = m_instance.m_hit_effects[m_instance.m_hit_effect_index];
			
			// Position effect object
			hit_effect.transform.position = position;
			
			// Grab reference to the audio effect setup on our text animation
			AudioEffectSetup audio_setup = hit_effect.GetAnimation(0).GetAction(0).GetAudioEffectSetup(0);
			
			// Depending on its colour value, dynamically alter the audio effect setup data.
			// Then set the text to the associated word, which automatically performs the necessary PrepareAnimationData() step.
			// Note: If text wasn't changing, you'd need to manually call hit_effect.PrepareAnimationData() before playing the animation, in order to see the effects of your changes
			switch(colour)
			{
				case TARGET_COLOUR.RED:
					audio_setup.m_audio_clip = m_instance.m_boom_clip;
					audio_setup.m_offset_time.SetConstant(0.2f);
					audio_setup.m_volume.SetConstant(0.5f);
					audio_setup.m_pitch.SetRandom(1.5f, 2.2f, true);
					hit_effect.SetText("BOOM!");
				
					m_instance.m_points_manager.AddPoints(1);
					break;
				case TARGET_COLOUR.GREEN:
					audio_setup.m_audio_clip = m_instance.m_ding_clip;
					audio_setup.m_offset_time.SetConstant(0);
					audio_setup.m_volume.SetConstant(1);
					audio_setup.m_pitch.SetRandom(1.8f, 2.2f, true);
					hit_effect.SetText("DING!");
				
					m_instance.m_points_manager.AddPoints(2);
					break;
				case TARGET_COLOUR.BLUE:
					audio_setup.m_audio_clip = m_instance.m_ding_clip;
					audio_setup.m_offset_time.SetConstant(0);
					audio_setup.m_volume.SetConstant(1);
					audio_setup.m_pitch.SetRandom(0.8f, 0.9f, true);
					hit_effect.SetText("DONG!");
				
					m_instance.m_points_manager.AddPoints(3);
					break;
			}
			
	//			hit_effect.PrepareAnimationData();		// Not required, since we've already set the text above, which will automatically prepare the animation data
			hit_effect.PlayAnimation();
			
			// Progress the object pool index
			m_instance.m_hit_effect_index = (m_instance.m_hit_effect_index+1) % EFFECT_POOL_SIZE;
		}
		
		void OnDestroy()
		{
			// Revert the cursor texture to the default
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		
		void OnGUI()
		{
			if(GUI.Button(new Rect(Screen.width - (Screen.width/28f) - (Screen.width/7f), 10.5f * (Screen.height/12f), Screen.width/7f, (Screen.height/13f)), "Reset"))
			{
				// Call reset event, which is listened to by the ShootTarget instances
				m_reset_event();
				
				m_instance.m_points_manager.SetPoints(0);
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