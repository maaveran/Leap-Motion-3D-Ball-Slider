using UnityEngine;
using System.Collections;

public class pause_menu : MonoBehaviour {
	
	// Use this for initialization
	public GUIStyle boxStyle;
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
	public bool showGUI = false;
	
	// Use this for initialization
	void Start () {
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
		if (gesture.Type == EasyLeapGestureType.CLAP) {

			showGUI = true;
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
		if (showGUI) {
			GUI.Box (new Rect (200, 140, 950, 400), "Anda Yakin ingin keluar dari permainan" + "\n " +
				"jika Iya miringkan tangan ke kiri \n\n Untuk kembali ke permainan " +
				"miringkan tangan ke kanan", boxStyle);
			if (pxsLeapInput.GetHandAxis ("Rotation") > 0) {
				Destroy (gameObject);
			} else {
				Application.Quit ();
			}
		}
	}
}
