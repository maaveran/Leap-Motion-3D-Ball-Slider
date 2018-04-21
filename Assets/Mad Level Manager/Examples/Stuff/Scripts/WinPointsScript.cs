/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class WinPointsScript : MonoBehaviour {

    public int points = 100;

    void OnMouseDown() {
        Debug.Log("Level won", this);
        MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);

        MadLevelProfile.SetLevelInteger(MadLevel.currentLevelName, "score", points);

        MadLevel.LoadLevelByName("Level Select");
    }

    void OnGUI() {
        var sp = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(sp.x - 50, sp.y + 100, 100, 50), "Win " + points);
    }
}

#if !UNITY_3_5
} // namespace
#endif