using UnityEngine;
using System.Collections;

public class menu_scroll : MonoBehaviour {

	string[] buttonNames = {"Start Game", "Tutorial","Credit","Exit"};
	bool[] buttons;
	public int currentSelection = 0; 
	public int a = 0;
	public int b = 0;
	/*public class GUIButton {
		public string controlName;
		public string text;
		public Rect rect;
	}*/

	//var selectedIndex = 0;
	
	//GUIButton[] buttons; // Array of buttons to navigate through; could generalize for any control type.
	//int current; 

//	int a = 1;
	//bool buttonPressed = false;
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
	
	// Use this for initialization
	void Start () {
		buttons = new bool[buttonNames.Length];
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
		//print ("Gesture! " + gesture.Type + " " + gesture.Id + " " + gesture.State +" " + gesture.Duration + " " + gesture.Position);
		/*if (gesture.Type == EasyLeapGestureType.CLOSE_FIST ) {
			// When the use key is pressed, the selected button will activate
			buttons[currentSelection] = true;
			print ("ahh");
		}*/
		if (gesture.Type == EasyLeapGestureType.PUSH) {
			//if( gesture.State == EasyLeapGestureState.STATESTART ){
			GUI.FocusControl(buttonNames[currentSelection + 1]);
			//}else{
			//	GUI.FocusControl(buttonNames[currentSelection + 0]);
			//}
			
		}
		
		if (gesture.Type == EasyLeapGestureType.PULL){
			if(gesture.State == EasyLeapGestureState.STATESTART){
				GUI.FocusControl(buttonNames[currentSelection + 1]);
			}else{
				GUI.FocusControl(buttonNames[currentSelection + 0]);
			}

		}
		


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

	void OnGUI(){
		for (int i = 0; i < buttonNames.Length; i++) {
			GUI.SetNextControlName(buttonNames[i]);
			buttons[i] = GUI.Button(new Rect(10, 70 + (20 * i), 80, 20), buttonNames[i]);
		}
		
		// Using button with keystroke

		
		// Button conditions
		if (buttons[0]) {
			print ("berhasil1");
		}
		if (buttons[1]) {
			// return to main menu
			print ("berhasil2");
		}
		
		// Cycling through buttons
		
	}


}
