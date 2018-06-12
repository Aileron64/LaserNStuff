using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour 
{
    //AudioSource audio;

	void Start () 
    {
        //audio = GetComponent<AudioSource>();

        if (!Application.isEditor)
            GetComponent<AudioSource>().Play();

        //if (!Application.isEditor)
        //    audio.Play();
	}
	

}
