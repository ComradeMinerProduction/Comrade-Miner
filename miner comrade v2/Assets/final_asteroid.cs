using UnityEngine;
using System.Collections;

public class final_asteroid : MonoBehaviour {

	public float health;

	public Rigidbody asteroid1prefab;

	public Rigidbody2D asteroidprefab;

	public Rigidbody2D oreprefab;

	public int numberofObjects;

    public AudioSource destroySound;

	void Update ()
	{

        if (Input.GetButton("Fire1"))
        {
            // (Input.GetButtonDown("Fire1")) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
                if (hit.collider.gameObject == gameObject)
                 health -= 10;
                 } 
	        	else if (health < 10) {
                 Destroy(gameObject);
                }



        if (health < 10)
        {
            Instantiate(oreprefab, asteroidprefab.transform.position, asteroidprefab.transform.rotation);
            Destroy(gameObject);
        }

        if (health < 10)
        {
            for (int i = 0; i < numberofObjects; i++)
            {
                Instantiate(asteroid1prefab, asteroidprefab.transform.position, asteroidprefab.transform.rotation);
            }
            DestroyImmediate(asteroidprefab, true);
            
        }

        

        if (health > 10)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.green);
        }
        else if (health < 20)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.red);
        }



    }

}