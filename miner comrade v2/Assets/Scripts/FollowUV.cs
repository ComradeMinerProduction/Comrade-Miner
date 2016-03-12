using UnityEngine;
using System.Collections;

public class FollowUV : MonoBehaviour {

    public float parralax;

    // Translate the ship that the field needs to move with onto the target slot

    public Transform ship;





    // Use this for initialization
    void Start () {
        Debug.Log(gameObject.name + " is on a script that needs optimized");
	}
	
	// this update function has been replaced by the 3D skybox feature
    // also if there is still interest in using this script,
    // please switch the mr to be called in start and optimize the use 
    // of update to a coroutine
	void Update () {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        //grabs first material in mesh renderers array
        Material mat = mr.material;
        Vector2 offset = mat.mainTextureOffset;
        offset.y = (ship.position.y / ship.localScale.y) / parralax;
        offset.x = (ship.position.x / ship.localScale.x) / parralax;

        mat.mainTextureOffset = offset;
        transform.position = new Vector3(ship.position.x, ship.position.y, 1);

    }
}
