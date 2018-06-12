using UnityEngine;
using System.Collections;

public class DestroyItself : MonoBehaviour 
{
    float lifeTime = 5;

    void Awake()
    {
        Destroy(this.gameObject, lifeTime);
    }
}
