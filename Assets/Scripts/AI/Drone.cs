using UnityEngine;
using System.Collections;

public class Drone : BaseAI
{
    Vector3 dashVelocity;
    GameObject target;

    
    float initialRotateSpeed = 50;
    float spinTime = 0.7f;
    float dashSpeed = 300;

    float initialSpeed;
    float speed;
    float timer;
    float targetDistance;
    float spinStartDistance;
    float dashTime;

    enum State
    {
        SEEK, SPIN, DASH
    }

    State state = State.SEEK;

    void Start() 
    {

        health = 100;
        explosionAmount = 25;
        explosionSpeed = 30;

        target = TargetPlayer();

        initialSpeed = Random.Range(50, 100);
        spinStartDistance = Random.Range(75, 250);
        dashTime = Random.Range(0.5f, 2.0f);
	}

    override protected void Update() 
    {
        if (gameState == GameState.NORMAL)
        {
            targetDistance = Vector3.Magnitude(target.transform.position - transform.position);

            switch (state)
            {
                default:  /// Chase Player
                case State.SEEK:
                    speed = initialSpeed;

                    if (rotateSpeed > initialRotateSpeed)
                        rotateSpeed -= 500;
                    else
                        rotateSpeed = initialRotateSpeed;

                    if (targetDistance < spinStartDistance)
                    {
                        state = State.SPIN;
                    }


                    velocity = Vector3.Normalize(target.transform.position - transform.position) * speed;
                    break;

                case State.SPIN:  /// Spin to look cool
                    rotateSpeed += 1000 * Time.deltaTime;

                    timer += Time.deltaTime;
                    if (timer >= spinTime)
                    {
                        timer = 0;
                        dashVelocity = Vector3.Normalize(target.transform.position - transform.position) * dashSpeed;
                        state = State.DASH;
                    }

                    velocity = Vector3.Normalize(target.transform.position - transform.position) * speed / 2;
                    //velocity.y = -5;
                    break;

                case State.DASH:  /// Dash attack
                    timer += Time.deltaTime;
                    if (timer >= dashTime)
                    {
                        timer = 0;
                        state = State.SEEK;
                    }
                    velocity = dashVelocity;

                    break;
            }
        }

        base.Update();
	}

    //protected override void Explode()
    //{
    //    Instantiate(explosion, transform.position, Quaternion.identity);

    //    Destroy(this.gameObject);
    //}
}
