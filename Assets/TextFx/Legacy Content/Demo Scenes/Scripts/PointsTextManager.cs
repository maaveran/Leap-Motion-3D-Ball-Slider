using UnityEngine;
using System.Collections;

namespace TextFx.LegacyContent.Demo
{
	public class PointsTextManager : MonoBehaviour {
		
		public int Points { get; set; }
		
		public EffectManager m_points_textfx;
		public float m_text_change_delay = 0.55f;
		
		
		void Start ()
		{
			SetPoints(0);
		}
		
		public void AddPoints(int points)
		{
			StartCoroutine( SetPointsAnimated(Points + points) );
		}
		
		public void SetPoints(int points)
		{
			Points = points;
			m_points_textfx.SetText("Points: " + Points);
		}
		
		IEnumerator SetPointsAnimated(int points)
		{
			Points = points;
			
			m_points_textfx.PlayAnimation();
			
			yield return new WaitForSeconds(m_text_change_delay);
			
			m_points_textfx.SetText("Points: " + Points);
		}
	}
}