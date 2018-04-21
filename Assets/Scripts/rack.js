/* Copyright (c) 2007 Technicat, LLC */

var pin:Transform;

var distance = 1.0;

var rows = 4;

var knockedAngle = 45.0;

static var knockedOver = 0;

private var pins:Array;

function Start () {
	pins = new Array();
	var offset = Vector3.zero;
	for (var row=0; row<rows; ++row) {
		offset.z+=distance;
		offset.x=-distance*row/2;
		for (n=0; n<=row; ++n) {
			pins.push(Instantiate(pin, transform.position+offset, transform.rotation));
			offset.x+=distance;
		}
		
	}
}

function Update() {
	knockedOver = 0;
	for (var pin:Transform in pins) {
		if (transform.localEulerAngles.x>knockedAngle ||
			pin.transform.localEulerAngles.z>knockedAngle) 
			++knockedOver;
		}
	}

function ResetPosition() {
	for (var pin:Transform in pins) {
		var ball:BowlingBall = pin.GetComponent(BowlingBall);
		ball.ResetPosition();
	}
	knockedOver = 0;
}
