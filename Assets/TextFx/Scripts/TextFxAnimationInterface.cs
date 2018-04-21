using UnityEngine;
using System.Collections;

namespace TextFx
{
	public interface TextFxAnimationInterface
	{
		TextFxAnimationManager AnimationManager { get; }

		string Text { get; }

		int LayerOverride { get; }			// A layer index to be applied to any externally used elements (particle systems for instance)
		float MovementScale { get; }		// A scaling factor to use on all positional movements to normalise with rest of assets
		string AssetNameSuffix { get; }		// A suffix used to find specific asset alternative versions.

		GameObject GameObject { get; }

		// Call to redraw the mesh with the provided mesh vertex positions and colours
		void UpdateTextFxMesh(Vector3[] verts, Color[] cols);

		// A guaranteed method for updating the text for gui renderer
		void SetText(string text);
	}
}