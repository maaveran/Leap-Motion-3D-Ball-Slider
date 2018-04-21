/* Copyright (c) 2008 Technicat, LLC */

var audioThreshold = 10.0;

function OnCollisionEnter(collision) {
	var volume = collision.relativeVelocity.magnitude;
	if (volume > audioThreshold) {
		GetComponent.<AudioSource>().Play();
	}
}