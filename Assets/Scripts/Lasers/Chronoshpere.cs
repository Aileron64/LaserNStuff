using UnityEngine;
using System.Collections;

public class Chronoshpere : MonoBehaviour 
{

    float maxSize = 420;
    float size = 1;
    float lifeTime = 8;
    float timer;

    bool shrink = false;

	// Use this for initialization
	void Start () 
    {
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
	}
	
	// Update is called once per frame
	void Update () 
    {
        timer += Time.deltaTime;

        if (timer >= (lifeTime))
            shrink = true;

        if (size < maxSize && !shrink)
        {
            size += Time.deltaTime * 1000;
            transform.localScale = new Vector3(size, size, size);
        }
        else if (shrink)
        {
            if (size >= 0)
                size -= Time.deltaTime * 100;
            else
                Destroy(this.gameObject);         

            transform.localScale = new Vector3(size, size, size);
        }
        else
        {
            size = maxSize;
            transform.localScale = new Vector3(size, size, size);
        }

        //transform.Rotate(new Vector3(0, 0.1f, 0));
	}
}
