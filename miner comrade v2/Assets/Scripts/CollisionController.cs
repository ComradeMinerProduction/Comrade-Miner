using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollisionController : MonoBehaviour {

    public GameObject notifierText;
    private Text notification;
    public GameObject station;
    public Collider2D stationCollider;
    public GameObject playerShip;
    public GameObject warpVector;
    public string currentLevel;
    public bool docked;
    public Rigidbody2D shipBody;
    public GameObject stationConsole;
    private bool collidingStation;
    public GameObject mainCamera;
    public AudioSource deathSound;
    
//    private GameObject console;
    private GameObject thrusters;
    public bool detachButPressed;
    private PlayerStatController playerStats;




	// Use this for initialization
	void Start () {
        stationConsole.SetActive(false);
        docked = false;
        collidingStation = false;
        notifierText.SetActive(false);
        stationCollider = station.GetComponent<Collider2D>();
        shipBody = playerShip.GetComponent <Rigidbody2D> ();
        notification = notifierText.GetComponent<Text>();
        thrusters = GameObject.Find("Thrusters");
        detachButPressed = false;
        playerStats = playerShip.GetComponent<PlayerStatController>();
        warpVector.SetActive(false);
        




    }
	
	// Update is called once per frame
	void Update () {

        
        if (collidingStation == true)
            {
                notifierText.SetActive(true);
            } else

            {
                notifierText.SetActive(false);
            }
        




        if (docked == false)
            {
                if ((collidingStation == true) && (Input.GetKey(KeyCode.Return)))
                {
                docked = true;
                } else
                 {
                detachButPressed = false;
                }

            }


        if (docked == true)
            {
                if (detachButPressed == true)
                     {
                      docked = false;
                      notification.text = "PRESS ENTER TO DOCK WITH STATION";
                      thrusters.active = true;
                      stationConsole.SetActive(false);

            } 
                 else
                    {
                    shipBody.angularVelocity = 0;
                    shipBody.velocity = Vector2.zero;
                    notifierText.SetActive(false);
                    thrusters.active = false;
                    stationConsole.SetActive(true);

                    }
                
             }
              
        }

       
        

    


    void OnCollisionEnter2D (Collision2D other) 
    {
        
       


        if (other.gameObject.name == "Station")
        {
            collidingStation = true;

        }

        if (other.gameObject.name == "WarpVector")
        {
            if (currentLevel == "baseScreen")
            {
                Application.LoadLevel("asteroidCluster");
            } else 
            {
                Application.LoadLevel("baseScreen");
            }
        }

        if (other.gameObject.tag == "cube" || other.gameObject.name == "miniCube")
        {
          //  playerStats.hullStrength -= 5;
            deathSound.Play();
            Application.LoadLevel("StartScreen");
        }

        if (other.gameObject.tag == "ore")
        {
            Destroy(other.gameObject);
            playerStats.currentCargo += 5;
            print("Ore Collected!");

        }

        
        /*
        if (other.gameObject.name == "Station" && (Input.GetKey("return") == true) && (landed = false)){
            shipBody.velocity = Vector2.zero;
            shipBody.angularVelocity = 0;
        } 
        */
       
    }

    void OnCollisionExit2D (Collision2D other)
    {
        if (other.gameObject.name == "Station")
        {
            collidingStation = false;
        }
        
    }

}
