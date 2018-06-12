using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour 
{
    GameObject player;

    public GameObject drone;
    public GameObject crossCannon;
    public GameObject motherShip;
    public GameObject boxBoy;
    public GameObject gigaCannon;
    public GameObject hWing;

    public int waveNum = 0;
    public float spawnTimer = 25;
    public float timer;

    public bool active = true;
    public bool testWave = false;


    float spawnDelay = 200;

    float maxDistance = 800;
    float minDistance = 200;

    float difficultyScale = 0.1f;

    void Start()
    {
        player = GameObject.Find("Player");
        timer = spawnTimer - 1;
    }

    void Update()
    {
        if(active)
        {
            timer += Time.deltaTime;

            if (timer >= spawnTimer)
            {
                if(testWave)
                {
                    SpawnEnemy(gigaCannon, 10);
                }
                else
                {
                    int rand = Random.Range(1, 10);
                    //Debug.Log("Spawning Wave #" + rand);

                    switch (rand)
                    {
                        default:
                        case 1:
                            SpawnEnemy(motherShip, 1);
                            SpawnEnemy(hWing, 6);
                            SpawnEnemy(drone, 25);
                            break;

                        case 2:
                            SpawnEnemy(boxBoy, 4);
                            SpawnEnemy(drone, 15);
                            SpawnEnemy(motherShip, 1);
                            SpawnEnemy(hWing, 3);
                            break;

                        case 3:
                            SpawnEnemy(gigaCannon, 2);
                            SpawnEnemy(drone, 20);
                            SpawnEnemy(hWing, 3);
                            break;

                        case 4:
                            SpawnEnemy(crossCannon, 10);
                            SpawnEnemy(drone, 10);
                            SpawnEnemy(gigaCannon, 1);
                            break;

                        case 5:
                            SpawnEnemy(crossCannon, 3);
                            SpawnEnemy(motherShip, 1);
                            SpawnEnemy(drone, 10);
                            break;

                        case 6:
                            SpawnEnemy(crossCannon, 5);
                            SpawnEnemy(boxBoy, 2);
                            SpawnEnemy(hWing, 5);
                            SpawnEnemy(drone, 15);
                            break;

                        case 7:
                            SpawnEnemy(boxBoy, 3);
                            SpawnEnemy(gigaCannon, 2);
                            SpawnEnemy(hWing, 2);
                            SpawnEnemy(drone, 7);
                            break;

                        case 8:
                            SpawnEnemy(motherShip, 2);
                            SpawnEnemy(crossCannon, 4);
                            SpawnEnemy(drone, 5);
                            break;

                        case 9:
                            SpawnEnemy(hWing, 9);
                            SpawnEnemy(gigaCannon, 1);
                            SpawnEnemy(drone, 12);
                            break;
                    }
                }

                waveNum++;
                timer = 0;
            }
        }
    }


    void SpawnEnemy(GameObject mob, int amount)
    {
        Vector3 spawnLocation;

        amount = (int)(amount * (1 + (waveNum * difficultyScale)));

        for (int i = 0; i < amount; i++)
        {
            float distance = -1000 - (i * spawnDelay);

            switch (Random.Range(0, 4))
            {
                default:
                case 0:
                    spawnLocation = new Vector3(Random.Range(minDistance, maxDistance),
                        distance, Random.Range(-maxDistance, maxDistance))
                        + player.transform.position;
                    break;

                case 1:
                    spawnLocation = new Vector3(Random.Range(-maxDistance, -minDistance),
                         distance, Random.Range(-maxDistance, maxDistance))
                         + player.transform.position;
                    break;

                case 2:
                    spawnLocation = new Vector3(Random.Range(-maxDistance, maxDistance),
                        distance, Random.Range(minDistance, maxDistance))
                        + player.transform.position;
                    break;

                case 3:
                    spawnLocation = new Vector3(Random.Range(-maxDistance, maxDistance),
                        distance, Random.Range(-maxDistance, -minDistance))
                        + player.transform.position;
                    break;
            }

            GameObject clone = Instantiate(mob, spawnLocation, transform.rotation) as GameObject;
            clone.GetComponent<BaseAI>().gameState = GameState.WARP;

        }
    }
}
