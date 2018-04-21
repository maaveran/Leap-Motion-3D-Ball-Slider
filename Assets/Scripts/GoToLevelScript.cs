using UnityEngine;
using System.Collections;
using MadLevelManager;

public class GoToLevelScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var sprite = GetComponent<MadSprite> ();
		sprite.onTap = sprite.onMouseUp =  (s) => MadLevel.LoadLevelByName ("main_menu_final");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
