﻿using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	
	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(new Vector3 (0, 20, 0) * Time.deltaTime);
        Debug.Log(gameObject.name + " should be changed to the other rotator script");
	}
}
