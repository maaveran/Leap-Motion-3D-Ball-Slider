using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial_first : MonoBehaviour {

	// Use this for initialization
	public AudioClip popAudio;
	public bool hasplayed = false;
	public bool show;
	public bool memo = false;
	public bool show1;
	public bool show2;
	public bool show3;
	public GUIStyle boxStyle;
	public GUIStyle menuStyleScore;
	public GameObject a;
	public GameObject b;
	public GameObject c;
	public GameObject d;
	public GameObject pin;
	public GameObject pin1;
	public GameObject pin2;
	public GameObject pin3;
	public string hitobject;
	public Slider healthBarSlider;
	public GameObject duri;
	public GameObject duri1;
	public GameObject duri2;
	public GameObject cube;
	public float score;

	void Start ()
	{
		ShowBox ();
	}
	
	void ShowBox ()
	{
		// show label
		show = true;
		
		// cancel invoking method if already set to call after 3 seconds
		CancelInvoke("HideBox");
		
		// will call HideBox () after 3 sec
		Invoke ("HideBox", 10.0F);
	}

	void ShowBox1 ()
	{
		// show label

		show1 = true;
		// cancel invoking method if already set to call after 3 seconds
		CancelInvoke("HideBox1");
		
		// will call HideBox () after 3 sec
		Invoke ("HideBox1", 10.0F);
	}
	void ShowBox2 ()
	{
		// show label
		//pin3.SetActive(true);
		show2 = true;
		CancelInvoke("HideBox2");
		
		// will call HideBox () after 3 sec
		Invoke ("HideBox2", 10.0F);
		// cancel invoking method if already set to call after 3 seconds

	}
	void HideBox ()
	{
		// dont show label
		show = false;
		a.SetActive (true);

	}

	void HideBox1 ()
	{
		// dont show label
		show1 = false;
		memo = true;
		pin.SetActive (true);

	}

	void HideBox2 ()
	{
		// dont show label
		show2 = false;
		duri.SetActive(true);
		duri1.SetActive(true);
		duri2.SetActive(true);
		cube.SetActive(true);
		
	}
	

	void OnGUI ()
	{
		if(show) 
		{

			a.SetActive(false);
			b.SetActive(false);
			c.SetActive(false);
			d.SetActive(false);
			pin.SetActive(false);
			pin1.SetActive(false);
			pin2.SetActive(false);
			pin3.SetActive(false);
			duri.SetActive(false);
			duri1.SetActive(false);
			duri2.SetActive(false);
			cube.SetActive(false);
			GUI.Box(new Rect((Screen.width/2)-250,(Screen.height/2)-200,500,200) , "Tutorial \n\n Pada tutorial ini pemain \ndiharuskan mengenai 4 kubus \n yang tersedia",boxStyle);
		}

		if(show1) 
		{
			
			a.SetActive(false);
			b.SetActive(false);
			c.SetActive(false);
			d.SetActive(false);
			
			GUI.Box(new Rect((Screen.width/2)-250,(Screen.height/2)-200,500,200) , "Tutorial II \n\n Pada tutorial ini pemain \ndiharuskan mengenai 4 pin \n yang tersedia \n nantinya dalam permainan berfungsi menambahkan nilai",boxStyle);
			hasplayed = true;
			if(hasplayed == true){
				GetComponent<AudioSource>().PlayOneShot(popAudio);
				hasplayed = false;
			}
		}
		if (show2) {
			GUI.Box(new Rect((Screen.width/2)-250,(Screen.height/2)-200,500,200) , "Tutorial III \n\n Pada tutorial ini pemain \ndiharuskan mengenai  cube  \n yang tersedia \n dan menghindari duri yang ada",boxStyle);
			hasplayed = true;
			if(hasplayed == true){
				GetComponent<AudioSource>().PlayOneShot(popAudio);
				hasplayed = false;
			}
			memo = false;
		}
		if (show3) {
			GUI.Box(new Rect((Screen.width/2)-250,(Screen.height/2)-200,500,200) , "Selamat Tutorial \n \n Telah berhasil di selesaikan \n\n Tepuk tangan untuke keluar dari permainan",boxStyle);
			hasplayed = true;
			if(hasplayed == true){
				GetComponent<AudioSource>().PlayOneShot(popAudio);
				hasplayed = false;
			}
		}

		if (memo) {
			GUI.Box (new Rect (1170, 127, 80, 30), "Score : " + score.ToString ("0"), menuStyleScore);
		}
	}

	void OnCollisionEnter(UnityEngine.Collision hit)
	{
		hitobject = hit.gameObject.tag;
		if (hitobject == "a")
		{
			b.SetActive(true);
			a.GetComponent<Renderer>().material.color = Color.green;
		}
		if (hitobject == "b")
		{
			Debug.Log("tes");
			c.SetActive(true);
			b.GetComponent<Renderer>().material.color = Color.green;
		}
		if (hitobject == "c")
		{
			Debug.Log("tes");
			d.SetActive(true);
			c.GetComponent<Renderer>().material.color = Color.green;
		}
		if (hitobject == "d")
		{
			//col.GetComponent<Renderer>().material.color = Color.blue;
			d.GetComponent<Renderer>().material.color = Color.green;
			a.SetActive(false);
			b.SetActive(false);
			c.SetActive(false);
			d.SetActive(false);
			ShowBox1();
		}
		if (hitobject == "pin")
		{
			score += 1.0f;
			pin1.SetActive(true);
			Destroy(hit.gameObject);
		}
		if (hitobject == "pin1")
		{
			score += 1.0f;
			Debug.Log("tes");
			pin2.SetActive(true);
			Destroy(hit.gameObject);
		}
		if (hitobject == "pin2")
		{
			score += 1.0f;
			Debug.Log("tes");
			pin3.SetActive(true);
			Destroy(hit.gameObject);
		}
		if (hitobject == "pin3")
		{
			score += 1.0f;
			Debug.Log("tes");
			Destroy(hit.gameObject);
			ShowBox2();
		}
		if(hitobject == "duri_hati")
		{
			Destroy(hit.gameObject);
			healthBarSlider.value -=.2f;
		}
		if(hitobject == "cube")
		{
			Destroy(hit.gameObject);
			show3=true;;
		}

	}
}
