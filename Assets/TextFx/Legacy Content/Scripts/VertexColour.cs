/**
	TextFx VertexColour class.
	Defines four colours for each vertex in a Quad mesh. Used to apply a specific vertex colour to each corner of a letters mesh.
**/
using UnityEngine;

namespace TextFx.LegacyContent
{
	[System.Serializable]
	public class VertexColour
	{
		public Color top_left = Color.white;
		public Color top_right = Color.white;
		public Color bottom_right = Color.white;
		public Color bottom_left = Color.white;
		
		public VertexColour(){}
		
		public VertexColour(Color init_color)
		{
			top_left = init_color;
			top_right = init_color;
			bottom_right = init_color;
			bottom_left = init_color;
		}
		
		public VertexColour(Color tl_colour, Color tr_colour, Color br_colour, Color bl_colour)
		{
			top_left = tl_colour;
			top_right = tr_colour;
			bottom_right = br_colour;
			bottom_left = bl_colour;
		}
		
		public VertexColour(VertexColour vert_col)
		{
			top_left = vert_col.top_left;
			top_right = vert_col.top_right;
			bottom_right = vert_col.bottom_right;
			bottom_left = vert_col.bottom_left;
		}
		
		public VertexColour Clone()
		{
			VertexColour vertex_col = new VertexColour();
			vertex_col.top_left = top_left;
			vertex_col.top_right = top_right;
			vertex_col.bottom_right = bottom_right;
			vertex_col.bottom_left = bottom_left;
			
			return vertex_col;
		}
		
		public VertexColour Add(VertexColour vert_col)
		{
			VertexColour v_col = new VertexColour();
			v_col.bottom_left = bottom_left + vert_col.bottom_left;
			v_col.bottom_right = bottom_right + vert_col.bottom_right;
			v_col.top_left = top_left + vert_col.top_left;
			v_col.top_right = top_right + vert_col.top_right;
			
			return v_col;
		}
		
		public VertexColour Sub(VertexColour vert_col)
		{
			VertexColour v_col = new VertexColour();
			v_col.bottom_left = bottom_left - vert_col.bottom_left;
			v_col.bottom_right = bottom_right - vert_col.bottom_right;
			v_col.top_left = top_left - vert_col.top_left;
			v_col.top_right = top_right - vert_col.top_right;
			
			return v_col;
		}
		
		public VertexColour Multiply(float factor)
		{
			VertexColour v_col = new VertexColour();
			v_col.bottom_left = bottom_left * factor;
			v_col.bottom_right = bottom_right * factor;
			v_col.top_left = top_left * factor;
			v_col.top_right = top_right * factor;
			
			return v_col;
		}
	}
}