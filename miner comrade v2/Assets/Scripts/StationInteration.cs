using UnityEngine;
using System.Collections;

public class StationInteration : MonoBehaviour {

    private bool notified;
    CollisionController enteringShip;

    void Awake() {
        notified = false;
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !notified)
        {
            notified = true;
            enteringShip = other.gameObject.GetComponent<CollisionController>();
            enteringShip.collidingStation = true;
            if (!enteringShip.dockNotification)
            {
                enteringShip.dockNotification = true;
                enteringShip.notifierText.SetActive(true);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject == enteringShip.gameObject)
        {
            enteringShip.dockNotification = false;
            enteringShip.notifierText.SetActive(false);
            enteringShip.collidingStation = false;
            notified = false;
        }
    }
}

