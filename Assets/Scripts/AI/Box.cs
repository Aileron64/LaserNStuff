using UnityEngine;
using System.Collections;

public class Box : BaseAI
{
    void Start()
    {
        health = 100;
        explosionAmount = 25;
        explosionSpeed = 30;
    }

}
