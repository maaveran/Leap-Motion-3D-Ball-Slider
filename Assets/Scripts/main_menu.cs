/*using UnityEngine;
using System.Collections;

public class main_menu : MonoBehaviour {

	public GUIStyle mystyle;
	public Texture backgroundTexture;
	public float guiplacementY;
	public float guiplacementY1;
	public float guiplacementY2;

	public float[] menuOptions;
	public int selectedIndex;

	void Start(){
		menuOptions = new float[4.0];
		menuOptions[0] = "Start Game";
		menuOptions[1] = "Tutorial";
		menuOptions[2] = "Credit";
		menuOptions[3] = "Quit";

		selectedIndex = 0;

	}

	function menuSelection (menuItems, selectedItem, direction){
		if (direction == "up")
		{

		}
	}
	// Use this for initialization
	void OnGUI(){

		//GUI.skin = newSkin;

		GUI.DrawTexture(new Rect (0,0,Screen.width,Screen.height),backgroundTexture);

		if(GUI.Button(new Rect(Screen.width*.35f,Screen.height*.5f,Screen.width*.3f,Screen.height*.1f),"Play Game"));

		if(GUI.Button(new Rect(Screen.width*.35f,Screen.height*guiplacementY,Screen.width*.3f,Screen.height*.1f),"Tutorial"));

		if(GUI.Button(new Rect(Screen.width*.35f,Screen.height*guiplacementY1,Screen.width*.3f,Screen.height*.1f),"Credit"));

		if(GUI.Button(new Rect(Screen.width*.35f,Screen.height*guiplacementY2,Screen.width*.3f,Screen.height*.1f),"Quit"));
	}
}
*/