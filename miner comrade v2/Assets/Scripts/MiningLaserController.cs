using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class MiningLaserController : MonoBehaviour
{

    


    //public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public int range = 100;

    float timer;
    Ray laserRay;
    RaycastHit laserHit;
    ParticleSystem laserParticles;
    LineRenderer laserLine;
    //AudioSource laserAudio;
    //Light laserLight
    float effectsDisplayTime = 0.2f;
    public float dirX;
    public float dirY;
    public Vector2 dir;

 

    void Start()
    {
        laserParticles = GetComponent<ParticleSystem>();
        laserLine = GetComponent<LineRenderer>();
        //laserAudio = getComponent<AudioSource>();
        //laserLight = getComponent<Light>();
    
    }

    void Update()
    {


        timer += Time.deltaTime;

        if (Input.GetButton ("Fire1") && timer >= timeBetweenBullets)
        {
            Shoot();
        }
        
        else
        {
            DisableEffects();
        }
        

      //  if (timer >= timeBetweenBullets * effectsDisplaytime)
      //  {
       //     DisableEffects();
       // }
    }

    public void DisableEffects()
    {
        laserLine.enabled = false;
        //laserLight.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;
        //laserAudio.Play();

        //laserLight.enabled = true;

        laserParticles.Stop();
        laserParticles.Play();

        laserLine.enabled = true;

        laserLine.SetPosition(0, transform.position);

        /*                                                                                                                                                                                                                                          
        laserRay.origin = target.position;
        
            
        Vector2 dir = Camera.main.ViewportPointToRay(Input.mousePosition);
        Vector2 newDir = new Vector3(dir.x * range, dir.y * range);
        dirX = dir.x;
        dirY = dir.y;

        laserRay.direction = newDir;
        */

       laserRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        
 



        Debug.DrawRay(laserRay.origin, laserRay.direction * range, Color.green);

        laserLine.SetPosition(1, (laserRay.origin + laserRay.direction * range));
        //transform.forward;

        /*
        if(Physics.Raycast (laserRay, out laserHit, range))
        {
            
            code here for future collision with asteroids
             ChunkHealth chunckHealth = laserHit.collider.GetComponent<ChunkHealth>();
            if (chunkHealth != null)
             {
             chunkHealth.TakeDamage (damagePerShot, laserHit.point);
             }
            laserLine.SetPosition(1, laserHit.point);
            
        }
        else
        {
        
        dirX = laserRay.direction.x;
            dirY = laserRay.direction.y;
            laserLine.SetPosition(1, laserRay.origin + laserRay.direction * range);
        //}

    */
    }
}

