using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollisionController : MonoBehaviour
{

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
    public bool collidingStation;
    public GameObject mainCamera;
    public AudioSource deathSound;
    public float dockDelayTime;

    //    private GameObject console;
    private GameObject thrusters;
    public bool detachButPressed;
    private PlayerStatController playerStats;
    public bool dockNotification = false;
    private bool stationTimer = false;




    // Use this for initialization
    void Start()
    {
        stationConsole.SetActive(false);
        docked = false;
        collidingStation = false;
        notifierText.SetActive(false);
        stationCollider = station.GetComponent<Collider2D>();
        shipBody = playerShip.GetComponent<Rigidbody2D>();
        notification = notifierText.GetComponent<Text>();
        thrusters = GameObject.Find("Thrusters");
        detachButPressed = false;
        playerStats = playerShip.GetComponent<PlayerStatController>();
        warpVector.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !stationTimer)
        {
            if (!docked && collidingStation)
            {
                Debug.Log("Docking");
                docked = true;
                shipBody.angularVelocity = 0;
                shipBody.velocity = Vector2.zero;
                notifierText.SetActive(false);
                thrusters.SetActive(false);
                stationConsole.SetActive(true);
                StartCoroutine(DockTimer());
                return;
            }
            if (docked && collidingStation)
            {
                Debug.Log("Undocking");
                docked = false;
                notification.text = "PRESS ENTER TO DOCK WITH STATION";
                thrusters.SetActive(true);
                stationConsole.SetActive(false);
            }
        }
    }
    IEnumerator DockTimer()
    {
        stationTimer = true;
        yield return new WaitForSeconds(dockDelayTime);
        stationTimer = false;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        

        if (other.gameObject.name == "WarpVector")
        {
            if (currentLevel == "baseScreen")
            {
                SceneManager.LoadScene("asteroidCluster");
                //Application.LoadLevel("asteroidCluster");
            }
            else
            {
                SceneManager.LoadScene("baseScreen");
                //Application.LoadLevel("baseScreen");
            }
        }

        if (other.gameObject.tag == "cube" || other.gameObject.name == "miniCube")
        {
            //  playerStats.hullStrength -= 5;
            deathSound.Play();
            SceneManager.LoadScene("StartScreen");
            //Application.LoadLevel("StartScreen");
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
}
