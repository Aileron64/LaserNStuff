using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StatusBars : MonoBehaviour 
{
    GameObject player;
    GameObject bar1;
    Text text1;
    Text timeText1;

    float time1;
    float timer1;

    bool bar1Active = false;

    float bubbleCheat = 0;

	void Start () 
    {
        player = GameObject.Find("Player");
        bar1 = transform.FindChild("Bar 1").gameObject;
        text1 = transform.FindChild("Text 1").GetComponent<Text>();
        timeText1 = transform.FindChild("Time 1").GetComponent<Text>();

        //StatusBar(4, "BALLS");
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (bar1Active)
        {
            timer1 += Time.deltaTime;
            bar1.transform.localScale = new Vector3((time1 - timer1) / time1, 1, 1);

            timeText1.text = string.Format("{0:f2}", time1 - timer1);

            if (timer1 >= time1)
            {
                text1.text = "";
                timeText1.text = "";
                bar1.SetActive(false);
                bar1Active = false;
                bubbleCheat = 0;
            }           
        }
        else
        {
            // This is so stupid

            if (bubbleCheat == 0)
                bubbleCheat = player.GetComponent<Player>().CheckBubble();
            else
                StatusBar(bubbleCheat, "Shield");
        }
	}


    public void StatusBar(float time, string name)
    {
        timer1 = 0;
        time1 = time;
        text1.text = name;

        bar1.SetActive(true);
        bar1Active = true;
        
    }
}
