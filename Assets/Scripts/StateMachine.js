/* Copyright (c) 2007 Technicat, LLC */

var ball:Transform;

var cheer:AudioClip;
var boo:AudioClip;
var OK:AudioClip;

private var roll;

// current frame, ranges for 0-9 (representing 1-10)
private var frame:int;

// bowling score

class FrameScore {
	var ball1:int;
	var ball2:int;
	var ball3:int;
	var total:int;
}

static var currentplayer:int;

static var numplayers:int = 1;

class Player {
	var name:String;
	var scores:FrameScore[];
}

static var players:Player[];

function InitPlayers() {
	players = new Player[numplayers];
	for (var p:int = 0; p<numplayers; ++p) {
		players[p]=new Player();
		players[p].scores = new FrameScore[10];
		for (var i:int=0; i<players[p].scores.length; ++i) {
			players[p].scores[i] = new FrameScore();
		}
		players[p].name =  "Player"+(p+1)+"Name";
	}
}


function ClearScore(p:int) {
	for (var i:int=0; i<players[p].scores.length; ++i) {
		var score:FrameScore = players[p].scores[i];
		score.ball1=-1;
		score.ball2=-1;
		score.ball3=-1;
		score.total=-1;
	}
}

// set the ball 1 score of the current frame
function SetBall1Score() {
	var scores:FrameScore[] = GetCurrentPlayer().scores;
	var score:int = GetPinsDown();
	var framescore:FrameScore = scores[frame];
	framescore.ball1=score;
	// if previous frame was a spare, set its score
	if (frame>0 && IsSpare(frame-1)) {
		SetSpareScore(frame-1);
	}
	// if the previous two frames were strikes, then set the score of the first frame
	if (frame>1 && IsStrike(frame-1) && IsStrike(frame-2)) {
		SetStrikeScore(frame-2);
	}
}

// set the ball 2 score of the current frame
function SetBall2Score() {
	var scores:FrameScore[] = GetCurrentPlayer().scores;
	var score:int = GetPinsDown();
	var framescore:FrameScore = scores[frame];
	if (IsStrike(frame)) {
		framescore.ball2=score; // final frame
	} else {
		framescore.ball2=score-framescore.ball1;
	}
	// calculate this frame's score if it isn't a spare or strike
	if (!IsSpare(frame) && !IsStrike(frame)) {
		framescore.total= score;
	}
	// if previous frame was a strike then set its score
	if (frame>0 && IsStrike(frame-1)) {
		SetStrikeScore(frame-1);
	}
}

// set the ball 3 score of the current frame (final frame)
function SetBall3Score() {
	var scores:FrameScore[] = GetCurrentPlayer().scores;
	var score:int = GetPinsDown();
	var framescore:FrameScore = scores[frame];
	if (IsStrike(frame) && framescore.ball2 <10) {
		framescore.ball3 = score - framescore.ball2;
	} else {
		framescore.ball3 = score; // spare or two strikes
	}
	framescore.total = framescore.ball1+framescore.ball2+framescore.ball3;
}

// calculate and set the strike score for the given frame
// not called for the final frame
function SetStrikeScore(f:int) {
	var scores:FrameScore[] = GetCurrentPlayer().scores;
	var framescore:FrameScore = scores[f];
	framescore.total = framescore.ball1;
	// always add the score from first roll of the next frame
	framescore.total+=scores[f+1].ball1;
	// if not the ninth or tenth frame, and the next frame is a strike, then add the the ball1 score from the frame after that
	if (f < 8 && IsStrike(f+1)) {
		framescore.total+=scores[f+2].ball1;
	} else {
		// for the ninth frame (frame 8) add the second ball from the next (final) frame (there always is one)
		framescore.total+=scores[f+1].ball2;
	}
}

// calculate and set the spare score for the given frame
function SetSpareScore(f:int) {
	var scores:FrameScore[] = GetCurrentPlayer().scores;
	var framescore:FrameScore = scores[f];
	// the score is the score of both rolls in this frame and teh first roll of the next
	framescore.total = framescore.ball1+framescore.ball2+scores[f+1].ball1;
}


static function GetCurrentPlayer():Player {
	return players[currentplayer];
}

static function GetPlayerName(p:int) {
	return players[p].name;
}

static function GetCurrentScores():FrameScore[] {
	return GetCurrentScores(currentplayer);
}

static function GetCurrentScores(p:int):FrameScore[] {
	return players[p].scores;
}

static function GetSinglePlayerScore():int {
	return GetScore(9,0);
}

static function GetScore(f:int):int {
	return GetScore(f,currentplayer);
}

