using UnityEngine;
using System.Collections;

public class FollowHand : MonoBehaviour {
	
	private Leap.Controller leapController;
	private Leap.Frame mFrame;
	
	// Use this for initialization
	void Start () {
		leapController = new Leap.Controller();
	}
	
	// Update is called once per frame
	void Update () {
		mFrame = leapController.Frame ();
		if(mFrame.Hands.Count >= 1) {
			// deal with position
			float sphereRadius = mFrame.Hands[0].SphereRadius;
			Leap.Vector sphereCentre = mFrame.Hands[0].StabilizedPalmPosition;
			transform.position = Vector3.Slerp (transform.position,new Vector3(-sphereCentre.x,sphereCentre.y,sphereCentre.z),Time.deltaTime*10f);
			transform.localScale = Vector3.one*sphereRadius;
			//Deal with rotation
			Leap.Vector forward = mFrame.Hands[0].Direction;
			Leap.Vector up = mFrame.Hands[0].PalmNormal;
			Vector3 rotation = new Vector3(forward.Pitch*Mathf.Rad2Deg,forward.Yaw*Mathf.Rad2Deg,-up.Roll*Mathf.Rad2Deg);
			transform.localEulerAngles = Vector3.Slerp (transform.localEulerAngles,rotation,Time.deltaTime*80f);
		}
	}
	
	
}
