using UnityEngine;
using System.Collections;

public enum GameState
{
    NORMAL, WARP, STUNNED
}

public class BaseAI : MonoBehaviour
{
    public Material flashMat;
    public Material stunMat;

    public ParticleSystem explosion;

    protected Material defaultMat;

    float flashTime = 0.05f;
    float flashTimer;
    bool isFlashed = false;

    protected Vector3 velocity;
    protected Vector3 rotation;
    protected float rotateSpeed;

    protected float health = 100;
    protected float maxHealth = 100;

    protected float impactDamage = 10;

    protected float explosionAmount = 25;
    protected float explosionSpeed = 25;
    protected float restHeight = 0;

    protected bool stunImmune = false;
    protected bool timeImmune = false;

    float stunTime;

    float warpSpeed = 3000;

    float timeDilation = 1;

    public GameState gameState = GameState.NORMAL;

    public float GetImpact()
    {
        return impactDamage;
    }

    protected void Awake()
    {
        defaultMat = GetComponent<Renderer>().material;
    }

    protected virtual void Update()
    {
        if (health <= 0)
        {
            Explode();
        }

        if (isFlashed)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashTime)
            {
                if (gameState != GameState.STUNNED)
                    ChangeMat(defaultMat);
                else
                    ChangeMat(stunMat);

                isFlashed = false;
                flashTimer = 0;
            }    
        }

        if (gameState == GameState.WARP)
        {
            velocity = new Vector3(0, warpSpeed, 0);

            if (transform.position.y >= 0)
            {
                gameState = GameState.NORMAL;
                velocity.y = 0;
                transform.position = new Vector3(transform.position.x, restHeight, transform.position.z);
                ChangeMat(defaultMat);
            }
                
        }
       
        if(gameState == GameState.STUNNED)
        {
            stunTime -= Time.deltaTime;

            if (stunTime <= 0)
            {
                gameState = GameState.NORMAL;
                ChangeMat(defaultMat);
            }
                
        }
        else
        {
            rotation.y += rotateSpeed * Time.deltaTime * timeDilation;
            transform.rotation = Quaternion.Euler(rotation);
            transform.position += velocity * Time.deltaTime * timeDilation;

            if (timeDilation < 1)
                timeDilation += Time.deltaTime;
        }


    }

    protected virtual GameObject TargetPlayer()
    {
        return GameObject.Find("Player");
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            health -= 200;

            if (flashMat)
            {
                ChangeMat(flashMat);
                isFlashed = true;
            }
        }

        if (col.gameObject.tag == "Laser")
        {
            health -= col.GetComponent<Laser>().damage;

            if (flashMat)
            {
                ChangeMat(flashMat);
                isFlashed = true;
            }
        }

        if (col.gameObject.tag == "Stun Laser")
        {
            health -= col.GetComponent<Laser>().damage;

            if (gameState == GameState.NORMAL && !stunImmune)
            {
                gameState = GameState.STUNNED;
                stunTime = col.GetComponent<Nova>().stunDuration;
            }

            if (flashMat)
                ChangeMat(stunMat);

        }
    }

    protected virtual void ChangeMat(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }

    protected virtual void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Chrono" && !timeImmune)
        {
            timeDilation = 0.2f;
        }
    }

    protected virtual void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }
}
