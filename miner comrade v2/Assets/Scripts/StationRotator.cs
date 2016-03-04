using UnityEngine;
using System.Collections;

public class StationRotator : MonoBehaviour {

    public float speedOfRotation;

	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * speedOfRotation, Time.deltaTime);
    }
}
