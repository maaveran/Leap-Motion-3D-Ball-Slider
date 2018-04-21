using UnityEngine;
using System.Collections;

public class raycast_slide : MonoBehaviour {



	public float threshold = 0.88f;
	public float slide_speed = 5f;
	public float magnitude;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 slideDirection = Vector3.zero;
		RaycastHit hitInfo = new RaycastHit ();

		if(Physics.Raycast(transform.position, Vector3.down,out hitInfo))
		{
			if(hitInfo.collider.tag != "slide")
			{
				return;
			}
			if(hitInfo.normal.y < threshold  ){
			//	transform.position threshold = new Vector3 (hitInfo.normal.x,0,hitInfo.normal.z);
			}
		}
		//if (threshold.magnitude < slide_speed) {
			//moveDirection += 
		//}

	}
}
