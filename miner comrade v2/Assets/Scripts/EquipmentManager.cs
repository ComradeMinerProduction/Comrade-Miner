using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour {

    private Text currentEquipmentNotice;
    public GameObject tractorBeam;
    public GameObject miningLaser;
    public TractorBeamRotator tbCode;
    public MiningLaserController mlCode;
    public GameObject current;
    public GameObject previous;
    public bool tractorBeamOwned;
    public int laserPower;

    private int weaponTempint;

    void Start () {
        currentEquipmentNotice = transform.GetComponent<Text>();
        currentEquipmentNotice.text = "Mining Laser, press right mouse to switch";
        mlCode = miningLaser.GetComponent<MiningLaserController>();
        tbCode = tractorBeam.GetComponent<TractorBeamRotator>();
        current = miningLaser;
        previous = tractorBeam;
        tractorBeam.SetActive(false);
        tractorBeamOwned = false;
        laserPower = 5;
        weaponTempint = 0;



    }
	
    // this code can be changed to be more extensible and to use assitant functions
    // the calls for the switch can occur within the update, but the switch code should
    // delegate a class of weapon and be outside of the update 


	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            weaponTempint += 1;
            // increase if more weapons get added
            if (weaponTempint > 1)
                weaponTempint = 0;
            SwitchWeapon(weaponTempint);
        }
        // if Input.GetKeyDown("1")
        //{
        //    SwitchWeapon(1);
        //}

        //if (Input.GetMouseButtonUp(1) )
        //{
            
        //    previous = current;
            

        //    if (previous == tractorBeam) {
        //        current = miningLaser;
        //        currentEquipmentNotice.text = "Mining Laser, press right mouse to switch";
        //        miningLaser.SetActive(true);
        //        tractorBeam.SetActive(false);
        //    } else if (tractorBeamOwned == true)
        //    {
        //        current = tractorBeam;
        //        currentEquipmentNotice.text = "Tractor Beam, press right mouse to switch";
        //        miningLaser.SetActive(false);
        //        tractorBeam.SetActive(true);
        //    } 
        //}
	}
    void SwitchWeapon(int weaponIndex)
    {
        previous = current;
        switch (weaponIndex)
        {
            case 0:
                current = miningLaser;
                currentEquipmentNotice.text = "Mining Laser, press right mouse to switch";
                break;
            case 1:
                if (tractorBeamOwned == false)
                    return;
                current = tractorBeam;
                currentEquipmentNotice.text = "Tractor Beam, press right mouse to switch";
                break;
            //case weaponIndex
                // current = weaponGO;
                // currentEquipmentNotice.text = "WeaponName, press right mouse to switch";
            default:
                break;
        }
        previous.gameObject.SetActive(false);
        current.gameObject.SetActive(true);
        //maybe some code for placement if ships have differnt locations of weapons
    }
}
