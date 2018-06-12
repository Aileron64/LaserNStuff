using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainCanvas : MonoBehaviour 
{
    GameObject player;
    GameObject spawner;

    Text waveInfo;
    Text waveTimer;
    Text gameOver;
    Text warning;
    Text warning2;
    Text instructions;
    Text debug;

    float lagTime;

    //public static string bigText;

    bool debugText = false;

    void Start()
    {
        player = GameObject.Find("Player");
        spawner = GameObject.Find("Spawner");

        waveInfo = transform.FindChild("Wave Info").GetComponent<Text>();
        waveTimer = transform.FindChild("Wave Time").GetComponent<Text>();
        gameOver = transform.FindChild("Game Over Text").GetComponent<Text>();
        warning = transform.FindChild("Warning Text").GetComponent<Text>();
        warning2 = transform.FindChild("Warning Text 2").GetComponent<Text>();
        instructions = transform.FindChild("Instructions").GetComponent<Text>();
        debug = transform.FindChild("Debug").GetComponent<Text>();

        //if (Application.isEditor)
        //    isEditor = true;
    }

    void Update()
    {
        waveInfo.text = string.Format("WAVE # {0}", (int)(spawner.GetComponent<Spawn>().waveNum));
        waveTimer.text = string.Format("NEXT WAVE: {0}",
            (int)(spawner.GetComponent<Spawn>().spawnTimer - spawner.GetComponent<Spawn>().timer));

        if (player.GetComponent<Player>().health <= 0)
        {
            gameOver.text = "GAME OVER";
            instructions.text = "Press 'n'";
            spawner.GetComponent<Spawn>().active = false;

            if ((Input.GetKey(KeyCode.N)))
            {
                Application.LoadLevel(0);
            }
        }
        else if (Vector3.Magnitude(player.transform.position) > 5000 &&
                 Vector3.Magnitude(player.transform.position) <= 5200)
        {

        }
        else if (Vector3.Magnitude(player.transform.position) > 5000)
        {

        }
        else
        {
            gameOver.text = "";
        }

        if (player.GetComponent<Player>().energy <= 20)
            warning.text = "ENERGY LOW";
        else
            warning.text = "";

        if (player.GetComponent<Player>().SolarDistance() >= player.GetComponent<Player>().solarMilestone[3])
            warning2.text = "RETURN TO CORE";
        else
            warning2.text = "";

        

        if ((Input.GetKeyDown(KeyCode.BackQuote)))
        {
            debugText = !debugText;
        }

        if (1.0f / Time.deltaTime <= 20)
        {
            lagTime += Time.deltaTime;
        }

        if (debugText)
            debug.text = string.Format("Solar Distance: {0:f2}\nFPS: {1}\nLagtime: {2}", 
                player.GetComponent<Player>().SolarDistance(), (int)(1.0f / Time.deltaTime), lagTime);
        else
            debug.text = "";

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
            
    }
}
