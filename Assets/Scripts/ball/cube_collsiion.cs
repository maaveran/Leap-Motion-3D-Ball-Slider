using UnityEngine;
using System.Collections;

public class cube_collsiion : MonoBehaviour {


	 public float timer;
	private float triggerTimer;
	//private float timerLimit = 3.0f;
	RaycastHit hit;
	public float buttonHover;
	// Use this for initialization
	void Start () {
	timer = 3.0f;
	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnTriggerEnter(Collider col) 
	{
		if (col.gameObject.tag == "StartGame")
		{
			timer -= Time.deltaTime;
		}
	} 

	void OnGUI(){
	
		if (timer != 0) {

			GUI.Box (new Rect (20, 30, 80, 30), "Waktu :" + timer.ToString ("0"));
		}
	}

	/*void OnTriggerStay (Collider col)
	{

		if(col.gameObject.name == "StarGame")
		{//	timer -= Time.deltaTime;
			Debug.Log("testes");
			triggerTimer += Time.deltaTime;
		//if (timer < 0){
			if(triggerTimer > timerLimit){
			Debug.Log("loading....");
			}
		}
		*/
}