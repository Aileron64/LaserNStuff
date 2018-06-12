using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour 
{
    public ParticleSystem explosion;

    //public Rigidbody debris;

    public Vector3 velocity;

    int debrisNum = 10;

    public float damage = 100;
    protected float lifeTime = 20;
    protected float timeDilation = 1;

    protected virtual void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        transform.position += velocity * Time.deltaTime * timeDilation;


        //if (timeDilation < 1)
        //    timeDilation += Time.deltaTime;
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Red Sun")
        {
            Explode();

            //damage -= 100;
            //transform.localScale *= 0.66f;

            //if (damage <= 0)
            Destroy(this.gameObject);
        }

        if (col.gameObject.tag == "Red Laser")
        {
            Explode();
            Destroy(this.gameObject);
        }  
    }

    //void OnTriggerStay(Collider col)
    //{
    //    if (col.gameObject.tag == "Chrono")
    //    {
    //        timeDilation = 0.2f;
    //    }
    //}

    protected virtual void Explode()
    {
        //for(int i = 0; i <= debrisNum; i++)
        //{
        //    Rigidbody clone = Instantiate(debris, transform.position,
        //        Quaternion.Euler(new Vector3(
        //        Random.Range(-100, 100),
        //        Random.Range(-100, 100),
        //        Random.Range(-100, 100)))) as Rigidbody;

        //    clone.velocity = GetComponent<Rigidbody>().velocity * 0.3f
        //        + new Vector3(
        //        Random.Range(-100, 100),
        //        Random.Range(-200, 300),
        //        Random.Range(-100, 100));

        //}

        Instantiate(explosion, transform.position, Quaternion.identity);
    }
}
