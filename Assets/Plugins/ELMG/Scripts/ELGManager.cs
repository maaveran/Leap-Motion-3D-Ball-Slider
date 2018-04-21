using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ELGManager : MonoBehaviour {
	
	// DELEGATE
	public delegate void onGestureRecognised(EasyLeapGesture gesture);
	public static event onGestureRecognised GestureRecognised;
	
	
	private Leap.Controller leapController;
	private Leap.Frame mFrame;
	private Dictionary<int,EasyLeapGesture> gestureList = new Dictionary<int, EasyLeapGesture>();
	
	public static bool circleGestureRegistered = true;
	public static  bool swipeGestureRegistered = true;
	public static  bool keytapGestureRegistered = true;
	public static  bool screentapGestureRegistered = true;
	public static  bool numbersGestureRegistered = true;
	public static bool closeFistRegistered = true;
	public static bool openFistRegistered = true;
	public static bool pushGestureRegistered = true;
	public static bool pullGestureRegistered = true;
	public static bool doubleInwardsSwipeGestureRegistered = true;
	public static bool doubleOutwardsSwipeGestureRegistered = true;
	public static bool clapGestureRegistered = true;
	public static bool twoFingerKeytapRegistered = true;
	public static bool threeFingerKeytapRegistered = true;
	public static bool twoFingerScreentapRegistered = true;
	public static bool threeFingerScreentapRegistered = true;
	public static bool steeringWheelRegistered = true;

	private float pushRestTime = 0f;
	private float pullRestTime = 0f;
	private float doubleInSwipeRestTime = 0f;
	private float doubleOutSwipeRestTime = 0f;
	private float clapRestTime = 0f;
	
	// Use this for initialization
	void Start () {
		leapController = new Leap.Controller();
		leapController.EnableGesture(Leap.Gesture.GestureType.TYPECIRCLE,true);
		leapController.EnableGesture(Leap.Gesture.GestureType.TYPESWIPE,true);
		leapController.EnableGesture(Leap.Gesture.GestureType.TYPEKEYTAP,true);
		leapController.EnableGesture(Leap.Gesture.GestureType.TYPESCREENTAP,true);
		
	}
	
	// Update is called once per frame
	void Update () {
		mFrame = leapController.Frame ();
		int fingerCount = 0;
		if(numbersGestureRegistered || closeFistRegistered || openFistRegistered ||
				keytapGestureRegistered || twoFingerKeytapRegistered || threeFingerKeytapRegistered || 
				screentapGestureRegistered || twoFingerScreentapRegistered || threeFingerScreentapRegistered || steeringWheelRegistered) 
			fingerCount = GetFingerCount();
		
		foreach(Leap.Gesture gesture in mFrame.Gestures ()) {
			switch(gesture.Type) {
				case Leap.Gesture.GestureType.TYPECIRCLE:
					if(circleGestureRegistered) BuiltInGestureRecognised(gesture,EasyLeapGestureType.TYPECIRCLE);
					break;
				case Leap.Gesture.GestureType.TYPESWIPE:
					if(swipeGestureRegistered) BuiltInGestureRecognised(gesture,EasyLeapGestureType.TYPESWIPE);
					break;
				case Leap.Gesture.GestureType.TYPEKEYTAP:
					if(keytapGestureRegistered && fingerCount == 1) BuiltInGestureRecognised(gesture,EasyLeapGestureType.TYPEKEYTAP);
					if(twoFingerKeytapRegistered && fingerCount == 2) BuiltInImprovedGestureRecognised(gesture,EasyLeapGestureType.TWO_FINGERS_KEYTAP);
					if(threeFingerKeytapRegistered && fingerCount == 3) BuiltInImprovedGestureRecognised(gesture,EasyLeapGestureType.THREE_FINGERS_KEYTAP);
					break;
				case Leap.Gesture.GestureType.TYPESCREENTAP:
					if(screentapGestureRegistered && fingerCount == 1) BuiltInGestureRecognised(gesture,EasyLeapGestureType.TYPESCREENTAP);
					if(twoFingerScreentapRegistered && fingerCount == 2) BuiltInImprovedGestureRecognised(gesture,EasyLeapGestureType.TWO_FINGERS_SCREENTAP);
					if(threeFingerScreentapRegistered && fingerCount == 3) BuiltInImprovedGestureRecognised(gesture,EasyLeapGestureType.THREE_FINGERS_SCREENTAP);
					break;
			}
			
		}
		if(mFrame.Gestures ().Count == 0) ClearDraggingGestures();
		
		if(numbersGestureRegistered || closeFistRegistered || openFistRegistered) {
			switch(fingerCount) {
				case 0:
					// NO FINGERS
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.DEFAULT);
					if(closeFistRegistered) {
						if(mFrame.Hands.Count == 1) {
							if(PalmIsHorizontal(mFrame.Hands[0])) CloseFistGestureRecognised(EasyLeapGestureState.STATESTOP);
						}
						else CloseFistGestureRecognised(EasyLeapGestureState.STATEINVALID);
					}
					if(openFistRegistered) {
						if(mFrame.Hands.Count == 1) {
							if(PalmIsHorizontal(mFrame.Hands[0])) OpenFistGestureRecognised(EasyLeapGestureState.STATESTART);
						}
						else OpenFistGestureRecognised(EasyLeapGestureState.STATEINVALID);
					}
					break;
				case 1:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.ONE);
					break;
				case 2:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.TWO);
					break;
				case 3:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.THREE);
					break;
				case 4:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.FOUR);
					break;
				case 5:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.FIVE);
					if(closeFistRegistered && mFrame.Hands.Count == 1) CloseFistGestureRecognised(EasyLeapGestureState.STATESTART);
					if(openFistRegistered) OpenFistGestureRecognised(EasyLeapGestureState.STATESTOP);
					break;
				case 6:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.SIX);
					break;
				case 7:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.SEVEN);
					break;
				case 8:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.EIGHT);
					break;
				case 9:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.NINE);
					break;
				case 10:
					if(numbersGestureRegistered) NumbersGestureRecognised(EasyLeapGestureType.TEN);
					break;
			}
		}
		
		if(pushGestureRegistered || pullGestureRegistered) {
			if(mFrame.Hands.Count == 1) {
				if(pushGestureRegistered) {
					if(mFrame.Hands[0].PalmVelocity.y < -EasyLeapGesture.MinPushPullVelocity) PushGestureRecognised(EasyLeapGestureState.STATESTART);
					else PushGestureRecognised(EasyLeapGestureState.STATEINVALID);
				}
				if(pullGestureRegistered) {
					if(mFrame.Hands[0].PalmVelocity.y > EasyLeapGesture.MinPushPullVelocity) PullGestureRecognised(EasyLeapGestureState.STATESTART);
					else PullGestureRecognised(EasyLeapGestureState.STATEINVALID);
				}
			} else {
				PushGestureRecognised(EasyLeapGestureState.STATEINVALID);
				PullGestureRecognised(EasyLeapGestureState.STATEINVALID);
			}
		}
		
		if(doubleInwardsSwipeGestureRegistered || doubleOutwardsSwipeGestureRegistered) {
			if(mFrame.Hands.Count == 2) {
				bool leftHandSwipeIn = (PalmIsHorizontal(mFrame.Hands.Leftmost)) && mFrame.Hands.Leftmost.PalmVelocity.x > EasyLeapGesture.MinSwipeVelocity;
				bool rightHandSwipeIn = (PalmIsHorizontal(mFrame.Hands.Rightmost)) && mFrame.Hands.Rightmost.PalmVelocity.x < -EasyLeapGesture.MinSwipeVelocity;
				if(doubleInwardsSwipeGestureRegistered && (leftHandSwipeIn && rightHandSwipeIn)) {
					if(mFrame.Hands[0].StabilizedPalmPosition.DistanceTo(mFrame.Hands[1].StabilizedPalmPosition) < EasyLeapGesture.MaxPalmDistance) DoubleInwardsSwipeRecognised(EasyLeapGestureState.STATESTART);
				} else DoubleInwardsSwipeRecognised(EasyLeapGestureState.STATEINVALID);
				bool leftHandSwipeOut = (PalmIsHorizontal(mFrame.Hands.Leftmost)) && mFrame.Hands.Leftmost.PalmVelocity.x < -EasyLeapGesture.MinSwipeVelocity;
				bool rightHandSwipeOut = (PalmIsHorizontal(mFrame.Hands.Rightmost)) && mFrame.Hands.Rightmost.PalmVelocity.x > EasyLeapGesture.MinSwipeVelocity;
				if(doubleOutwardsSwipeGestureRegistered && leftHandSwipeOut && rightHandSwipeOut) {
					if(mFrame.Hands[0].StabilizedPalmPosition.DistanceTo(mFrame.Hands[1].StabilizedPalmPosition) > EasyLeapGesture.MaxPalmDistance) DoubleOutwardsSwipeRecognised(EasyLeapGestureState.STATESTART);
				} else DoubleOutwardsSwipeRecognised(EasyLeapGestureState.STATEINVALID);
			}
		}
		
		if(clapGestureRegistered) {
			if(mFrame.Hands.Count == 2) {
				bool leftHandSwipeIn = (!PalmIsHorizontal(mFrame.Hands.Leftmost)) && mFrame.Hands.Leftmost.PalmVelocity.x > EasyLeapGesture.MinClapVelocity;
				bool rightHandSwipeIn = (!PalmIsHorizontal(mFrame.Hands.Rightmost)) && mFrame.Hands.Rightmost.PalmVelocity.x < -EasyLeapGesture.MinClapVelocity;
				if(leftHandSwipeIn && rightHandSwipeIn) {
					if(mFrame.Hands[0].StabilizedPalmPosition.DistanceTo(mFrame.Hands[1].StabilizedPalmPosition) < EasyLeapGesture.MaxPalmClapDistance) ClapRecognised(EasyLeapGestureState.STATESTART);
				} else ClapRecognised(EasyLeapGestureState.STATEINVALID);
			}
		}
		
		if(steeringWheelRegistered) {
			float palmsAngle = (mFrame.Hands.Leftmost.PalmNormal.AngleTo(mFrame.Hands.Rightmost.PalmNormal)*Mathf.Rad2Deg);
			if(mFrame.Hands.Count >= 1 && fingerCount < 2 && (palmsAngle > 110 || palmsAngle < 70)) {
				Leap.Vector leftMost = mFrame.Hands.Leftmost.StabilizedPalmPosition;
				Leap.Vector rightMost = mFrame.Hands.Rightmost.StabilizedPalmPosition;
				Leap.Vector steerVector = (leftMost - rightMost).Normalized;
				//steerVector.z = 0;
				float angle = steerVector.AngleTo(Leap.Vector.Left)*Mathf.Rad2Deg * (leftMost.y > rightMost.y ? 1 : -1);
				SteeringWheelRecognised(angle, Mathf.Abs (leftMost.z) - Mathf.Abs (rightMost.z));
			}
		}
		
		// Send gestures detected to all registered gesture listeners
		SendGesturesToListeners();
	}
	
	// BUGGY: clear any gestures that have not been removed properly from the list -- to improve
	private void ClearDraggingGestures() {
		if(gestureList.Count == 0) return;
		var keys = new List<int>(gestureList.Keys);
		foreach(int cKey in keys) {
			if(gestureList[cKey].Type == EasyLeapGestureType.TYPECIRCLE || gestureList[cKey].Type == EasyLeapGestureType.TYPESWIPE) {
				EasyLeapGesture g =  gestureList[cKey];
				g.State = EasyLeapGestureState.STATESTOP;
				gestureList[cKey] = g;
			}
		}
	}
	// Store the recognised gesture on the gesture List
	private void RecordNewGesture(int id, EasyLeapGestureState startState, EasyLeapGestureState updateState, EasyLeapGestureType type, long duration, Leap.Vector position) {
		if(gestureList.ContainsKey(id)) {
			EasyLeapGesture g = gestureList[id];
			g.State = updateState;
			g.Duration = duration < 0 ? (long)(1000000*Time.deltaTime)+g.Duration : duration;
			gestureList[id] = g;
		} else {
			EasyLeapGesture gest = new EasyLeapGesture();
			gest.Duration = 0;
			gest.Id = id;
			gest.State = startState;
			gest.Type = type;
			gest.Frame = mFrame;
			gest.Position = position;
			gestureList.Add(id,gest);
		}
	}
	// Individual gesture start-stop handler
	private void SteeringWheelRecognised(float angle, float zDepth) {
		RecordNewGesture(-(int)EasyLeapGestureType.STEERING_WHEEL,EasyLeapGestureState.STATESTOP,EasyLeapGestureState.STATESTOP,EasyLeapGestureType.STEERING_WHEEL,0,new Leap.Vector(angle,angle,zDepth));
	}
	private void ClapRecognised(EasyLeapGestureState state) {
		if(state == EasyLeapGestureState.STATEINVALID || Time.time < clapRestTime + EasyLeapGesture.ClapRecoveryTime) {
			gestureList.Remove(-(int)EasyLeapGestureType.CLAP);
			return;
		}
		clapRestTime = Time.time;
		RecordNewGesture(-(int)EasyLeapGestureType.CLAP,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,EasyLeapGestureType.CLAP,-1,new Leap.Vector(mFrame.Hands[0].StabilizedPalmPosition.x + mFrame.Hands[1].StabilizedPalmPosition.x,mFrame.Hands[0].StabilizedPalmPosition.y,mFrame.Hands[0].StabilizedPalmPosition.z));
		
	}
	private void DoubleInwardsSwipeRecognised(EasyLeapGestureState state) {
		if(state == EasyLeapGestureState.STATEINVALID || Time.time < doubleInSwipeRestTime + EasyLeapGesture.DoubleInwardsRecoveryTime) {
			gestureList.Remove(-(int)EasyLeapGestureType.DOUBLE_SWIPE_IN);
			return;
		}
		doubleInSwipeRestTime = Time.time;
		RecordNewGesture(-(int)EasyLeapGestureType.DOUBLE_SWIPE_IN,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,EasyLeapGestureType.DOUBLE_SWIPE_IN,-1,new Leap.Vector(mFrame.Hands[0].StabilizedPalmPosition.x + mFrame.Hands[1].StabilizedPalmPosition.x,mFrame.Hands[0].StabilizedPalmPosition.y,mFrame.Hands[0].StabilizedPalmPosition.z));
		
	}
	private void DoubleOutwardsSwipeRecognised(EasyLeapGestureState state) {
		if(state == EasyLeapGestureState.STATEINVALID || Time.time < doubleOutSwipeRestTime + EasyLeapGesture.DoubleOutwardsRecoveryTime) {
			gestureList.Remove(-(int)EasyLeapGestureType.DOUBLE_SWIPE_OUT);
			return;
		}
		doubleOutSwipeRestTime = Time.time;
		RecordNewGesture(-(int)EasyLeapGestureType.DOUBLE_SWIPE_OUT,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,EasyLeapGestureType.DOUBLE_SWIPE_OUT,-1,new Leap.Vector(mFrame.Hands[0].StabilizedPalmPosition.x + mFrame.Hands[1].StabilizedPalmPosition.x,mFrame.Hands[0].StabilizedPalmPosition.y,mFrame.Hands[0].StabilizedPalmPosition.z));
		
	}
	private void PushGestureRecognised(EasyLeapGestureState state) {
		if(state == EasyLeapGestureState.STATEINVALID || Time.time < pushRestTime + EasyLeapGesture.PushRecoveryTime) {
			gestureList.Remove(-(int)EasyLeapGestureType.PUSH);
			return;
		}
		pushRestTime = Time.time;
		RecordNewGesture(-(int)EasyLeapGestureType.PUSH,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,EasyLeapGestureType.PUSH,-1,mFrame.Hands[0].StabilizedPalmPosition);
		
	}
	private void PullGestureRecognised(EasyLeapGestureState state) {
		if(state == EasyLeapGestureState.STATEINVALID || Time.time < pullRestTime + EasyLeapGesture.PullRecoveryTime) {
			gestureList.Remove(-(int)EasyLeapGestureType.PULL);
			return;
		}
		pullRestTime = Time.time;
		RecordNewGesture(-(int)EasyLeapGestureType.PULL,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,EasyLeapGestureType.PULL,-1,mFrame.Hands[0].StabilizedPalmPosition);
		
	}
	private void CloseFistGestureRecognised(EasyLeapGestureState state) {
		if(EasyLeapGestureState.STATEINVALID == state) {
			gestureList.Remove (-(int)EasyLeapGestureType.CLOSE_FIST);
			return;
		}
		if(state == EasyLeapGestureState.STATESTOP && !gestureList.ContainsKey(-(int)EasyLeapGestureType.CLOSE_FIST)) return;
		RecordNewGesture(-(int)EasyLeapGestureType.CLOSE_FIST,
			EasyLeapGestureState.STATESTART, 
			state == EasyLeapGestureState.STATESTART ? EasyLeapGestureState.STATEUPDATE : EasyLeapGestureState.STATESTOP, 
			EasyLeapGestureType.CLOSE_FIST,
			-1,
			mFrame.Hands[0].StabilizedPalmPosition);
	}
	private void OpenFistGestureRecognised(EasyLeapGestureState state) {
		if(EasyLeapGestureState.STATEINVALID == state) {
			gestureList.Remove (-(int)EasyLeapGestureType.OPEN_FIST);
			return;
		}
		if(state == EasyLeapGestureState.STATESTOP && !gestureList.ContainsKey(-(int)EasyLeapGestureType.OPEN_FIST)) return;
		RecordNewGesture(-(int)EasyLeapGestureType.OPEN_FIST,
			EasyLeapGestureState.STATESTART, 
			state == EasyLeapGestureState.STATESTART ? EasyLeapGestureState.STATEUPDATE : EasyLeapGestureState.STATESTOP, 
			EasyLeapGestureType.OPEN_FIST,
			-1,
			mFrame.Hands[0].StabilizedPalmPosition);
	}
	private void BuiltInImprovedGestureRecognised(Leap.Gesture gesture, EasyLeapGestureType type) {
		if(!gestureList.ContainsKey(-(int)type)) {
			RecordNewGesture(-(int)type,
				EasyLeapGestureState.STATESTOP, 
				EasyLeapGestureState.STATEUPDATE, 
				type,
				-1,
				gesture.Hands[0].StabilizedPalmPosition);
		}
	}
	private void BuiltInGestureRecognised(Leap.Gesture gesture, EasyLeapGestureType type) {
		RecordNewGesture(gesture.Id,ConvertGestureState(gesture.State),ConvertGestureState(gesture.State),type,gesture.Duration,gesture.Hands[0].StabilizedPalmPosition);
	}
	private void NumbersGestureRecognised(EasyLeapGestureType type) {
		if(type != EasyLeapGestureType.DEFAULT) {
			RecordNewGesture(-(int)type,EasyLeapGestureState.STATESTART,EasyLeapGestureState.STATEUPDATE,type,-1,mFrame.Hands[0].StabilizedPalmPosition);
		}
		if(gestureList.Count == 0) return;
		for(int ii =(int)EasyLeapGestureType.ONE; ii<=(int)EasyLeapGestureType.TEN; ii++) {
			if(type != (EasyLeapGestureType)(ii) && gestureList.ContainsKey(-ii)) {
				EasyLeapGesture g = gestureList[-ii];
				g.State = EasyLeapGestureState.STATESTOP;
				gestureList[-(int)ii] = g;
			}
		}
	}
	
	
	// Send Gestures to all registered listeners with gestures recognised on current frame
	private void SendGesturesToListeners() {
		Dictionary<int,EasyLeapGesture> copy = new Dictionary<int, EasyLeapGesture> (gestureList);
		foreach(KeyValuePair<int,EasyLeapGesture> obj in copy) {
			GestureRecognised(obj.Value);
			if(obj.Value.State == EasyLeapGestureState.STATESTOP) gestureList.Remove (obj.Key);
		}
	}
	
	// Auxiliary functions //
	private int GetFingerCount() {
		int count = 0;
		foreach(Leap.Finger finger in mFrame.Fingers) {
			if(Mathf.Rad2Deg*finger.Direction.AngleTo(finger.Hand.Direction) < EasyLeapGesture.MaxAngleFinger
				&& finger.Length > EasyLeapGesture.MinDistanceFinger) count++;
		}
		return count;
	}
	private EasyLeapGestureState ConvertGestureState(Leap.Gesture.GestureState state) {
		switch(state) {
			case Leap.Gesture.GestureState.STATESTART:
				return EasyLeapGestureState.STATESTART;
			case Leap.Gesture.GestureState.STATEUPDATE:
				return EasyLeapGestureState.STATEUPDATE;
			case Leap.Gesture.GestureState.STATESTOP:
				return EasyLeapGestureState.STATESTOP;
		}
		return EasyLeapGestureState.STATEINVALID;
	}
	
	private bool PalmIsHorizontal(Leap.Hand hand) {
		return hand.PalmNormal.AngleTo(Leap.Vector.Down)*Mathf.Rad2Deg < EasyLeapGesture.MaxAnglePalm && 
			Mathf.Abs (hand.StabilizedPalmPosition.x) < EasyLeapGesture.MaxFieldPalm &&
			Mathf.Abs (hand.StabilizedPalmPosition.z) < EasyLeapGesture.MaxFieldPalm;
	}
}


