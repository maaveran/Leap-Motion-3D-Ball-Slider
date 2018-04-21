using UnityEngine;
using System.Collections;

public class sound_pin : MonoBehaviour {
	private string hitobject;
	AudioClip snd;
	bool played;
	// Use this for initialization
	void Start () {
	//	Start = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(UnityEngine.Collision hit)
	{
		hitobject = hit.gameObject.tag;
		if (hitobject == "player") {
		//	audio.PlayOneShot(snd);
		} else {
		//	audio.Stop();
		}

	}
}
