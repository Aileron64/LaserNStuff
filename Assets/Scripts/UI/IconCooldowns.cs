using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconCooldowns : MonoBehaviour
{
    Image dashIcon;
    Image pShotIcon;
    Image novaIcon;
    Image chronoIcon;
    Image bubbleIcon;

    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");

        dashIcon = transform.FindChild("Dash").gameObject.transform.FindChild("Icon").GetComponent<Image>();
        pShotIcon = transform.FindChild("Power Shot").gameObject.transform.FindChild("Icon").GetComponent<Image>();
        novaIcon = transform.FindChild("Nova").gameObject.transform.FindChild("Icon").GetComponent<Image>();
        chronoIcon = transform.FindChild("Chronoshpere").gameObject.transform.FindChild("Icon").GetComponent<Image>();
        bubbleIcon = transform.FindChild("Bubble").gameObject.transform.FindChild("Icon").GetComponent<Image>();
    }

    void Update()
    {
        dashIcon.fillAmount = player.GetComponent<Player>().GetCooldown("Dash");

        pShotIcon.fillAmount = player.GetComponent<Player>().GetCooldown("Power Shot");

        novaIcon.fillAmount = player.GetComponent<Player>().GetCooldown("Nova");

        chronoIcon.fillAmount = player.GetComponent<Player>().GetCooldown("Chronoshpere");

        bubbleIcon.fillAmount = player.GetComponent<Player>().GetCooldown("Bubble");
    }
}