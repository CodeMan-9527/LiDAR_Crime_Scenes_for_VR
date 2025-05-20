using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Rotator : MonoBehaviour
{
    private Vector3 speed; // Rotation speed in degrees per second
    void Update()
    {
        // Rotate the object around its local Y axis at the specified speed
        transform.Rotate(speed * Time.deltaTime,Space.Self);
    }
}

    

