using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResumeScript : MonoBehaviour {

    public GameObject ConsoleUI;
    public GameObject RedStarContent;
    public GameObject SystemMapContent;
    public GameObject OptionsContent;
    private GameObject currentObject;
    private GameObject previousObject;
    private GameObject playerShip;
    CollisionController colScript;
    public GameObject warpVector;
    public GameObject warpButton;

    public GameObject notifierText;
    private Text notification;

    private bool paused = false;

    void Start()
    {
        playerShip = GameObject.Find("playerShip");
        ConsoleUI.SetActive(false);
        OptionsContent.SetActive(false);
        SystemMapContent.SetActive(false);
        currentObject = RedStarContent;
        previousObject = SystemMapContent;
        colScript = playerShip.GetComponent<CollisionController>();
        warpButton.SetActive(false);
        notification = notifierText.GetComponent<Text>();
        notifierText.SetActive(false);





    }

    void Update()
    {
        
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
        Application.LoadLevel("baseScreen");
    }

    public void MainMenu()
    {
        //SceneManager.LoadScene(SceneManager get scene at 0)
        warpButton.SetActive(false);
        Application.LoadLevel("StartScreen");
    }

    public void Quit()
    {
        Application.Quit();
        warpButton.SetActive(false);
    }

    public void RedStar()
    {
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
