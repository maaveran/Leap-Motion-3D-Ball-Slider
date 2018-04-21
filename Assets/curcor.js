/* Copyright (c) 2007 Technicat, LLC */

var power = 3.0;
var powerx = 2.0;

function FixedUpdate () {
	var force:Vector3 = new Vector3(powerx*pxsLeapInput.GetHandAxis("Rotation"),
					0,
					power*pxsLeapInput.GetHandAxis("Depth")
				       );
	GetComponent.<Rigidbody>().AddForce(force);
}

