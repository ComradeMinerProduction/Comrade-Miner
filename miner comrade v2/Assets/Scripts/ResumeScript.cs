using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResumeScript : MonoBehaviour {

    public float minZoom;           // min for camera zoom
    public float maxZoom;           // max for camera zoom
    public float zoomSpeed;         // how fast the zoom moves
    private float currentZoom;      // current camera distance from player


    //  static references to other gameObjects and player
    public GameObject ConsoleUI;
    public GameObject RedStarContent;
    public GameObject SystemMapContent;
    public GameObject OptionsContent;
    private GameObject currentObject;
    private GameObject previousObject;
    private GameObject playerShip;
    // private CollisionController colScript;
    private GameObject mainCamera;
    public GameObject warpVector;
    public GameObject warpButton;

    public GameObject notifierText;
    private Text notification;

    private bool paused = false;            //  Is the game paused, sets timeScale to 0;

    void Start()
    {
        playerShip = GameObject.Find("PlayerShip");
        ConsoleUI.SetActive(false);
        OptionsContent.SetActive(false);
        SystemMapContent.SetActive(false);
        currentObject = RedStarContent;
        previousObject = SystemMapContent;
        // colScript = playerShip.GetComponent<CollisionController>()  -  ;
        warpButton.SetActive(false);
        notification = notifierText.GetComponent<Text>();
        notifierText.SetActive(false);
        mainCamera = playerShip.GetComponentInChildren<Camera>().gameObject;
        currentZoom = mainCamera.GetComponent<Camera>().orthographicSize;

    }

    void Update()
    {

        //  mousewheel zoom input
        float zoomChange = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= zoomChange * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        //  mousewheel moves camera
        if (zoomChange != 0)
        {
            mainCamera.GetComponent<Camera>().orthographicSize = currentZoom;
        }

        if (Input.GetButtonDown("Console"))
        {
            if (paused == true)
            {
                paused = false;
            } else
            {
                paused = true;
            }
            
        } 

        if (paused)
        {
            ConsoleUI.SetActive(true);
            previousObject.SetActive(false);
            currentObject.SetActive(true);
            Time.timeScale = 0.0f;
        }

        if (!paused)
        {
            ConsoleUI.SetActive(false);
            Time.timeScale = 1.0f;
        }
        
       
    }

    

    public void Restart()
    {
        SceneManager.LoadScene("baseScreen");
        //Application.LoadLevel("baseScreen");
    }

    public void MainMenu()
    {
        //SceneManager.LoadScene(SceneManager get scene at 0)
        warpButton.SetActive(false);
        SceneManager.LoadScene("StartScreen");
        //Application.LoadLevel("StartScreen");
    }

    public void Quit()
    {
        Application.Quit();
        warpButton.SetActive(false);
    }

    public void RedStar()
    {
        // This previousObject / currentObject  looks like it needs a static controller or a singleton
        previousObject = currentObject;
        currentObject = RedStarContent;
        warpButton.SetActive(false);
    }

    public void SystemMap()
    {
        previousObject = currentObject;
        currentObject = SystemMapContent;
        warpButton.SetActive(true);
    }


    public void Options()
    {
        previousObject = currentObject;
        currentObject = OptionsContent;
        warpButton.SetActive(false);
    }

    public void warpInitiate()
    {
        warpVector.SetActive(true);
        paused = false;
        notifierText.SetActive(true);
        notification.text = "Align on top of Warp Vector to Warp";

    }






}
