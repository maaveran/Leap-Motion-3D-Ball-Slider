using UnityEngine ;
using System.Collections ;

public class cursor : MonoBehaviour {
	public float speed = 5.0f;
	public GUIStyle boxStyle;
	bool showGUI = false;
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
	public bool startmenu = false;
	public bool tutorial = false;
	public bool credit = false;
	public bool quit = false;

	void Start ()
	{
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

		if (gesture.Type == EasyLeapGestureType.OPEN_FIST) {
			if (startmenu && gesture.State == EasyLeapGestureState.STATESTART){
				Debug.Log ("berhasil start menu");
				Application.LoadLevel ("level_config");
			}
			if (tutorial && gesture.State == EasyLeapGestureState.STATESTART){
				Application.LoadLevel ("tutorial");
			}
			if (credit && gesture.State == EasyLeapGestureState.STATESTART){
				Application.LoadLevel ("credit");
			}
			if (quit && gesture.State == EasyLeapGestureState.STATESTART){
				showGUI = true;
			}
		}

	}
	
	void Update ()
	{
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
		//transform.position = new Vector3 (127.94f+power*pxsLeapInput.GetHandAxis("Mouse X"),100.9f+powerx*pxsLeapInput.GetHandAxis("Mouse Y"), -39.498f);
		float x = pxsLeapInput.GetHandAxisStep("Tilt") * Time.deltaTime * speed;
		float y = pxsLeapInput.GetHandAxisStep("Rotation") * Time.deltaTime * speed;
		transform.Translate(y, x, 0);
	}



	void OnTriggerEnter(Collider col) 
	{
		if (col.gameObject.tag == "StartGame")
		{
			Debug.Log("tes");
			startmenu = true;
			col.GetComponent<Renderer>().material.color = Color.blue;
		}
		if (col.gameObject.tag == "tutorial")
		{
			Debug.Log("tes");
			tutorial = true;
			col.GetComponent<Renderer>().material.color = Color.blue;
		}
		if (col.gameObject.tag == "credit")
		{
			Debug.Log("tes");
			credit = true;
			col.GetComponent<Renderer>().material.color = Color.blue;
		}
		if (col.gameObject.tag == "quit")
		{
			quit = true;
			Debug.Log("tes");
			col.GetComponent<Renderer>().material.color = Color.blue;
		}
		if (col.gameObject.tag == "menu_awal")
		{
			Debug.Log("tes");
			col.GetComponent<Renderer>().material.color = Color.blue;
		}
	}
	void OnTriggerExit(Collider col){
		startmenu = false;
		credit = false;
		tutorial = false;
		quit = false;
		col.GetComponent<Renderer> ().material.color = Color.green;
	}


	void OnGUI(){
		if (showGUI) {
			GUI.Box (new Rect (200, 140, 950, 400), "Anda Yakin ingin keluar dari permainan" + "\n " +
			         "jika Iya miringkan tangan ke kiri \n\n Untuk kembali ke permainan " +
			         "miringkan tangan ke kanan", boxStyle);
			if (pxsLeapInput.GetHandAxis ("Rotation") > 0) {
				Destroy(gameObject);
			}
			else
			{
				Application.Quit();
			}
		}

	}
}