// Structs and enums //
public struct EasyLeapGesture {
	public EasyLeapGestureType Type;
	public EasyLeapGestureState State;
	public long Duration;
	public Leap.Frame Frame;
	public Leap.Vector Position;
	public int Id;
	// configurable settings
	public static float MaxAngleFinger = 45f; // max angle to consider a pointable a finger
	public static float MinDistanceFinger = 25f; // min distance to consider a pointable a finger (away from hand)
	public static float MaxAnglePalm = 25f; // max angle to consider a hand horizontal
	public static float MaxFieldPalm = 180f; // max x and z to read palm pos
	public static float PullRecoveryTime = 0.2f; // min time in between pull gestures
	public static float PushRecoveryTime = 0.2f; // min time in between push gestures
	public static float MinPushPullVelocity = 350f; // min velocity to recognise push pull gestures
	public static float DoubleInwardsRecoveryTime = 0.2f; // min time in between double inwards swipe gestures
	public static float DoubleOutwardsRecoveryTime = 0.2f; // min time in between double outwards swipe gestures
	public static float MinSwipeVelocity = 200f; // min velocity to recognise double swipe gestures
	public static float MaxPalmDistance = 120f; // max distance between palms to consider together -double swipe
	public static float MinClapVelocity = 350f; // min velocity to recognise a clap gesture
	public static float MaxPalmClapDistance = 90f; // max distance between palms to consider together -clap
	public static float ClapRecoveryTime = 0.15f; // min time in between claps
}

public enum EasyLeapGestureType {
	DEFAULT,
	TYPECIRCLE,
	TYPESWIPE,
	TYPEKEYTAP,
	TWO_FINGERS_KEYTAP,
	THREE_FINGERS_KEYTAP,
	TYPESCREENTAP,
	TWO_FINGERS_SCREENTAP,
	THREE_FINGERS_SCREENTAP,
	ONE,
	TWO,
	THREE,
	FOUR,
	FIVE,
	SIX,
	SEVEN,
	EIGHT,
	NINE,
	TEN,
	CLOSE_FIST,
	OPEN_FIST,
	PUSH,
	PULL,
	DOUBLE_SWIPE_IN,
	DOUBLE_SWIPE_OUT,
	CLAP,
	STEERING_WHEEL,
	NUM_OF_ITEMS
}

public enum EasyLeapGestureState {
	STATEINVALID,
	STATESTART,
	STATEUPDATE,
	STATESTOP
}
