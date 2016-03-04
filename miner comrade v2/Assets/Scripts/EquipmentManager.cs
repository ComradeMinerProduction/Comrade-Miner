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

    // Use this for initialization
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



    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonUp(1) )
        {
            
            previous = current;
            

            if (previous == tractorBeam) {
                current = miningLaser;
                currentEquipmentNotice.text = "Mining Laser, press right mouse to switch";
                miningLaser.SetActive(true);
                tractorBeam.SetActive(false);
            } else if (tractorBeamOwned == true)
            {
                current = tractorBeam;
                currentEquipmentNotice.text = "Tractor Beam, press right mouse to switch";
                miningLaser.SetActive(false);
                tractorBeam.SetActive(true);
            } 



            
        }

        
	
	}
}
