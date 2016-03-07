using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class MiningLaserController : MonoBehaviour
{

    


    //public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public int range = 100;
    public float laserHeight;

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
        timer = Time.time;
        laserHeight = transform.position.z;
    }

    void Update()
    {

        // changing timer to meet convention
        //timer += Time.deltaTime;

        if (Input.GetButton ("Fire1") && Time.time >= (timeBetweenBullets + timer))
        {
            timer = Time.time;
            Shoot();
        }
        
        //else
        //{
        //    DisableEffects();
        //}
        

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

        //  laserRay is the ScreenpointToRay for the mouse input
        laserRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // creates a hit from what the mouse clicked on / above
        RaycastHit2D hit = Physics2D.Raycast(laserRay.origin, laserRay.direction);

        //timer = 0f;
        //laserAudio.Play();

        //laserLight.enabled = true;

        //laserParticles.Stop();
        //laserParticles.Play();

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

        

        
        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            laserLine.SetPosition(1, new Vector3(hit.point.x, hit.point.y, laserHeight));
        } else
        {
            Vector2 missDir = new Vector2(dir.x - transform.position.x, dir.y - transform.position.y);
            laserLine.SetPosition(1, new Vector3(missDir.x * 10 + transform.position.x,
                missDir.y * 10 + transform.position.y, laserHeight));
        }
 



        // Debug.DrawRay(laserRay.origin, Vector3.forward, Color.green, 2.0f);


        // deactivate laser after time
        StartCoroutine(LaserActiveTimer());


        //laserLine.SetPosition(1, (laserRay.origin + laserRay.direction * range));
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
    IEnumerator LaserActiveTimer()
    {
        yield return new WaitForSeconds(0.1f);
        DisableEffects();
    }
}

