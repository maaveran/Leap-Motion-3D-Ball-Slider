/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadDragStopDraggable : MadDraggable {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public DragStopCallback dragStopCallback;
    List<Vector2> dragStops = new List<Vector2>();
    
    int forcedDragStopIndex = -1;

    public Direction direction = Direction.Horizontal;
    public bool directionInvert = false;
    private float avarageDistance;

    // ===========================================================
    // Properties
    // ===========================================================

    public int dragStopCount {
        get {
            return dragStops.Count;
        }
    }

    public int dragStopCurrentIndex { get; private set; }

    public override Vector2 progress {
        get {
            if (dragStopCount == 0) {
                return Vector2.zero;
            } else if (dragStopCount == 1) {
                return new Vector2(dragStopCurrentIndex, 0);
            } else {
                return new Vector2(dragStopCurrentIndex / (float) (dragStopCount - 1) , 0);
            }
        }
    }

    public bool animating { get; private set; }

    // ===========================================================
    // Methods
    // ===========================================================

    protected override void Update() {
        if (!Application.isPlaying) {
            return;
        }

        base.Update();
        
        if (dragStops.Count == 0) {
            return;
        }
        
        if (!IsTouchingSingle()) {
            if (dragging) {
                int newIndex = IntendedDragStopIndex();
                if (newIndex != dragStopCurrentIndex) {
                    dragStopCurrentIndex = newIndex;
                    dragStopCallback(newIndex);
                }

                dragging = false;
            }

            if (forcedDragStopIndex != -1) {
                dragStopCurrentIndex = forcedDragStopIndex;
                dragStopCallback(dragStopCurrentIndex);
                forcedDragStopIndex = -1;
            }

            
            ReturnToDragStop();
            cameraPos = cachedCamPos;

            Clear();
        } else {
            forcedDragStopIndex = -1;
            int intended = IntendedDragStopIndex();
            Vector2 lastCamPos = cachedCamPos;
            var touchPos = TouchPosition();
            
            if (IsTouchingJustStarted()) {
                lastPosition = touchPos;
            } else {
                cachedCamPos -= touchPos - lastPosition;
                lastPosition = touchPos;
            }
            
            int closestNeighbor = ClosestNeighborTo(intended);
            
            if (closestNeighbor != -1) {
                // dot product to AB vector to stay on the line
                Vector2 ab = dragStops[closestNeighbor] - dragStops[intended];
                Vector2 ac = cachedCamPos - dragStops[intended];
                
                float projection = Vector2.Dot(ac, ab.normalized);
                Vector2 position = ab.normalized * projection;
                cachedCamPos = dragStops[intended] + position;
            } else {
                // out of drag area, return NOW!
                cachedCamPos = dragStops[intended];
            }
            
            RegisterDelta(cachedCamPos - lastCamPos);
            
            if (dragDistance > deadDistance) {
                dragging = true;
                cameraPos = cachedCamPos;
            }
        }
    }

    public void ClearDragStops() {
        dragStops.Clear();
        dragStopCurrentIndex = 0;
    }

    public int AddDragStop(float x, float y) {
        dragStops.Add(new Vector2(x, y));
        ComputeAvarageDistance();
        return dragStops.Count - 1;
    }

    private void ComputeAvarageDistance() {
        if (dragStops.Count < 2) {
            return;
        }

        float distance = 0;
        for (int i = 1; i < dragStops.Count; i++) {
            distance += Vector2.Distance(dragStops[i - 1], dragStops[i]);
        }

        avarageDistance = distance / (dragStops.Count - 1);
    }
    
    public void MoveTo(int dragStop, bool now) {
        forcedDragStopIndex = dragStop;

        if (!now) {
            // little hack :-(
            lastTouchTime = Time.time;
            lastTouchCameraPos = cameraPos;
        }
    }
    
    void ReturnToDragStop() {
        Vector3 dragStopPos = dragStops[dragStopCurrentIndex];
        float timeDiff = Time.time - lastTouchTime;
        if (timeDiff < moveEasingDuration && cameraPos != (Vector2) dragStopPos) {
            animating = true;
            cachedCamPos = Ease(moveEasingType, lastTouchCameraPos, (Vector2) dragStopPos, timeDiff / moveEasingDuration);
        } else {
            animating = false;
            cachedCamPos = dragStopPos;
        }
    }

    int IntendedDragStopIndex() {
        if (dragStops.Count == 0) {
            return -1;
        }

        if (forcedDragStopIndex != -1) {
            return forcedDragStopIndex;
        }

        int index = dragStopCurrentIndex;

        var force = GetInteriaForce();
        float thredshold = avarageDistance * 2;

        if (force > thredshold) {
            index = Mathf.Clamp(index + IndexChange(force, thredshold), 0, dragStops.Count);
        } else if (force < -thredshold) {
            index = Mathf.Clamp(index - IndexChange(force, thredshold), 0, dragStops.Count);
        } else {
            index = ClosestDragStopIndex();
        }

        index = Mathf.Clamp(index, 0, dragStops.Count - 1);
        return index;
    }

    private int IndexChange(float force, float thredshold) {
        // returns index change by changing the linear value using this rule:
        // linear 1 -> 1
        // linear 2 -> 1 + 2
        // linear 3 -> 1 + 2 + 3

        int linear = (int) Mathf.Abs(force / thredshold);

        int counter = 1;
        int next = 1;
        int power = 1;

        while (linear > counter) {
            counter += ++next;
            power++;
        }

        return power;
    }

    private float GetInteriaForce() {
        switch (direction) {
            case Direction.Horizontal:
                return directionInvert ? -interiaForce.x : interiaForce.x;
            case Direction.Vertical:
                return directionInvert ? interiaForce.y : -interiaForce.y;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    //Vector3 ClosestDragStop() {
    //    var index = ClosestDragStopIndex();
    //    if (index != -1) {
    //        return dragStops[index];
    //    } else {
    //        Debug.LogError("No drag stops defined");
    //        return Vector3.zero;
    //    }
    //}

    int ClosestDragStopIndex(int skip = -1) {
        if (forcedDragStopIndex != -1 && forcedDragStopIndex != skip) {
            return forcedDragStopIndex;
        }

        Vector3 currentPosition = cachedCamPos;
        float closestDistance = float.PositiveInfinity;
        int index = -1;

        for (int i = 0; i < dragStops.Count; ++i) {
            var dragStop = dragStops[i];
            float distance = Vector2.Distance(currentPosition, dragStop);
            if (distance < closestDistance && i != skip) {
                closestDistance = distance;
                index = i;
            }
        }

        // make sure that index is one up or one down
//        if (dragStopCurrentIndex != index) {
//            if (index > dragStopCurrentIndex) {
//                index = dragStopCurrentIndex + 1;
//            } else {
//                index = dragStopCurrentIndex - 1;
//            }
//        }

        return index;
    }
    
    int ClosestNeighborTo(int index) {
        int result = -1;
        float closestDistance = float.PositiveInfinity;
        var currentPoint = dragStops[index];
        
        if (index - 1 >= 0) {
            var dragStopPoint = dragStops[index - 1];
            var distance = Vector2.Distance(currentPoint, dragStopPoint);
            
            closestDistance = distance; // it always will be closer than infinity
            result = index - 1;
        }
        
        if (index + 1 < dragStops.Count) {
            var dragStopPoint = dragStops[index + 1];
            var currentPointToDragStopDistance = Vector2.Distance(currentPoint, dragStopPoint);
            
            if (currentPointToDragStopDistance < closestDistance) {
                result = index + 1;
            }
        }
        
        return result;
    }

    public float GetProgress() {
        if (!animating && !dragging) {
            return dragStopCurrentIndex;
        }

        int first = ClosestDragStopIndex();
        int second = ClosestDragStopIndex(first);

        if (first == -1 || second == -1) {
            return dragStopCurrentIndex;
        }

        int left = first < second ? first : second;
        int right = first < second ? second : first;

        Vector2 leftPos = dragStops[left];
        Vector2 rightPos = dragStops[right];

//        Debug.Log(first + " " + second);
        float f = left + Mathf.Abs((cameraPos - leftPos).magnitude / (rightPos - leftPos).magnitude);
        return f;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public delegate void DragStopCallback(int index);

    public enum Direction {
        Horizontal,
        Vertical
    };
}

#if !UNITY_3_5
} // namespace
#endif