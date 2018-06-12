using UnityEngine;
using System.Collections;

public class RedSun : BaseAI 
{
    public Material redSunMat;
    public ParticleSystem hit;

    GameObject outer;
    GameObject inner;

    public GameObject cannon;
    GameObject shootPoint;

    float shootDelay = 1;
    float shootTimer;

	// Use this for initialization
	void Start () 
    {
        stunImmune = true;
        timeImmune = true;

        defaultMat = redSunMat;

        health = 100000;
        impactDamage = 1000;

        outer = transform.FindChild("Outer Sun").gameObject;
        inner = transform.FindChild("Inner Sun").gameObject;

        shootPoint = transform.FindChild("Shoot Point").gameObject;

        rotateSpeed = 10;
	}
	
	// Update is called once per frame
    protected override void Update() 
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootDelay)
        {
            shootTimer = 0;


            int rand = Random.Range(1, 6);

            switch (rand)
            {
                default:
                case 1:            
                for (int i = 0; i < 10; i++)
                    {
                        Attack_1(5, i, 200 + (i * 25));
                    }            
                    break;

                case 2:
                    for (int i = 0; i < 10; i++)
                    {
                        Attack_1(5, i * - 1, 200 + (i * 25));
                    }                              
                    break;

                case 3:
                    Attack_1(15, 0, 300);
                    Attack_1(15, 2, 250);
                    Attack_1(15, 4, 200);
                    break;

                case 4:
                    Attack_1(20, 0, 300);
                    Attack_1(20, 4, 200);
                    break;

                case 5:
                    Attack_1(10, 0, 350);
                    Attack_1(10, 2, 300);
                    Attack_1(10, 4, 250);
                    Attack_1(10, 6, 200);
                    break;
            }
        }


        base.Update();
	}

    void Attack_1(int num, float rotDelay, float shootSpeed)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject clone = Instantiate(cannon, shootPoint.transform.position, transform.rotation) as GameObject;

            float angle = 0 + i * (6.28f / num) + rotDelay * 0.1f;

            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            clone.GetComponent<Laser>().velocity = direction * shootSpeed;
        }
    }

    protected override void ChangeMat(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }

    //protected virtual void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.tag == "Laser" || col.gameObject.tag == "Stun Laser")
    //    {
    //        health -= col.GetComponent<Laser>().damage;
    //        Instantiate(hit, col.gameObject.transform.position, Quaternion.identity);
    //    }
    //}
}
