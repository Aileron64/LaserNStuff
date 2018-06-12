using UnityEngine;
using System.Collections;

public class BoxBoy : BaseAI
{
    GameObject target;
    public GameObject box;
    Vector3 nextStop;

    float speed = 50;
    float buildDistance = 300;
    float buildHeight = 25;
    float seekDistance = 1000;
    float delay = 0.5f;
    float upDownSpeed = 50;
    float boxGap = 27;
    int boxMax = 3;
    int boxMin = 8;

    float timer;
    float targetDistance;
    int boxCount;

    enum State
    {
        SEEK, BUILD, WAIT, MOVE
    }
    State state = State.SEEK;

    enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
    Direction direction;

    void Start() 
    {
        health = 1200;
        impactDamage = 30;
        explosionAmount = 200;
        explosionSpeed = 80;

        float rand = Random.Range(0, 3);

        if (rand <= 1)
        {
            direction = Direction.UP;
            rotation.y = 0;
        }    
        else if (rand <= 2)
        {
            direction = Direction.DOWN;
            rotation.y = 180;
        }    
        else if (rand <= 3)
        {
            direction = Direction.LEFT;
            rotation.y = 270;
        }      
        else
        {
            direction = Direction.RIGHT;
            rotation.y = 90;
        }

        boxCount = Random.Range(boxMin, boxMax);
        target = TargetPlayer();
	}

    override protected void Update() 
    {
        targetDistance = Vector3.Magnitude(target.transform.position - transform.position);

	    switch(state)
        {
            case State.SEEK:

                if (targetDistance < buildDistance)
                {
                    nextStop = transform.position;
                    nextStop.y = buildHeight;
                    state = State.MOVE;
                }

                velocity = Vector3.Normalize(target.transform.position - transform.position) * speed;
                break;




            case State.BUILD:
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    BuildBox();
                    state = State.WAIT;
                    timer = 0;
                    boxCount--;
                }

                velocity = new Vector3(0, upDownSpeed * -1, 0);
                break;




            case State.WAIT:
                timer += Time.deltaTime;
                if (timer >= delay)
                {                 
                    nextStop = transform.position;
                    nextStop.y = buildHeight;

                    if (boxCount <= 0)
                        ChangeDirection();

                    switch(direction)
                    {
                        case Direction.UP:
                            nextStop.x += boxGap;
                            break;

                        case Direction.DOWN:
                            nextStop.x -= boxGap;
                            break;

                        case Direction.LEFT:
                            nextStop.z += boxGap;
                            break;

                        case Direction.RIGHT:
                            nextStop.z -= boxGap;
                            break;
                    }
                    state = State.MOVE;
                    timer = 0;
                }


                velocity = new Vector3(0, upDownSpeed * 1, 0);
                break;




            case State.MOVE:

                if (Vector3.Magnitude(nextStop - transform.position) <= 0.5f)                
                    state = State.BUILD;

                if (targetDistance > seekDistance)
                    state = State.SEEK;

                velocity = Vector3.Normalize(nextStop - transform.position) * speed;

                break;
        }



        base.Update();
	}

    void BuildBox()
    {
         GameObject clone = Instantiate(box, transform.position, transform.rotation) as GameObject;
         //complety pointless but clone has to do something because unity
         clone.transform.rotation = Quaternion.Euler(rotation); 
    }

    void ChangeDirection()
    {
        float rand = Random.Range(0, 2);

        switch (direction)
        {
            default:
            case Direction.UP:
            case Direction.DOWN:
                if (rand >= 1)
                {
                    direction = Direction.RIGHT;
                    rotation.y = 90;
                }  
                else
                {
                    direction = Direction.LEFT;
                    rotation.y = 270;
                }      
                break;

            case Direction.LEFT:
            case Direction.RIGHT:
                if (rand >= 1)
                {
                    direction = Direction.UP;
                    rotation.y = 0;
                }              
                else
                {
                    direction = Direction.DOWN;
                    rotation.y = 180;
                }     
                break;
        }

        boxCount = Random.Range(boxMin, boxMax);


    }

}
