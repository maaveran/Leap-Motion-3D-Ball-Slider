/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Runtime.InteropServices;
using UnityEngine;
#if !UNITY_3_5
namespace MadLevelManager {
#endif
using MadLevelManager;

public class Box : MonoBehaviour {

    public int starsCount;
    public bool completeLevel;

    public void Update() {
        if (Input.GetMouseButton(0)) { // mouseDown
            CheckPressed(Input.mousePosition); // run check on current mouse position
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) { // just pressed the screen
            CheckPressed(Input.touches[0].position); // run check on current finger position
        }
    }

    private void CheckPressed(Vector2 screenPosition) {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { // raycast what the player has touched
            if (hit.collider.gameObject == gameObject) { // check if this box has been pressed
                Execute();
            }
        }
    }

    private void Execute() {
        // gain stars
        for (int i = 1; i <= starsCount; i++) { // i = 1, 2, 3...
            string starName = "star_" + i; // this is the star property name
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, starName, true);
        }

        // complete level
        if (completeLevel) {
            MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
        }

        // go back to level select screen
        MadLevel.LoadLevelByName("Level Select"); // name from level configuration
    }
}

#if !UNITY_3_5
} // namespace
#endif