using UnityEngine;
using System.Collections;

public class MotherShip : BaseAI 
{
    public GameObject drone;
    GameObject target;
    GameObject[] spawnPoint = new GameObject[4];

    float speed = 20;
    float spawnTime = 2;
    float campDistance = 500;

    float timer;
    float targetDistance;
    int spawnCount;

    void Start() 
    {
        health = 1500;
        impactDamage = 40;
        explosionAmount = 420;
        explosionSpeed = 100;

        target = TargetPlayer();

        for (int i = 0; i <= 3; i++)
        {
            spawnPoint[i] = transform.FindChild("" + i).gameObject;
        }

        rotateSpeed = 20;
	}

    override protected void Update() 
    {
        if(gameState == GameState.NORMAL)
        {

            targetDistance = Vector3.Magnitude(target.transform.position - transform.position);

            if (targetDistance <= campDistance)
                speed = 10;
            else
                speed = 50;

            timer += Time.deltaTime;
            if (timer >= spawnTime)
            {
                timer = 0;
                SpawnAdds();
            }

            rotation.y += rotateSpeed * Time.deltaTime;


            velocity = Vector3.Normalize(target.transform.position - transform.position) * speed;


            //if (transform.position.y < 0)
            //    velocity.y = transform.position.y * -1;
        }
     
        base.Update();
	}


    void SpawnAdds()
    {
        if (spawnCount >= 4)
        {
            spawnCount = 0;
            timer = -8;
        }
     
        GameObject clone = Instantiate(drone, spawnPoint[spawnCount].transform.position, transform.rotation) as GameObject;
        //complety pointless but clone has to do something because unity
        clone.transform.rotation = Quaternion.Euler(rotation); 

        spawnCount++;
    }

    //override protected void Explode()
    //{
    //    Debug.Log("DERP");
    //}
}
