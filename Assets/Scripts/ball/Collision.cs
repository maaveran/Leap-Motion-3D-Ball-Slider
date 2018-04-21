using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MadLevelManager;

public class Collision : MonoBehaviour {

	public AudioClip popAudio;
	public bool hasplayed = false;
	public float score;
	public float timer;
	public int  showgui;
	public GUIStyle menuStyleScore;
	public Slider healthBarSlider;
	public GUIStyle menuStyle;
	public GUIStyle boxStyle;
	public float level1;
	public float level2;
	public float level3;
	public float level4;
	public Texture2D myGUITexture;
	public float level5;
	public float level;
	private string hitobject;
	public float scoring;
	private int score_check;	
	//public bool useGravity;
	//public float yesnoquestion;

	/*public Slider healtBar; 
	public int Health;

	*/
	void Start(){
		score_check = 0;
		showgui = 0;
		level = 0.0f;
		score = 0.0f;
		level1 = 15f;
		level2 = 18f;
		level3 = 21f;
		level4 = 24f;
		level5 = 27f;

		if (Application.loadedLevelName == "level2") {
			timer = 85.0f;
		} else if (Application.loadedLevelName == "level1") {
			timer = 75.0f;
		}else if (Application.loadedLevelName == "level3") {
			timer = 95.0f;
		} else if (Application.loadedLevelName == "level4") {
			timer = 105.0f;
		} else {
			timer = 115.0f;
		}
		if (Application.loadedLevelName == "level2") {
			level = level2;
		} else if (Application.loadedLevelName == "level1") {
			level =level1;
		}else if (Application.loadedLevelName == "level3") {
			level = level3;
		} else if (Application.loadedLevelName == "level4") {
			level = level4;
		} else {
			level = level5 ;
		}
		//yesnoquestion = pxsLeapInput.GetHandAxis("Rotation");

	}


	void OnCollisionEnter(UnityEngine.Collision hit)
	{
		hitobject = hit.gameObject.tag;
		if(hitobject == "pin")
		{
			hasplayed = true;
			if(hasplayed == true){
				GetComponent<AudioSource>().PlayOneShot(popAudio);
				hasplayed = false;
			}
			score += 1.0f;
			scoring = PlayerPrefs.GetFloat("score");

			Destroy(hit.gameObject,0.9f);
		}
		if(hitobject == "duri_hati")
		{
			Destroy(hit.gameObject);
			healthBarSlider.value -=.2f;
		}
		if(hitobject == "finished" && score > level  )
		{
			showgui += 1;
		}
	}



	void OnGUI()
		{ 
		if (timer != 0) {
			GUI.Box (new Rect (1170, 127, 80, 30), "Score : " + score.ToString ("0"), menuStyleScore);

			GUI.Box (new Rect (20, 30, 80, 30), "Waktu :" + timer.ToString ("0"), menuStyleScore);
		}
		timer -= Time.deltaTime;

		if (timer < 0 && score <= level || healthBarSlider.value < 0.0f) {
			//Time.timeScale = 0;
			showgui = 0;
			timer = 0;
			//Debug.Log("tes");
			//Time.timeScale = 0;
			GUI.Box (new Rect (200, 140, 950, 400), "Anda kalah, Score : \n" + score.ToString ("0") + "\n " +
				"Untuk kembali ke menu awal miringkan tangan ke kiri \n\n Untuk mengulangi permainan " +
			         "miringkan tangan ke kanan", boxStyle);
			//Time.timeScale = 1;

			if (pxsLeapInput.GetHandAxis ("Rotation") > 0) {
				//Debug.Log("kanan");
			//		Time.timeScale = 1;
				if (Application.loadedLevelName == "level1") {
					Application.LoadLevel ("level1");
				} else if (Application.loadedLevelName == "level2") {
					Application.LoadLevel ("level2");
				} else if (Application.loadedLevelName == "level3") {
					Application.LoadLevel ("level3");
				} else if (Application.loadedLevelName == "level4") {
					Application.LoadLevel ("level4");
				} else {
					Application.LoadLevel ("level5");
				}
			}
			if (pxsLeapInput.GetHandAxis ("Rotation") < 0) {
				Application.LoadLevel("main_menu_final");
			}
		}

		if (score > level && showgui > 0 || timer < 0 && score >= level) {
			timer = 0;
			//Time.timeScale = 0;
			GUI.Box (new Rect (200, 140, 950, 400), "Anda Menang, Score : \n" + score.ToString ("0") + "\n Untuk kembali ke menu awal miringkan tangan ke kiri \n\n Untuk lanjut ke level berikutnya miringkan tangan ke kanan", boxStyle);
			if (pxsLeapInput.GetHandAxis ("Rotation") > 0) {
				//Debug.Log("kanan");
			//	Time.timeScale = 1;
				if (Application.loadedLevelName == "level1") {
					Application.LoadLevel ("level2");
				} else if (Application.loadedLevelName == "level2") {
					Application.LoadLevel ("level3");
				} else if (Application.loadedLevelName == "level3") {
					Application.LoadLevel ("level4");
				} else if (Application.loadedLevelName == "level4") {
					Application.LoadLevel ("level5");
				} else {

				}
			}
			if (pxsLeapInput.GetHandAxis ("Rotation") < 0) {
				//MadLevel.LoadLevelByName ("Level Select Screen");
				Application.LoadLevel("main_menu_final");
			}
		}

		if (timer <= 0 && score >= level) {
			if (Application.loadedLevelName == "level2") {
				if (score > 15) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_3", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				} else if (score > 10) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					
					//MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
				} else if (score > 5) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				}
			} else if (Application.loadedLevelName == "level1") {
				if (score > 18) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_3", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				} else if (score > 13) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					
					//MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
				} else if (score > 7) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				}
			} else if (Application.loadedLevelName == "level3") {
				if (score > 21) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_3", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				} else if (score > 18) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					
					//MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
				} else if (score > 15) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				}
			} else if (Application.loadedLevelName == "level4") {

				if (score > 24) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_3", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				} else if (score > 21) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					
					//MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
				} else if (score > 18) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				}
			} else {
				level = level5;
				if (score > 27) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_3", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				} else if (score > 24) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_2", true);
					
					//MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
				} else if (score > 21) {
					MadLevelProfile.SetLevelBoolean (MadLevel.currentLevelName, "star_1", true);
					
					MadLevelProfile.SetCompleted (MadLevel.currentLevelName, true);
				}
			

		
			}




			//MadLevel.LoadLevelByName("Level Select Screen");
		




		}
	



	
	}	}