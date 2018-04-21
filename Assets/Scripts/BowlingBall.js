/* Copyright (c) 2007 Technicat, LLC */
/* Bowling ball */

var sunk = -100;

private var startPos:Vector3;
private var startRot:Vector3;

function Awake() {
	startPos = transform.localPosition;
	startRot = transform.localEulerAngles;
}

function ResetPosition() {
	transform.localPosition = startPos;
	transform.localEulerAngles = startRot;
	if (GetComponent.<Rigidbody>() != null) {
		GetComponent.<Rigidbody>().velocity = Vector3.zero;
		GetComponent.<Rigidbody>().angularVelocity = Vector3.zero;
	}
}

function IsSunk() {
	return transform.localPosition.y<sunk;
}