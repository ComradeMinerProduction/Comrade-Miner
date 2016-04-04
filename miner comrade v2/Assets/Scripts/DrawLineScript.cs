using UnityEngine;
using System.Collections;

public class DrawLineScript : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    public Transform origin;
    public float width;

    public float lineDrawSpeed = 50f;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>(); 
        lineRenderer.SetWidth(width, width);
        print(origin.position.x);
        print(origin.position.y);



        //if statement determining if something was hit or not
        //dist = Vector3.Distance(origin.position, destination.position);
        
	
	}

    // Update is called once per frame
    
    //removed this update to speed up gameplay

    //void Update()
    //{
    //    lineRenderer.SetPosition(0, origin.position);
    //    lineRenderer.SetPosition(1, Input.mousePosition);
    //    dist = 25f;




    //}   
}
