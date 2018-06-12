using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnergyBar : MonoBehaviour
{
    float playerEnergy;

    GameObject player;
    RawImage bar;
    Text text;

    GameObject[] point = new GameObject[4];
    int distanceNum = 4;

    void Start()
    {
        player = GameObject.Find("Player");    

        bar = transform.FindChild("Bar").GetComponent<RawImage>();
        text = transform.FindChild("Text").GetComponent<Text>();

        for (int i = 0; i <= 3; i++)
        {
            point[i] = transform.FindChild("" + i).gameObject;
        }
    }

    void Update()
    {
        playerEnergy = player.GetComponent<Player>().energy;

        if (playerEnergy < 0)
            playerEnergy = 0;

        text.text = string.Format("{0}", (int)(playerEnergy));

        bar.transform.localScale = new Vector3(playerEnergy / 100, 1, 1);


        switch (distanceNum)
        {
            default:
            case 4:
                if (player.GetComponent<Player>().SolarDistance() >= player.GetComponent<Player>().solarMilestone[0]) // 500
                {
                    distanceNum = 3;
                    point[3].SetActive(false);
                }
                break;

            case 3:
                if (player.GetComponent<Player>().SolarDistance() <= player.GetComponent<Player>().solarMilestone[0]) // 500
                {
                    distanceNum = 4;
                    point[3].SetActive(true);
                }
                
                if (player.GetComponent<Player>().SolarDistance() >= player.GetComponent<Player>().solarMilestone[1]) // 1000
                {
                    distanceNum = 2;
                    point[2].SetActive(false);
                }
                break;


            case 2:
                if (player.GetComponent<Player>().SolarDistance() <= player.GetComponent<Player>().solarMilestone[1]) // 1000
                {
                    distanceNum = 3;
                    point[2].SetActive(true);
                }

                if (player.GetComponent<Player>().SolarDistance() >= player.GetComponent<Player>().solarMilestone[2]) // 1500
                {
                    distanceNum = 1;
                    point[1].SetActive(false);
                }
                break;

            case 1:
                if (player.GetComponent<Player>().SolarDistance() <= player.GetComponent<Player>().solarMilestone[2]) //
                {
                    distanceNum = 2;
                    point[1].SetActive(true);
                }

                if (player.GetComponent<Player>().SolarDistance() >= player.GetComponent<Player>().solarMilestone[3])
                {
                    distanceNum = 0;
                    point[0].SetActive(false);
                }
                break;

            case 0: 
                if (player.GetComponent<Player>().SolarDistance() <= player.GetComponent<Player>().solarMilestone[3])
                {
                    distanceNum = 1;
                    point[0].SetActive(true);
                }
                break;
        }
    }
}
