using UnityEngine;
using UnityEditor;

namespace TextFx
{
	static internal class MenuOptionsTextFxNative
	{
		[MenuItem("GameObject/TextFx/Text", false)]
		static public void AddTextFxNativeInstance ()
		{
			GameObject go = new GameObject ("TextFx Text");;
			
			TextFxNative textfxComp = go.AddComponent<TextFxNative>();
			textfxComp.SetText("New TextFx");
			
			Selection.activeGameObject = go;
		}
	}
}
