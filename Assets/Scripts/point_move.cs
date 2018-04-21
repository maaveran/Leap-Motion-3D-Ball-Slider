using UnityEngine;
using System.Collections;

public class point_move : MonoBehaviour {


	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		if (pxsLeapInput.GetHandAxis("Mouse X") < 0) {
			transform.Translate(-Vector2.right*1f*Time.deltaTime);
			//Debug.Log(transform.Translate);
		}
		if (pxsLeapInput.GetHandAxis("Mouse X") > 0) {
			transform.Translate(Vector2.right*1f*Time.deltaTime);
		}
		if (pxsLeapInput.GetHandAxis("Mouse Y") < 0) {
			transform.Translate(-Vector2.up*1f*Time.deltaTime);
		}
		if (pxsLeapInput.GetHandAxis("Mouse Y") > 0) {
			transform.Translate(Vector2.up*1f*Time.deltaTime);
		}



		Vector3 mouseworld = Camera.main.ScreenToWorldPoint(GameObject.Find("Cursor").transform.localPosition);
		
		Vector2 mousepos = new Vector2 (mouseworld.x , mouseworld.y);
		Debug.DrawRay(transform.position, mouseworld, Color.green);
		Debug.Log (mousepos);
		
		Vector2 dir = Vector2.zero;
		
//		RaycastHit2D hit = Physics2D.Raycast (mousepos,dir, Mathf.Infinity,LayerMask);
		//Debug.DrawRay (transform,mousepos.x,Color.green);
		
		if (Physics2D.Raycast(mousepos,dir)) {
			//Debug.Log("tes");
			
		/*	if( hit.collider.gameObject.tag == "StartGame"){
				Debug.Log("it's working guys");
			}
			if(hit.collider.gameObject.tag == "quit"){
				Debug.Log("it's working BOLD");
			}
		} */

		}
	}

}

	
//Ray testRay = Camera.main.ScreenPointToRay(GameObject.Find("Player").transform.position);
//Vector2 mousePos = new Vector2 (testRay.origin.x, testRay.origin.y);
//Debug.Log (mousePos);
//float x = GameObject.Find ("Player").transform.position.x;
//float x = GameObject.Find ("Player").transform.position.y;	
//Debug.Log (testRay);
//Debug.DrawRay (testRay.origin, testRay.direction * 10, Color.yellow);
//RaycastHit hit;

//	if(Physics.Raycast(testRay, out hit)){
//		if(hit.collider.gameObject.tag == "StartGame"){
//		Debug.Log(hit.collider);
//		}
//	}

//Vector2 mousePosition = new Vector2()

/*
		Vector3 mouseworld = Camera.main.ScreenToWorldPoint(GameObject.Find("Cursor").transform.localPosition);

		Vector2 mousepos = new Vector2 (mouseworld.x , mouseworld.y);
		Debug.DrawRay(transform.position, mouseworld, Color.green);
		Debug.Log (mousepos);

		Vector2 dir = Vector2.zero;

		RaycastHit2D hit = Physics2D.Raycast (mousepos,dir);
		//Debug.DrawRay (transform,mousepos.x,Color.green);

			if (hit != null && hit.collider != null) {
			//Debug.Log("tes");
				
			if(hit.collider != null && hit.collider.gameObject.tag == "StartGame"){
				Debug.Log("it's working guys");
				}
			if(hit.collider != null && hit.collider.gameObject.tag == "quit"){
				Debug.Log("it's working BOLD");
				}
			}
		*/




