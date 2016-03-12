using UnityEngine;
using System.Collections;

public class StationRotator : MonoBehaviour {

    public float speedOfRotation;

	void Awake()
    {
        StartCoroutine(AutoRotate());
    }


    IEnumerator AutoRotate() {
        while (true )
        {
            for (int i = 0; i < 720; i++)
            {
                yield return new WaitForSeconds(0.001f * speedOfRotation);
                transform.Rotate(0.0f, 0.5f, 0.0f);

            }
        }
    }

    // Update function removed as part of speed optimization
    
    //// Update is called once per frame
    //void Update () {
    //    transform.Rotate(Vector3.forward * speedOfRotation, Time.deltaTime);
    //}
}
