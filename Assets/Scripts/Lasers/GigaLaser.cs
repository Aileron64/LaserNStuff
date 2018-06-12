using UnityEngine;
using System.Collections;

public class GigaLaser : MonoBehaviour 
{

    public Transform endPoint;
    LineRenderer laserLine;

    float minWidth = 1;
    float maxWidth = 15;
    float width;

	void Start () 
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.SetWidth(minWidth, minWidth);
        width = minWidth;
	}
	
	void Update () 
    {
        if(width <= maxWidth)
        {
            width += Time.deltaTime * 10;
            laserLine.SetWidth(width, width);
        }

        laserLine.SetPosition(0, transform.position);
        laserLine.SetPosition(1, endPoint.position);
	}
}
