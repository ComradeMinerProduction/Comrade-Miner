using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TractorBeamRotator : MonoBehaviour
{
    public float suckSpeed;
    private GameObject[] ores;
    private Vector2 targetDir;
    private Vector2 forward;
    List<GameObject> oresToMine;
    private Rigidbody2D rb;
    private ParticleSystem beam;

    void Start()
    {
        beam = GameObject.Find("BeamEffect").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1"))
        {
            beam.Play();
            ores = GameObject.FindGameObjectsWithTag("ore");
            // need to turn this into one time per button down
            List<GameObject> oresToMine = getOresToMine(ores);
            
            foreach (GameObject ore in oresToMine)
            {

                Vector2 direction = ore.transform.position - transform.position;
                rb = ore.GetComponent<Rigidbody2D>();
                rb.AddRelativeForce(-direction.normalized * suckSpeed, ForceMode2D.Force);
               // rb.AddForce(-transform.forward * suckSpeed);


                //if (ore.transform.position)
            }
        } else
        {
            beam.Stop();

        }
        








    }


    public List<GameObject> getOresToMine(GameObject[] ores) 
    {
        oresToMine = new List<GameObject>();
        forward = transform.forward;
        for (int i = 0; i < ores.Length; i++)
        {
            targetDir = ores[i].transform.position - transform.position;
            float angle = Vector2.Angle(targetDir, forward);
            float distance = Vector2.Distance(ores[i].transform.position, transform.position);
            if (angle < 20.0f && distance < 15.0f)
            {
                oresToMine.Add(ores[i]);
                print("yes");

            } else
            {

            }

        }
        return oresToMine;
    }


}