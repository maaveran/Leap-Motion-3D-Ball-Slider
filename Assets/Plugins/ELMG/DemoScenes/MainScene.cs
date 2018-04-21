using UnityEngine;
using System.Collections;

public class MainScene : MonoBehaviour {
	
	public bool circle = false;
	public bool keytap = false;
	public bool twofingerkeytap = false;
	public bool threefingerkeytap = false;
	public bool screentap = false;
	public bool twofingerscreentap = false;
	public bool threefingerscreentap = false;
	public bool swipe = false;
	public bool numbers = false;
	public bool closefist = false;
	public bool openfist = false;
	public bool push = false;
	public bool pull = false;
	public bool doubleinwardswipe = false;
	public bool doubleoutwardswipe = false;
	public bool clap = false;
	public bool steeringwheel = false;
	bool showGUI = false;
	public int i ;

	public GUIStyle customGui;
	public GUIStyle customGui2;
	public GUIStyle customGui3;
	public GUIStyle customGui4;
	public GUIStyle customGui_txt;
	public AudioClip popAudio;
//	string[] menuOptions = {"Start Game","Tutorial","Credits","Exit"}; 
	public AudioSource Audio;
	public bool exit = false;
	public int a = 0;

	// Use this for initialization
	void Start () {

		i = 0;
		Audio = GetComponent<AudioSource> ();

		ELGManager.GestureRecognised += onGestureRecognised;
		ELGManager.circleGestureRegistered = circle;
		ELGManager.keytapGestureRegistered = keytap;
		ELGManager.screentapGestureRegistered = screentap;
		ELGManager.swipeGestureRegistered = swipe;
		ELGManager.numbersGestureRegistered = numbers;
		ELGManager.closeFistRegistered = closefist;
		ELGManager.openFistRegistered = openfist;
		ELGManager.pushGestureRegistered = push;
		ELGManager.pullGestureRegistered = pull;
		ELGManager.doubleInwardsSwipeGestureRegistered = doubleinwardswipe;
		ELGManager.doubleOutwardsSwipeGestureRegistered = doubleoutwardswipe;
		ELGManager.clapGestureRegistered = clap;
		ELGManager.twoFingerKeytapRegistered = twofingerkeytap;
		ELGManager.threeFingerKeytapRegistered = threefingerkeytap;
		ELGManager.twoFingerScreentapRegistered = twofingerscreentap;
		ELGManager.threeFingerScreentapRegistered = threefingerscreentap;
		ELGManager.steeringWheelRegistered = steeringwheel;
	}
	
	void OnDestroy() {
		ELGManager.GestureRecognised -= onGestureRecognised;
	}
	
	void onGestureRecognised(EasyLeapGesture gesture) {
		print ("Gesture! " + gesture.Type + " " + gesture.Id + " " + gesture.State + " " + gesture.Duration + " " + gesture.Position);
		if (gesture.Type == EasyLeapGestureType.PUSH) {
			if (i < 4) {
				if (gesture.State == EasyLeapGestureState.STATESTART && gesture.Duration == 0) {
					i += 1;
				
				}
			}

		}

		if (gesture.Type == EasyLeapGestureType.PULL) {
				if (gesture.State == EasyLeapGestureState.STATESTART && gesture.Duration == 0) {
					if(  i == 4)
					{ 	
					i -= 1;
					}else if( i == 3){
					i -= 1;
					}else if( i == 2){
					i -= 1;
					}
			}
		}
		if (gesture.Type == EasyLeapGestureType.CLAP && i == 1) {
			Application.LoadLevel ("level_config");
		} else if (gesture.Type == EasyLeapGestureType.CLAP && i == 2) {
			Application.LoadLevel ("tutorial");
		} else if (gesture.Type == EasyLeapGestureType.CLAP && i == 3) {
			Application.LoadLevel ("credit");
		} else if (gesture.Type == EasyLeapGestureType.CLAP && i == 4) {
			Application.LoadLevel ("Quit");
		}else if (gesture.Type == EasyLeapGestureType.CLAP ){
			a = 0;
		}else{
			Debug.Log("something miss");
		}
	}

	void OnGUI ()
	{
		if (GUI.Button (new Rect (Screen.width/3+100, 200, 300, 60), "Start Game", customGui2))
			Debug.Log ("Clicked the button with text");
		if (GUI.Button (new Rect (Screen.width/3+100, 300, 300, 60), "Tutorial", customGui2))
			Debug.Log ("Clicked the button with text");
		if (GUI.Button (new Rect (Screen.width/3+100, 400, 300, 60), "Credit", customGui2))
			Debug.Log ("Clicked the button with text");
		if (GUI.Button (new Rect (Screen.width/3+100, 500, 300, 60), "Quit", customGui2))
			Debug.Log ("Clicked the button with text");

		switch (i) {

		case 1:
			GUI.SetNextControlName ("Start Game");
			if (GUI.Button (new Rect (Screen.width/3+100 , 200, 300, 60), "Start Game", customGui))
			//GetComponent<AudioSource>().PlayOneShot(popAudio,0.2F);
			//GetComponent<AudioSource>().Play();
			//GetComponent<AudioSource>().Stop();
				Audio.PlayOneShot(popAudio,0.2F);
			Audio.Play();
			//Audio.loop(false);
			break;
	//		a=0;
		case 2:
			GUI.SetNextControlName ("Tutorial");
			if (GUI.Button (new Rect (Screen.width/3+100, 300, 300, 60), "Tutorial",customGui))
				GetComponent<AudioSource>().PlayOneShot(popAudio);
			break;
		case 3:
			GUI.SetNextControlName ("Credits");
			if (GUI.Button (new Rect (Screen.width/3+100, 400, 300, 60), "Credit", customGui))
				GetComponent<AudioSource>().PlayOneShot(popAudio);
			break;

		case 4:
			GUI.SetNextControlName ("Exit");
			if (GUI.Button (new Rect (Screen.width/3+100, 500, 300, 60), "Quit",customGui))
				GetComponent<AudioSource>().PlayOneShot(popAudio);	
			break;
		default:

			break;
		}



		
		//GUI.FocusControl (menuOptions[i]);
	}
	
	
	void Update() {
		ELGManager.circleGestureRegistered = circle;
		ELGManager.keytapGestureRegistered = keytap;
		ELGManager.screentapGestureRegistered = screentap;
		ELGManager.swipeGestureRegistered = swipe;
		ELGManager.numbersGestureRegistered = numbers;
		ELGManager.closeFistRegistered = closefist;
		ELGManager.openFistRegistered = openfist;
		ELGManager.pushGestureRegistered = push;
		ELGManager.pullGestureRegistered = pull;
		ELGManager.doubleInwardsSwipeGestureRegistered = doubleinwardswipe;
		ELGManager.doubleOutwardsSwipeGestureRegistered = doubleoutwardswipe;
		ELGManager.clapGestureRegistered = clap;
		ELGManager.twoFingerKeytapRegistered = twofingerkeytap;
		ELGManager.threeFingerKeytapRegistered = threefingerkeytap;
		ELGManager.twoFingerScreentapRegistered = twofingerscreentap;
		ELGManager.threeFingerScreentapRegistered = threefingerscreentap;
		ELGManager.steeringWheelRegistered = steeringwheel;
	}
}
