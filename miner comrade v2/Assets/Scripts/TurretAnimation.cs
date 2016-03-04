using UnityEngine;
using System.Collections;

public class TurretAnimation : MonoBehaviour {



	// Update is called once per frame
	void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);



    }
}
