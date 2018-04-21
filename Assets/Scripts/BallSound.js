// bowling ball rolling sound

// other physic materials we might collide against
var physmats:PhysicMaterial[];
var rollSounds:AudioClip[];

var slowspeed:float = 0.001;

var rollVolume:float = 1000.0;

var minbounce:float = 0;
var collisionScale:float = 1;

var minsound:float = 0.1;
var maxsound:float = 1.0;

private var trans:Transform;
private var audiosource:AudioSource;

function Start() {
	trans = transform;
	audiosource = GetComponent.<AudioSource>();
}

function OnCollisionEnter(collider:Collision) {
//	var speed:float = collider.relativeVelocity.sqrMagnitude;
	var speed:float = Vector3.Dot(collider.relativeVelocity,collider.contacts[0].normal);
	if (speed>minbounce) {
		//Debug.Log(collider.gameObject.name);
		// a bit of hack - assume collision sound is in the other object
		var source:AudioSource = collider.gameObject.GetComponent.<AudioSource>();
		if (source != null && source.clip != null) {
			minsound = 0; // source.minVolume;
			var vol:float = Mathf.Max(minsound,Mathf.Min(maxsound,collisionScale*speed));
		//	audio.PlayOneShot(source.clip,vol);
		// avoid interfering with rolling sound
			AudioSource.PlayClipAtPoint(source.clip,trans.position,vol);
		}
	}
}

function OnCollisionStay (collider:Collision) {
	for (var i:int = 0; i<physmats.length; ++i) {
		if (collider.collider.sharedMaterial == physmats[i]) {
			if (audiosource.clip != rollSounds[i]) {
				//audiosource.Stop(); // necessary when we change clips?
				audiosource.clip = rollSounds[i];
				audiosource.Play();
			}
			var speed:float = collider.relativeVelocity.sqrMagnitude;
			audiosource.volume = speed*rollVolume; // adjust volume based on speed
			if (!audiosource.isPlaying) {
				if (speed>slowspeed) {
					audiosource.Play();
				}
			} else {
				if (speed<slowspeed) {
				//	audiosource.Stop();
				//	audiosource.Pause();
					audiosource.volume = 0.0; // just so we can see in the Inspector
				}
			}
			return; // take the first rolling sound
		}
	}
	audiosource.volume = 0.0;
	//audiosource.Stop(); // no contact with known material, no sound
}

function ResetPosition() {
	audiosource.volume = 0.0;
	//audiosource.Stop();
}

function OnCollisionExit(collider:Collision) {
		//audiosource.Stop();
		audiosource.volume = 0.0;
}
