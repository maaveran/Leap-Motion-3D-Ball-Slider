#pragma strict
// just assume single-player 

var font:Font;

function OnGUI() {
	if (font) GUI.skin.font = font;
	for (var f:int=0; f<10; f++) {
		var score:String="";
		var roll1:int = StateMachine.players[0].scores[f].ball1;
		var roll2:int = StateMachine.players[0].scores[f].ball2;
		var roll3:int = StateMachine.players[0].scores[f].ball3;
		switch (roll1) {
			case -1: score += " "; break;
			case 10: score +="X"; break;
			default: score += roll1;
		}
		score+="/";
		if (StateMachine.IsSpare(f,0)) {
			score +="I";
		} else {
			switch (roll2) {
				case -1: score += " "; break;
				case 10: score +="X"; break;
				default: score += roll2;
			}
		}
		if (f==9) {
			score+="/";
			if (roll2+roll3 == 10) {
				score+="/";
			} else {
				switch (roll3) {
					case -1: score += " "; break;
					default: score += roll3;
				}
			}
		}
		GUI.Label(Rect(f*30+5,5,50,20),score);
	}
}