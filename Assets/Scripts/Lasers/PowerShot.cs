using UnityEngine;
using System.Collections;

public class PowerShot : Laser
{
    //float scaleRate = 0.5f;

    void Awake()
    {
        damage = 800;
        lifeTime = 25;
    }

    //void Update()
    //{
    //    //transform.localScale = transform.localScale + new Vector3(scaleRate, 0, scaleRate);
    //}

    protected override void OnTriggerEnter(Collider col)
    {
        Explode();

        if(col.gameObject.tag == "Red Sun")
        {
            Destroy(this.gameObject);
        }

        //damage -= 50;

        //if (damage <= 0)
        //    Destroy(this.gameObject);

        //if (col.gameObject.tag == "Red Laser")
        //{
        //    Destroy(this.gameObject);
        //}
    }
}
