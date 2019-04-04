using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Transform cam = Camera.main.transform;

        Vector3 direction = transform.position - cam.position;

        transform.rotation = Quaternion.LookRotation(direction);
	}
}
