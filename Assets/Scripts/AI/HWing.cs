using UnityEngine;
using System.Collections;

public class HWing : BaseAI  
{
    public GameObject laser;

    GameObject[] shootPoint = new GameObject[2];
    
    GameObject target;
    Vector3 orbit;

    float orbitDistance;

    float speed;
    float shootSpeed = 450;

    float shootDelay = 5;
    float shootTimer;

    bool gunSwap = true;

    void Start()
    {
        health = 400;
        impactDamage = 20;

        orbitDistance = Random.Range(500, 800);
        speed = Random.Range(150, 250);
        
        target = TargetPlayer();

        shootPoint[0] = transform.FindChild("Shoot Point 1").gameObject;
        shootPoint[1] = transform.FindChild("Shoot Point 2").gameObject;
    }


    override protected void Update()
    {
        if(gameState == GameState.NORMAL)
        {
            shootTimer += Time.deltaTime;

            if (shootTimer >= shootDelay)
            {
                if (gunSwap)
                {
                    shootTimer = 0;
                    Shoot(0);
                    gunSwap = false;
                }
                else
                {
                    shootTimer = shootDelay - 1;
                    Shoot(1);
                    gunSwap = true;
                }
            }


            // Orbit around player
            orbit = target.transform.position - transform.position;
            orbit = new Vector3(orbit.x * Mathf.Cos(90) - orbit.z * Mathf.Sin(90), 0,
                                          orbit.x * Mathf.Sin(90) + orbit.z * Mathf.Cos(90));
            orbit = Vector3.Normalize(orbit) * orbitDistance;
            orbit = target.transform.position + orbit;

            velocity = Vector3.Normalize(orbit - transform.position) * speed;

            rotation.y = (Mathf.Atan2(transform.position.x - target.transform.position.x,
                transform.position.z - target.transform.position.z) * Mathf.Rad2Deg) - 135;
        }

        base.Update();
    }

    void Shoot(int num)
    {
        GameObject clone = Instantiate(laser, shootPoint[num].transform.position, transform.rotation) as GameObject;
        clone.GetComponent<Laser>().velocity = shootPoint[num].transform.forward * shootSpeed;   
    }

}
