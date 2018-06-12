using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public ParticleSystem explosion;

    public Material flashMat;
    public Material bubbleMat;
    Material defaultMat;
    float flashTime = 0.1f;
    float flashTimer;
    bool isFlashed = false;

    

    float explosionSpeed = 60;
    float explosionAmount = 420;

    GameObject ship;

    GameObject smallgun1;
    GameObject smallgun2;
    GameObject biggun1;
    GameObject biggun2;

    //Animator smallGun1Anim;

    // Audio

    //public AudioSource laserSound;
   // public AudioSource hurtSound;

    /*
        STATS
        
     Power
     Speed
     Efficiency (Regen)
     Cooldown Reduction
     Endurance (Health)
     
     
     */



    GameObject bubble;
    GameObject shootPoint;
    GameObject skybox;
    //GameObject statusBars;

    public Vector3 velocity;
    Vector3 rotation;

    Vector3 skyboxRotation;

    public GameObject bullet;
    public GameObject powerShot;
    public GameObject chrono;
    public GameObject nova;

    float moveForce = 300;
    float maxSpeed = 300;
    //float tiltMod = -0.2f;

    public float maxHealth = 800;
    public float health;
    public float energy = 100;

    public Vector3 solarCore;
    public float coreDistance;

    public float[] solarMilestone = { 500, 1000, 1500, 2000 };

    // Face the mouse
    Vector3 mousePos;
    Vector3 mainPos;
    float mouseAngle;


    // Shooting
    float shootTimer;
    float shootPower = 400;
    float shootDelay = 0.2f;

    // Dash
    Vector3 dashVelocity;
    float dashSpeed = 1500;
    float dashTime = 0.2f;
    float dashTimer;
    bool isDashing = false;

    // Bubble
    bool isBubbled = false;
    float bubbleTime = 3;
    float bubbleTimer;
    
    // Skill costs

    public float dashCost = 10;
    public float powerShotCost = 15;
    public float novaCost = 25;
    public float chronoCost = 25;
    public float bubbleCost = 25;

    //Cooldowns
    float dashCooldown = 2.0f;
    float powerShotCooldown = 3.0f;
    float novaCooldown = 15.0f;
    float chronoCooldown = 10.0f;
    float bubbleCooldown = 12.0f;

    float dashCDTimer;
    float powerShotCDTimer;
    float novaCDTimer;
    float chronoCDTimer;
    float bubbleCDTimer;

    

    bool isAlive = true;

    bool shittyControls = false;

    void Start()
    {
        ship = transform.FindChild("Ship").gameObject;

        smallgun1 = ship.transform.FindChild("Small Gun 1").gameObject;
        smallgun2 = ship.transform.FindChild("Small Gun 2").gameObject;

        biggun1 = ship.transform.FindChild("Big Gun 1").gameObject;
        biggun2 = ship.transform.FindChild("Big Gun 2").gameObject;
 
        bubble = transform.FindChild("Bubble").gameObject;
        shootPoint = ship.transform.FindChild("Shoot Point").gameObject;
        skybox = transform.FindChild("Skybox Camera").gameObject;

        dashCDTimer = dashCooldown;
        powerShotCDTimer = powerShotCooldown;
        novaCDTimer = novaCooldown;
        chronoCDTimer = chronoCooldown;
        bubbleCDTimer = bubbleCooldown;

        //smallGun1Anim = smallgun1.GetComponent<Animator>();

        //statusBars = transform.Find("Canvas").gameObject;//FindChild("Status Bars").gameObject;

        //health = maxHealth;

        defaultMat = ship.GetComponent<Renderer>().material;

        solarCore = new Vector3(1000, 0, 1000);

        //if (Application.isEditor)
         //   shittyControls = true;
    }

    public float SolarDistance()
    {
        return Vector3.Magnitude(transform.position - solarCore);
    }

    void Update()
    {
        if (isAlive)
        {
            // Cooldowns 
            dashCDTimer += Time.deltaTime;
            powerShotCDTimer += Time.deltaTime;
            novaCDTimer += Time.deltaTime;
            chronoCDTimer += Time.deltaTime;
            bubbleCDTimer += Time.deltaTime;

            // Switches on my weird control sceme if in the editor
            if (shittyControls)
                ShittyKeyboardControls();
            else
                KeyboardControls();

            OtherControls();

            // Check if alive
            if (health <= 0)
            {
                isAlive = false;
                Explode();
            }

            //Movement & Dash
            if (!isDashing)
                transform.position += velocity * Time.deltaTime;
            else
            {
                dashTimer += Time.deltaTime;

                if (dashTimer >= dashTime)
                {
                    velocity = dashVelocity * 0.25f;
                    isDashing = false;
                    dashTimer = 0;
                }

                transform.position += dashVelocity * Time.deltaTime * Mathf.Asin(dashTimer / dashTime);
            }

            if (transform.position.y != 0)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            velocity.x *= 0.99f;// *Time.deltaTime;
            velocity.z *= 0.99f;// *Time.deltaTime;

            // Flash when takeing damage
            if (isFlashed)
            {
                flashTimer += Time.deltaTime;

                if (flashTimer >= flashTime)
                {
                    Flash(false);                    
                    flashTimer = 0;
                }
            }

            // Bubble Timer

            if(isBubbled)
            {
                bubbleTimer += Time.deltaTime;

                if (bubbleTimer >= bubbleTime)
                {
                    bubbleTimer = 0;
                    isBubbled = false;
                    ChangMat(defaultMat);
                    bubble.GetComponent<Bubble>().Close();
                }
            }

            // Energy

            if (energy <= 0)
            {
                energy = 0;
                health -= Time.deltaTime * 10;
            }


            if (SolarDistance() <= solarMilestone[0])
            {
                energy += Time.deltaTime * 20;
            }
            else if (SolarDistance() <= solarMilestone[1])
            {
                energy += Time.deltaTime * 10;
            }
            else if (SolarDistance() <= solarMilestone[2])
            {
                energy += Time.deltaTime * 5;
            }
            else if (SolarDistance() <= solarMilestone[3])
            {
                energy -= Time.deltaTime;
            }
            else
            {
                energy -= Time.deltaTime * 5;
            }

            /// Converts extra energy into health

            if (energy > 100)
            {
                if (health < maxHealth)
                    health += (energy - 100) * 0.1f;

                energy = 100;
            }
                


            // Face the mouse

            mainPos = Camera.main.WorldToViewportPoint(transform.position);
            mousePos = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

            mouseAngle = Mathf.Atan2(mainPos.y - mousePos.y, mainPos.x - mousePos.x) * Mathf.Rad2Deg;

            rotation.y = mouseAngle * -1 - 135;
            ship.transform.rotation = Quaternion.Euler(rotation);

            skyboxRotation.x = transform.position.x * -0.02f;
            skyboxRotation.y = transform.position.z * -0.02f;

            skybox.transform.rotation = Quaternion.Euler(skyboxRotation);

        }
    }

    // Returns percentage of cooldown remaining
    public float GetCooldown(string name)
    {
        switch(name)
        {
            case "Dash":
                return dashCDTimer / dashCooldown;

            case "Power Shot":
                return powerShotCDTimer / powerShotCooldown;

            case "Nova":
                return novaCDTimer / novaCooldown;

            case "Chronoshpere":
                return chronoCDTimer / chronoCooldown;

            case "Bubble":
                return bubbleCDTimer / bubbleCooldown;

            default:
                Debug.Log("Cooldown Not Found");
                return 0;
        }
    }

    // Lots of cubes // needs to be redone in particle system
    void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(ship);
    }

    // Flash when take damgage, true = change texture, false = change back
    void Flash(bool flash)
    {
        if (flash) 
            ChangMat(flashMat);
        else
            ChangMat(defaultMat);

        isFlashed = flash;
    }

    // Change the material of all models
    void ChangMat(Material newMAt)
    {
        ship.GetComponent<Renderer>().material = newMAt;
        smallgun1.GetComponent<Renderer>().material = newMAt;
        smallgun2.GetComponent<Renderer>().material = newMAt;
        biggun1.GetComponent<Renderer>().material = newMAt;
        biggun2.GetComponent<Renderer>().material = newMAt;
    }

    // On Collision
    void OnTriggerEnter(Collider col)
    {
        if (isAlive && !isBubbled)
        {
            if (col.gameObject.tag == "Enemy")
            {
                health -= col.GetComponent<BaseAI>().GetImpact();
                Flash(true);
                GetComponent<AudioSource>().Play();
            }

            if (col.gameObject.tag == "Enemy Laser")
            {
                health -= col.GetComponent<Laser>().damage;
                Flash(true);
                GetComponent<AudioSource>().Play();
            }
        }

        if (col.gameObject.tag == "Red Sun")
        {
            health = 0;
        }
    }

    // Called 
    void OnTriggerStay(Collider col)
    {
        if (isAlive && !isBubbled)
        {
            if (col.gameObject.tag == "Enemy Laser Beam")
            {
                health -= 300 * Time.deltaTime;
                Flash(true);
                GetComponent<AudioSource>().Play();
            }
        }
    }


    // Skills
    void Shoot()
    {
        // Animation

        //smallGun1Anim.Play("SmallGun1");

        smallgun1.GetComponent<Animator>().Play("SmallGun1");



        //laserSound.Play();

        GameObject clone = Instantiate(bullet, shootPoint.transform.position, transform.rotation) as GameObject;

        clone.GetComponent<Laser>().velocity = new Vector3(
            (mousePos.y - mainPos.y) * 1, 0, (mousePos.x - mainPos.x) * -1).normalized * shootPower;


        //clone = Instantiate(bullet, shootPoint.transform.position, transform.rotation) as Rigidbody;
        //clone.velocity = Quaternion.AngleAxis(-45, Vector3.up) 
        //    * new Vector3((mousePos.y - mainPos.y) * 1, 0, (mousePos.x - mainPos.x) * -1).normalized * shootPower  
    }

    void PowerShot()
    {
       GameObject clone = Instantiate(powerShot, shootPoint.transform.position, transform.rotation) as GameObject;

       clone.GetComponent<Laser>().velocity = new Vector3(
            (mousePos.y - mainPos.y) * 1, 0, (mousePos.x - mainPos.x) * -1).normalized * shootPower * 1.5f;
    }

    void Nova()
    {
        GameObject clone = Instantiate(nova, transform.position, transform.rotation) as GameObject;
        clone.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    void Chronoshpere()
    {
        GameObject clone = Instantiate(chrono, transform.position, transform.rotation) as GameObject;
    }

    void Bubble()
    {
        ChangMat(bubbleMat);
        isBubbled = true; 
        bubble.SetActive(true);   
    }

    // Calls status bars in a really dumb way
    public float CheckBubble()
    {
        if (isBubbled)
            return bubbleTime;
        else
            return 0;
    }

    void OtherControls()
    {
        // Shoot
        shootTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (shootTimer <= 0)
            {
                Shoot();
                shootTimer = shootDelay;
            }
        }

        // Power Shot
        if (Input.GetMouseButtonDown(1))
        {
            if (powerShotCDTimer >= powerShotCooldown && energy >= powerShotCost)
            {
                PowerShot();
                shootTimer = shootDelay;
                powerShotCDTimer = 0;
                energy -= powerShotCost;
            }
        }

        // Dash
        if (Input.GetKey(KeyCode.Space))
        {
            if (dashCDTimer >= dashCooldown && energy >= dashCost)
            {
                dashCDTimer = 0;
                energy -= dashCost;
                dashVelocity = Vector3.Normalize(new Vector3(mousePos.y, 0, mousePos.x * -1)
                    - new Vector3(mainPos.y, 0, mainPos.x * -1)) * dashSpeed;
                isDashing = true;
            }
        }

        // Nova
        if (Input.GetKey(KeyCode.Q))
        {
            if (novaCDTimer >= novaCooldown && energy >= dashCost)
            {
                Nova();
                energy -= dashCost;
                novaCDTimer = 0;
            }
        }

        // Bubble
        if (Input.GetKey(KeyCode.R))
        {
            if (bubbleCDTimer >= bubbleCooldown && energy >= bubbleCost)
            {
                Bubble();
                bubbleCDTimer = 0;
                energy -= bubbleCost;
            }
        }
    }

    //Real Keyboard Controls
    void KeyboardControls()
    {
        if (Input.GetKey(KeyCode.W)) // Up
        {
            if (velocity.x <= maxSpeed)
                velocity.x += moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S)) // Down
        {
            if (velocity.x >= -maxSpeed)
                velocity.x -= moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A)) // Left
        {
            if (velocity.z <= maxSpeed)    
                velocity.z += moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D)) // Right
        {
            if (velocity.z >= -maxSpeed)
                velocity.z -= moveForce * Time.deltaTime;
        }



        // Chronoshpere!!!!!!!!!!!!!!!!!
        if (Input.GetKey(KeyCode.E))
        {
            if (chronoCDTimer >= chronoCooldown && energy >= chronoCost)
            {
                chronoCDTimer = 0;
                Chronoshpere();
                energy -= chronoCost;
            }
        }



    }

    //Shitty Keyboard Controls
    void ShittyKeyboardControls()
    {
        if (Input.GetKey(KeyCode.E)) // Up
        {
            if (velocity.x <= maxSpeed)
                velocity.x += moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D)) // Down
        {
            if (velocity.x >= -maxSpeed)
                velocity.x -= moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A)) // Left
        {
            if (velocity.z <= maxSpeed)
                velocity.z += moveForce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.F)) // Right
        {
            if (velocity.z >= -maxSpeed)
                velocity.z -= moveForce * Time.deltaTime;
        }


        // Chronoshpere!#!$!
        if (Input.GetKey(KeyCode.W))
        {
            if (chronoCDTimer >= chronoCooldown && energy >= chronoCost)
            {
                chronoCDTimer = 0;
                Chronoshpere();
                energy -= chronoCost;
            }
        }
    }
}
