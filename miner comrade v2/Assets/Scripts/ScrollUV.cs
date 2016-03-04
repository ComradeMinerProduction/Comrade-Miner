using UnityEngine;
using System.Collections;

public class ScrollUV : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        //grabs first material in mesh renderers array
        Material mat = mr.material;
        Vector2 offset = mat.mainTextureOffset;
        offset.x += Time.deltaTime / 10f;

        mat.mainTextureOffset = offset;
	
	}
}
