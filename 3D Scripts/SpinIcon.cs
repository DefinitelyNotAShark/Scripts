using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script does: spins object
//This script goes on: the object
//This script requires: that the object is 3D

public class SpinIcon : MonoBehaviour
{
    float speed = 100;
	
    private void FixedUpdate()
    {
        Spin();
    }

    private void Spin()
    {
        transform.Rotate(Vector3.up, speed* 2 * Time.deltaTime);      
    }
}
