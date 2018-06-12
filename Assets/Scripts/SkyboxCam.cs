using UnityEngine;
using System.Collections;

public class SkyboxCam : MonoBehaviour 
{

    public Vector3 rotation;


    void Start()
    {
        rotation = transform.rotation.eulerAngles;
    }
	

	void Update () 
    {
        rotation.x += Time.deltaTime * 50;
        //rotation.y += Time.deltaTime * 50;
        //rotation.z += Time.deltaTime * 50;

        //transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation);
	}
}
