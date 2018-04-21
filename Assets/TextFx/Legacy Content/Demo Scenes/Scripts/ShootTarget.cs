using UnityEngine;
using System.Collections;

namespace TextFx.LegacyContent.Demo
{

	public enum TARGET_COLOUR
	{
		BLUE,
		GREEN,
		RED
	}

	public class ShootTarget : MonoBehaviour {
		
		Color m_red_colour = new Color(1,0,0,1);
		Color m_green_colour = new Color(0,1,0,1);
		Color m_blue_colour = new Color(0,0,1,1);
		
		Mesh m_mesh;
		Color m_tint_colour = new Color(1,1,1,0.2f);
		Color m_active_colour;
		bool m_activated = false;
		Transform m_transform;
		TARGET_COLOUR m_colour;
		
		void Start ()
		{
			m_mesh = GetComponent<MeshFilter>().mesh;
			
			// Initialise mesh vertex colours to clear, to hide the mesh texture on screen
			m_mesh.colors = new Color[]{ Color.clear, Color.clear, Color.clear, Color.clear };
			
			// register reset event callback
			RuntimeDynamicSceneManager.m_reset_event += Reset;
			
			// Cache transform component reference
			m_transform = transform;
			
			SetupRandomColour();
		}
		
		void SetupRandomColour()
		{
			m_colour = (TARGET_COLOUR) Random.Range(0,3);
			
			switch(m_colour)
			{
				case TARGET_COLOUR.BLUE:
					m_active_colour = m_blue_colour; break;
				case TARGET_COLOUR.GREEN:
					m_active_colour = m_green_colour; break;
				case TARGET_COLOUR.RED:
					m_active_colour = m_red_colour; break;
			}
		}
		
		void OnMouseOver()
		{
			if(!m_activated)
				m_mesh.colors = new Color[]{ m_tint_colour, m_tint_colour, m_tint_colour, m_tint_colour };
		}
		
		void OnMouseExit()
		{
			if(!m_activated)
				m_mesh.colors = new Color[]{ Color.clear, Color.clear, Color.clear, Color.clear };
		}
		
		void Reset()
		{
			m_activated = false;
			m_mesh.colors = new Color[]{ Color.clear, Color.clear, Color.clear, Color.clear };
			
			SetupRandomColour();
		}
		
		void OnMouseDown()
		{
			if(!m_activated)
			{
				m_mesh.colors = new Color[]{ m_active_colour, m_active_colour, m_active_colour, m_active_colour };
				
				RuntimeDynamicSceneManager.TargetHit(m_transform.position, m_colour);
				
				m_activated = true;
			}
		}
	}
}