// return score for given player and frame, -1 if score has not finalized
static function GetScore(f:int, playerindex:int):int {
	var scores:FrameScore[] = GetCurrentScores(playerindex);
	if (f==0 || scores[f].total==-1) {
		return scores[f].total;
	} else {
		var prev:int = GetScore(f-1,playerindex);
		if (prev==-1) {
			return -1;
		} else {
			return scores[f].total+prev;
		}
	}
}

static function IsStrike(f:int):boolean {
	return IsStrike(f,currentplayer);
}

static function IsStrike(f:int, playerindex:int):boolean {
	var scores:FrameScore[] = GetCurrentScores(playerindex);
	var framescore:FrameScore = scores[f];
	return framescore.ball1 == 10;
}

static function IsSpare(f:int):boolean {
	return IsSpare(f,currentplayer);
}

static function IsSpare(f:int, playerindex:int):boolean {
	var scores:FrameScore[] = GetCurrentScores(playerindex);
	var framescore:FrameScore = scores[f];
	// doesn't handle spare on ball3
	return !IsStrike(f,playerindex) &&
		(framescore.ball1 + framescore.ball2 == 10);
}

// reset game objects
function ResetBall() {
	var ballscript:BowlingBall = ball.GetComponent(BowlingBall);
	ballscript.ResetPosition();
}

function ResetEverything() {
	BroadcastMessage("ResetPosition");
}	

function GetPinsDown() {
	return rack.knockedOver;
}

// state machine

private var state:String;

function Start() {
	InitPlayers();
	state="NewGame";
	while (true) {
//		Debug.Log("State "+state);
		yield StartCoroutine(state);
	}
}

function NewGame() {
	for (var p:int=0; p<numplayers; ++p) {
		ClearScore(p);
	}
	frame = 0;
	currentplayer=0;
	state="Ball1";
}

function Ball1() {
	ResetEverything();
	roll = 1;
	state="Rolling";
	// need to check for end of game, i.e. frame 10
}

function Ball2() {
	if (GetPinsDown()==10) {
		ResetEverything();
	} else {
		ResetBall();
	}
	roll = 2;
	state="Rolling";
}

function Ball3() {
	if (GetPinsDown()==10) {
		ResetEverything();
	} else {
		ResetBall();
	}
	roll = 3;
	state="Rolling";
}

function Rolling() {
	var ballscript:BowlingBall = ball.GetComponent(BowlingBall);
	// modify this roll-finished test according to your game
	while (!ballscript.IsSunk() && GetPinsDown()!=10) {
		yield;
	}
	state = "RollOver";
}

function RollOver() {
	switch (roll) {
		case 1:	SetBall1Score(); break;
		case 2: SetBall2Score(); break;
		case 3: SetBall3Score(); break;
	}
	if (roll == 1 && IsStrike(frame)) {
		state = "Strike";
		return;
	}
	if (roll == 2 && IsSpare(frame)) {
		state = "Spare";
		return;
	}
	state = "KnockedSomeDown";
}

function Spare() {
	BroadcastMessage("Score","Spare!");
	var audio:AudioSource = GetComponent(AudioSource);
	audio.clip=cheer;
	audio.Play();
	state="NextBall";
}

function Strike() {
	BroadcastMessage("Score","Strike!");
	var audio:AudioSource = GetComponent(AudioSource);
	audio.clip=cheer;
	audio.Play();
	state = "NextBall";
}

function KnockedSomeDown() {
	BroadcastMessage("Score",""+GetPinsDown()+" pins!");
	var audio:AudioSource = GetComponent(AudioSource);
	audio.clip=OK;
	audio.Play();
	state="NextBall";
}

function NextBall() {
	if (frame == 9) {
		switch (roll) {
			case 1:
			 state = "Ball2";
			 break;
			case 2:
				if (IsSpare(frame) || IsStrike(frame)) {
					state = "Ball3";
				} else {
					if (++currentplayer < numplayers) {
					//	UpdateName();
						state = "Ball1";
					} else {
						state = "GameOver";
					}
				}
				break;
			case 3:
				if (++currentplayer < numplayers) {
						//UpdateName();
						state = "Ball1";
					} else {
						state = "GameOver";
					}
				return;
		}
	} else if (roll == 1 && !IsStrike(frame)) {
				state = "Ball2";
			} else {
				if (++currentplayer < numplayers) {
				} else {
					++frame;
					currentplayer = 0;
				}
				//UpdateName();
				state = "Ball1";
			}
	}

function GameOver() {
	BroadcastMessage("Score","Game Over");
	state="NewGame";
}
