using UnityEngine;
using System.Collections;

public class GigaCannon : BaseAI 
{
    GameObject target;
    float targetDistance;
    float spinDistance = 300;
    float speed = 50;

    GameObject[] piston = new GameObject[4];

    public Transform startPoint;
    public Transform endPoint;

    GameObject laser;

    float timeDelay = 1;
    float timer;

    bool spining = false;

    

    enum State
    {
        SEEK, ROTATE, SPINNING_UP
    }
    State state = State.SEEK;

	void Start () 
    {
        health = 2000;
        impactDamage = 40;
        explosionAmount = 520;
        explosionSpeed = 50;

        rotation = transform.rotation.eulerAngles;
        rotation.z = 50;

        laser = transform.FindChild("Laser").gameObject;

        target = TargetPlayer();
        

        rotateSpeed = 0;

        for (int i = 0; i <= 3; i++)
        {
            piston[i] = transform.FindChild("" + i).gameObject;
        }
    }


    override protected void Update() 
    {
        if (gameState == GameState.NORMAL)
        {
            targetDistance = Vector3.Magnitude(target.transform.position - transform.position);

            switch (state)
            {
                default:
                case State.SEEK:
                    if (targetDistance < spinDistance)
                    {
                        state = State.ROTATE;
                    }

                    rotation.y = (Mathf.Atan2(transform.position.x - target.transform.position.x,
                        transform.position.z - target.transform.position.z) * Mathf.Rad2Deg) - 90;

                    velocity = Vector3.Normalize(target.transform.position - transform.position) * speed;
                    break;

                case State.ROTATE:
                    rotation.z -= Time.deltaTime * 25;

                    if (rotation.z <= 0)
                    {
                        rotation.z = 0;
                        timer += Time.deltaTime;
                    }

                    if (timer >= timeDelay)
                    {
                        laser.SetActive(true);
                        timer = 0;
                        state = State.SPINNING_UP;
                    }

                    velocity = Vector3.Normalize(target.transform.position - transform.position) * speed * 0.75f;
                    break;

                case State.SPINNING_UP:

                    timer += Time.deltaTime;


                    if (timer >= timeDelay && !spining)
                    {
                        spining = true;

                        /// Checks which way the player is
                        float angle = (Mathf.Atan2(transform.position.x - target.transform.position.x,
                            transform.position.z - target.transform.position.z) * Mathf.Rad2Deg) - 90;

                        if (angle - rotation.z >= 0 || angle - rotation.z <= -180)
                            rotateSpeed = 30;
                        else
                            rotateSpeed = -30;

                    }
                    velocity = Vector3.zero;
                    break;
            }

            //theta += pistonSpeed * Time.deltaTime;
            //piston[0].transform.position = new Vector3(4.5f, -8.5f * Mathf.Sin(theta), 18);

            //laserLine.SetPosition(0, startPoint.position);
            //laserLine.SetPosition(1, endPoint.position);
        }

        base.Update();
	}

    protected override void ChangeMat(Material mat)
    {
        GetComponent<Renderer>().material = mat;

        for (int i = 0; i <= 3; i++)
        {
            piston[i].GetComponent<Renderer>().material = mat;
        }
    }
}
