//-----------------------------------------
//   Jason Walters
//   http://glitchbeam.com
//   @jasonrwalters
//
//   last edit on 7/15/2015
//-----------------------------------------

using UnityEngine;
using System.Collections;

public class TargetFire : MonoBehaviour 
{
    public bool debugOn = false;
    public GameObject bullet;

    private Transform tr;
    private float rotY = 0.0f;
    private bool targetHit = true;

	// Use this for initialization
	void Start() 
    {
        // reference components
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update() 
    {
        // constantly revolve the object on y axis
        /*
		rotY++;
        if (rotY > 360.0f) rotY = 0.0f;
        tr.eulerAngles = new Vector3(0.0f, rotY, 0.0f);
		*/
        RaycastHit hit;
        // Create a ray from the transform position along the transform's z-axis
        Ray ray = new Ray(tr.position, tr.forward);
        if (Physics.Raycast(ray, out hit))
        {
            // if target is detected...
            if (hit.collider.name == "Target" && !targetHit)
            {
                // spawn bullet prefab
                Instantiate(bullet, tr.position, tr.rotation);

                Debug.Log("Target Has Been Detected!");
                targetHit = true;
            }
        }
        else if (targetHit)
        {
            Debug.Log("Nothing Is Detected.");
            targetHit = false;
        }
            

        // draws debug ray
        if (debugOn) DebugDraw();

	}

    void DebugDraw()
    {
        Vector3 forward = tr.TransformDirection(Vector3.right) * 10;
        Debug.DrawRay(tr.position, forward, Color.green);
    }
}
