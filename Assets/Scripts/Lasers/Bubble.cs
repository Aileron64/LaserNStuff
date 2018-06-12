using UnityEngine;
using System.Collections;

public class Bubble : Laser 
{
    float maxSize;
    float size;
    bool isClosing = false;

    protected override void Start() 
    {
        damage = 1200;

        maxSize = transform.localScale.x;
        transform.localScale = new Vector3(1, 1, 1);
	}
	
    protected override void Update()
    {
	    if (size < maxSize && !isClosing)
        {
            size += Time.deltaTime * 500;  
        }
        else if (!isClosing)
        {
            size = maxSize;
        }
        else
        {
            size -= Time.deltaTime * 500;

            if (size <= 1)
            {
                size = 1;
                isClosing = false;
                gameObject.SetActive(false);
            }
        }

        transform.localScale = new Vector3(size, size, size);
	}

    public void Close()
    {
        isClosing = true;
    }

    protected override void OnTriggerEnter(Collider col)
    { 
    }
}
