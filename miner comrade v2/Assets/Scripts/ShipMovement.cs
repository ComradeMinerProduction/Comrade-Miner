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

    private bool stoppingAng;           // Is the ship stopping its rotation
    private bool stoppingHor;           // is the ship stopping its horizontal drift
    private bool notMoving;



    // Use this for initialization
    //anything here will occur when object is instantiated
    void Start()
    {
        ship = GetComponent<Rigidbody2D>();
        thrusters = GameObject.Find("Thrusters");
        notMoving = true;

    }

    // Update is called once per frame
    //called everytime frame is drawn


    //called at a fixed interval (once every 30th of a second), better to put physics here 
    //user else ifs on axis movement to make sure users cannot hold down both thrusters for an axis

    // as a note, FixedUpdate could be used instead of update, more precise controls
    // also, think about consolidating the updates into one player control script (there 
    // are a number of scripts that control the player's function on update already)

    void FixedUpdate()
    {
        //thrusters.SetActive(true);

        if (Input.GetKey(KeyCode.Q))
        {
            //rotate shift to the left
            ship.angularVelocity = rotationSpeed;
            upperLeftThrusters.Stop();
            upperRightThrusters.Play();
            stoppingAng = false;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            ship.angularVelocity = -rotationSpeed;
            upperRightThrusters.Stop();
            upperLeftThrusters.Play();
            stoppingAng = false;
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
            stoppingHor = false;
            notMoving = false;
            StopCoroutine("HorizontalStopCo");

        }
        else if (Input.GetKey(KeyCode.A))
        {
            forceVector = -(transform.right * otherThrustForce);

            upperRightThrusters.Play();
            lowerRightThrusters.Play();
            lowerLeftThrusters.Stop();
            ship.AddForce(forceVector);
            stoppingHor = false;
            notMoving = false;
            StopCoroutine("HorizontalStopCo");
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
            stoppingHor = false;
            notMoving = false;
            StopCoroutine("HorizontalStopCo");
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
            stoppingHor = false;
            notMoving = false;
            StopCoroutine("HorizontalStopCo");
        }
        else
        {
            forwardThrusters.Stop();
            backwardThrusters1.Stop();
            backwardThrusters2.Stop();
        }

        ship.AddForce(forceVector);
        if (thrusters.activeSelf != true)
            thrusters.SetActive(true);
        if (stoppingHor)
            forceVector = Vector2.zero;
        if (!stoppingAng || !stoppingHor)
        {
            
            //  starts a 0.2 second timer to stop rotation and movement
            //  This could be an option enabled under options for auto stop
            if (ship.angularVelocity != 0 && !stoppingAng)
            //    RotationStop();
            if (ship.velocity != Vector2.zero && notMoving)
                if (!stoppingHor)
                    HorizontalStop();
            //turns off the parent gameObject to the thrusters
        }
        notMoving = true;
    }
    public void RotationStop()
    {
        StartCoroutine(RotationStopCo());
        stoppingAng = true;
    }
    IEnumerator RotationStopCo()
    {
        float currentAngularVel = ship.angularVelocity;
        for (int i = 0; i < 20; i++){
            yield return new WaitForSeconds(0.05f);
            ship.angularVelocity = Mathf.Lerp(currentAngularVel, 0.0f, i / 20.0f);
        }
        ship.angularVelocity = 0.0f;
    }
    public void HorizontalStop()
    {
        StartCoroutine("HorizontalStopCo");
        stoppingHor = true;
    }
    IEnumerator HorizontalStopCo()
    {
        Vector2 currentHorVel = ship.velocity;
        for (int i = 0; i < 20; i++)
        {
            if (notMoving == false && i > 2)
                StopCoroutine("HorizontalStopCo");
            yield return new WaitForSeconds(0.05f);
            ship.velocity =new Vector2( Mathf.Lerp(currentHorVel.x, 0.0f, i / 20.0f), Mathf.Lerp(currentHorVel.y, 0.0f, i / 20.0f));
        }
        yield return new WaitForSeconds(0.5f);
        if (notMoving == true)
            ship.velocity = Vector2.zero;
    }

}



