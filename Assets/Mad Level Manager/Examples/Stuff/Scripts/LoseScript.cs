/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class LoseScript : MonoBehaviour {

    void OnMouseDown() {
        Debug.Log("Level lost", this);
        MadLevel.LoadLevelByName("Level Select");
    }

    void OnGUI() {
        var sp = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(sp.x - 50, sp.y + 100, 100, 50), "Lose");
    }
}

#if !UNITY_3_5
} // namespace
#endif