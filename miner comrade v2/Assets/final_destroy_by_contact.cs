using UnityEngine;
using System.Collections;

public class final_destory_by_contact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject playerExplosion;
	private game_controller gameController;
	public int scoreValue;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <game_controller>();
		}
		if (gameController == null) 
		{
			Debug.Log ("Cannot find 'GameController' script");
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "boundary")
		{
			return;
		}

		Instantiate(explosion, transform.position, transform.rotation);
		if (other.tag == "Player") 
		{
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
			gameController.GameOver ();
		}
		gameController.AddScore (scoreValue);
		Destroy (other.gameObject);
		Destroy (gameObject);
	}

}
