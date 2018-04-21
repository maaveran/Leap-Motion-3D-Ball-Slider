using UnityEngine;
using System.Collections;

public class menu : MonoBehaviour {

	public Texture backgroundTexture;

	//public GUISkin customSkin;

	void OnGUI(){

	//	customSkin = GUISkin;

		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

		if(GUI.Button(new Rect (Screen.width * .70f, Screen.height  * .5f, Screen.width * .2f, Screen.height * .1f),"Start Game"));
		{
		}
		if(GUI.Button(new Rect (Screen.width * .70f, Screen.height  * .62f, Screen.width * .2f, Screen.height * .1f),"Start Game"));
		{
		}
		if(GUI.Button(new Rect (Screen.width * .70f, Screen.height  * .74f, Screen.width * .2f, Screen.height * .1f),"Start Game"));
		{
		}
		if(GUI.Button(new Rect (Screen.width * .70f, Screen.height  * .86f, Screen.width * .2f, Screen.height * .1f),"Start Game"));
		{
		}


}
}
