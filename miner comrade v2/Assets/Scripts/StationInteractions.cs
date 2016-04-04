using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StationInteractions : MonoBehaviour {


    public int oreSellPrice;
    public GameObject playerShip;
    public CollisionController colCont;
    public PlayerStatController playerStats;
    public GameObject stationNotice;
    public GameObject numOreUnitsDisplay;
    public GameObject sellButton;
    public GameObject stationBuyingNotice;
    public GameObject item1;
    public GameObject item2;
    public GameObject item1Label;
    public GameObject item2Label;
    private Text oreUnits;
    public bool inStore;
    public GameObject hud;
    public EquipmentManager playerEquip;



    // Use this for initialization
    void Start () {
        
        colCont = playerShip.GetComponent<CollisionController>();
        playerStats = playerShip.GetComponent<PlayerStatController>();
        playerEquip = hud.GetComponent<EquipmentManager>();
        oreUnits = numOreUnitsDisplay.GetComponent<Text>();
        inStore = false;
        stationNotice.SetActive(true);
        numOreUnitsDisplay.SetActive(false);
        stationBuyingNotice.SetActive(false);
        sellButton.SetActive(false);
        item1.SetActive(false);
        item2.SetActive(false);
        item1Label.SetActive(false);
        item2Label.SetActive(false);
        UpdateOre();



    }
	
	
	void UpdateOre () {
         oreUnits.text = "You have " + playerStats.currentCargo + "Units of Ore";
	}



    public void sellOre()
    {
        playerStats.credits += (oreSellPrice * playerStats.currentCargo);
        playerStats.currentCargo = 0;
        UpdateOre();
    }


    public void Detach()
    {
        colCont.detachButPressed = true;
        print("Detach");
        
    }

    public void buyItem1()
    {
        if (playerStats.credits >= 5)
        {
            print("You Have bought Item1!");
            playerEquip.tractorBeamOwned = true;
            playerStats.credits -= 5;
            UpdateOre();

        } else
        {
            print("Not enough money!");
        }
    }

    public void buyItem2()
    {
        if (playerStats.credits >= 10)
        {
            print("You have bought Item2!");
            playerEquip.laserPower += 5;
            playerStats.credits -= 10;
            UpdateOre();
        } else
        {
            print("Not Enough money!");
        }
        
    }
    public void Store()
    {
        if (inStore == true) {
            inStore = false;
            stationNotice.SetActive(true);
            numOreUnitsDisplay.SetActive(false);
            stationBuyingNotice.SetActive(false);
            sellButton.SetActive(false);
            item1.SetActive(false);
            item2.SetActive(false);
            item1Label.SetActive(false);
            item2Label.SetActive(false);
            UpdateOre();
        } else
        {
            inStore = true;
            stationNotice.SetActive(false);
            numOreUnitsDisplay.SetActive(true);
            stationBuyingNotice.SetActive(true);
            sellButton.SetActive(true);
            item1.SetActive(true);
            item2.SetActive(true);
            item1Label.SetActive(true);
            item2Label.SetActive(true);
            UpdateOre();
        }
         
    }
}
