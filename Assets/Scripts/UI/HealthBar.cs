using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HealthBar : MonoBehaviour 
{
    float playerMaxHealth;
    float playerHealth;
    float healthPercent = 1;

    GameObject player;
    RawImage bar;
    Text text;

	void Start () 
    {
        player = GameObject.Find("Player");
        playerMaxHealth = player.GetComponent<Player>().maxHealth;

        bar = transform.FindChild("Bar").GetComponent<RawImage>();
        text = transform.FindChild("Text").GetComponent<Text>();
	}

	void Update () 
    {
        playerHealth = player.GetComponent<Player>().health;

        if (healthPercent > 0)
            healthPercent = playerHealth / playerMaxHealth;
        else
            healthPercent = 0;

        text.text = string.Format("{0} %", (int)(healthPercent * 100));
      
        bar.transform.localScale = new Vector3(healthPercent, 1, 1);
	}
}
