using UnityEngine;
using System.Collections;

public class Nova : Laser 
{
    public float stunDuration;


    float maxStun = 5;
    float maxDamage = 150;

    float size;
    float timer;

	void Awake () 
    {
        damage = maxDamage;
        lifeTime = 0.75f;
	}
	
	void Update () 
    {
        timer += Time.deltaTime;

        damage = maxDamage * ((lifeTime - timer) / lifeTime);
        stunDuration = maxStun * ((lifeTime - timer) / lifeTime);

        size += Time.deltaTime * 2500;
        transform.localScale = new Vector3(size, size, 0);
	}

    protected virtual void OnTriggerEnter(Collider col)
    {

    }
}
