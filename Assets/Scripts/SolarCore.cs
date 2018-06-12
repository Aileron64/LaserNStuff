using UnityEngine;
using System.Collections;

public class SolarCore : MonoBehaviour 
{
    GameObject sun;
    GameObject[] ring = new GameObject[3];

    void Start()
    {
        sun = transform.FindChild("Sun").gameObject;
        ring[0] = transform.FindChild("Ring 0").gameObject;
        ring[1] = ring[0].transform.FindChild("Ring 1").gameObject;
        ring[2] = ring[1].transform.FindChild("Ring 2").gameObject;
    }

	void Update () 
    {
        sun.transform.Rotate(new Vector3(0, 0.2f, 0));

        ring[0].transform.Rotate(new Vector3(0.5f, 0.5f, 0));
        ring[1].transform.Rotate(new Vector3(0.5f, 0, 0));
        ring[2].transform.Rotate(new Vector3(0, 0, 0.8f));
	}
}
