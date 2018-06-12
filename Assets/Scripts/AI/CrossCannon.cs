using UnityEngine;
using System.Collections;

public class CrossCannon : BaseAI 
{
    public GameObject cannonBall;
    GameObject target;
    GameObject[] shootPoint = new GameObject[4];

    float slowSpeed = 0;
    float fastSpeed = 40;
    float shootDelay = 1.7f;
    float shootSpeed = 300;

    float campDistance = 500;

    float timer;
    float targetDistance;
    float speed;


	void Start () 
    {
        health = 600;
        impactDamage = 20;
        explosionAmount = 100;
        explosionSpeed = 50;

        target = TargetPlayer();

        for (int i = 0; i <= 3; i++)
        {
            shootPoint[i] = transform.FindChild("" + i).gameObject;
        }


        rotateSpeed = -60;
	}


    override protected void Update()
    {
        if (gameState == GameState.NORMAL)
        {

            targetDistance = Vector3.Magnitude(target.transform.position - transform.position);

            if (targetDistance <= campDistance)
                speed = slowSpeed;
            else
                speed = fastSpeed;

            timer += Time.deltaTime;
            if (timer >= shootDelay)
            {
                timer = 0;
                ShootCannons();
            }

            velocity = Vector3.Normalize(target.transform.position - transform.position) * speed;     
        }

        base.Update();
    }

    void ShootCannons()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject clone = Instantiate(cannonBall, shootPoint[i].transform.position, transform.rotation) as GameObject;
            clone.GetComponent<Laser>().velocity = shootPoint[i].transform.forward * shootSpeed;   
        }
    }
}
