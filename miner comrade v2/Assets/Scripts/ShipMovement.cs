using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {

    private Rigidbody2D ship;

    public float rotationSpeed;

    public float forwardThrustForce;
    public float otherThrustForce;

    public ParticleSystem forwardThrusters;
    public ParticleSystem backwardThrusters1;
    public ParticleSystem backwardThrusters2;
    public ParticleSystem upperRightThrusters;
    public ParticleSystem upperLeftThrusters;
    public ParticleSystem lowerRightThrusters;
    public ParticleSystem lowerLeftThrusters;
    private Vector2 forceVector;
    private GameObject thrusters;




    // Use this for initialization
    //anything here will occur when object is instantiated
    void Start()
    {
        ship = GetComponent<Rigidbody2D>();
        thrusters = GameObject.Find("Thrusters");

    }

    // Update is called once per frame
    //called everytime frame is drawn


    //called at a fixed interval (once every 30th of a second), better to put physics here 
    //user else ifs on axis movement to make sure users cannot hold down both thrusters for an axis

    void Update()
    {
        if (Input.anyKey)
        {

            thrusters.active = true;

            if (Input.GetKey(KeyCode.Q))
            {
                //rotate shift to the left
                ship.angularVelocity = rotationSpeed;
                upperLeftThrusters.Stop();
                upperRightThrusters.Play();
            }
            else if (Input.GetKey(KeyCode.E))
            {
                ship.angularVelocity = -rotationSpeed;
                upperRightThrusters.Stop();
                upperLeftThrusters.Play();

            }
            else if (Input.GetKey(KeyCode.R))
            {
                ship.angularVelocity = 0f;
            }
            else
            {
                upperLeftThrusters.Stop();
                upperRightThrusters.Stop();
            }



            if (Input.GetKey(KeyCode.D))
            {
                forceVector = (transform.right * otherThrustForce);

                upperLeftThrusters.Play();
                lowerLeftThrusters.Play();
                lowerRightThrusters.Stop();
                ship.AddForce(forceVector);

            }
            else if (Input.GetKey(KeyCode.A))
            {
                forceVector = -(transform.right * otherThrustForce);

                upperRightThrusters.Play();
                lowerRightThrusters.Play();
                lowerLeftThrusters.Stop();
                ship.AddForce(forceVector);
            }
            else
            {
                lowerLeftThrusters.Stop();
                lowerRightThrusters.Stop();
            }












            if (Input.GetKey(KeyCode.S))
            {
                forceVector = -(transform.up * otherThrustForce);

                forwardThrusters.Stop();
                backwardThrusters1.Play();
                backwardThrusters2.Play();
            }
            //float yRotation = Input.GetAxis("Horizontal");
            //float xRotation = Input.GetAxis("Vertical");
            //Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
            //Vector3 direction = rotation * Vector3.forward;

            else if (Input.GetKey(KeyCode.W))
            {
                // Engage forward Thrusters
                forceVector = transform.up * forwardThrustForce;



                // Show thrust particle effect
                backwardThrusters1.Stop();
                backwardThrusters2.Stop();

                forwardThrusters.Play();
            }
            else
            {
                forwardThrusters.Stop();
                backwardThrusters1.Stop();
                backwardThrusters2.Stop();
            }

            ship.AddForce(forceVector);


        }
        else
        {
            thrusters.active = false;
        }
    }
    

}



